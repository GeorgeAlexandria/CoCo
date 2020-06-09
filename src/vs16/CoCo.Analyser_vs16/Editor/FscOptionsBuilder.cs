using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CoCo.Utils;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;

namespace CoCo.Analyser.Editor
{
    internal class FscOptionsBuilder
    {
        private static readonly char[] _referencePathDelimeters = new[] { ';', ',' };
        private static readonly char[] _nowarnDelimeters = new[] { ' ', ';', ',', '\r', '\n' };
        private static readonly char[] _defineConstsDelimeters = new[] { ';' };
        private static readonly char[] _warningAsErrorDelimeters = new[] { ' ', ';', ',' };

        private readonly string _projectFilePath;

        private ProjectCollection _projectCollection;
        private Project _msbuildProject;
        private List<string> _options;

        public FscOptionsBuilder(string projectFilePath)
        {
            _projectFilePath = projectFilePath;
        }

        /// <remarks>
        /// Generates fsc command options the similar with
        /// https://github.com/dotnet/fsharp/blob/master/src/fsharp/FSharp.Build/Fsc.fs#L86 and retrieves options as
        /// https://github.com/dotnet/fsharp/blob/master/src/fsharp/FSharp.Build/Microsoft.FSharp.Targets#L277
        /// </remarks>
        public string[] Build()
        {
            var projectWasLoadedBefore = false;
            var loadedProjects = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(_projectFilePath);
            if (loadedProjects is null || loadedProjects.Count == 0)
            {
                InitializeProjectCollection();

                _msbuildProject = new Project(
                    _projectFilePath, _projectCollection.GlobalProperties, _projectCollection.DefaultToolsVersion, _projectCollection);
            }
            else
            {
                _msbuildProject = loadedProjects.First();
                projectWasLoadedBefore = true;
            }

            _options = new List<string>();

            AppendProperty("TargetProfile", "--targetprofile:");
            AppendProperty("SubsystemVersion", "--subsystemversion:");
            AppendProperty("PreferredUILang", "--preferreduilang:");
            AppendProperty("WarningLevel", "--warn:");
            AppendProperty("LCID", "--LCID:");
            AppendProperty("SourceLink", "--sourcelink:");
            AppendProperty("DotnetFscCompilerPath", null);
            AppendProperty("OtherFlags", null);

            AppendKeyByProperty("Optimize", "--optimize+", "--optimize-");
            AppendKeyByProperty("Tailcalls", null, "--tailcalls-");
            AppendKeyByProperty("TreatWarningsAsErrors", "--warnaserror", null);
            AppendKeyByProperty("EmbedAllSources", "--embed+", "--embed-");
            AppendKeyByProperty("HighEntropyVA", "--highentropyva+", "--highentropyva-");

            AppendItems("EmbeddedFiles", "--embed:");
            AppendItems("ActualEmbeddedResources", "--resource:");

            // TODO: perhaps that referencepath contains not full (absolute) path at this moment!
            AppendPropertyAsEnumeration("ReferencePath", "--lib:", _referencePathDelimeters);
            AppendPropertyAsEnumeration("NoWarn", "--nowarn:", _nowarnDelimeters);

            _options.Add("--noframework");

            var property = _msbuildProject.GetProperty("DefineConstants");
            if (!(property is null))
            {
                foreach (var item in property.EvaluatedValue.Split(_defineConstsDelimeters, StringSplitOptions.RemoveEmptyEntries))
                {
                    _options.Add(Combine("--define:", item));
                }
            }

            property = _msbuildProject.GetProperty("WarningsAsErrors");
            {
                var builder = StringBuilderCache.Acquire();
                builder.Append("--warnaserror:76");
                if (!(property is null))
                {
                    foreach (var item in property.EvaluatedValue.Split(_warningAsErrorDelimeters, StringSplitOptions.RemoveEmptyEntries))
                    {
                        builder.Append(',').Append(item);
                    }
                }
                _options.Add(StringBuilderCache.Release(builder));
            }

            property = _msbuildProject.GetProperty("PlatformTarget");
            if (!(property is null))
            {
                if (property.EvaluatedValue.EqualsNoCase("anycpu"))
                {
                    _options.Add((_msbuildProject.GetProperty("Actual32Bit")?.EvaluatedValue).IsTrue()
                        ? "--platform:anycpu32bitpreferred"
                        : "--platform:anycpu");
                }
                else if (property.EvaluatedValue.EqualsNoCase("x86"))
                {
                    _options.Add("--platform:x86");
                }
                else if (property.EvaluatedValue.EqualsNoCase("x64"))
                {
                    _options.Add("--platform:x64");
                }
            }

            if (!projectWasLoadedBefore)
            {
                _projectCollection.UnloadAllProjects();
            }
            var result = _options.ToArray();
            _msbuildProject = null;
            _options = null;

            return result;
        }

        private void InitializeProjectCollection()
        {
            if (!(_projectCollection is null)) return;

            var globalCollection = ProjectCollection.GlobalProjectCollection;
            var globalProperties = new Dictionary<string, string>();
            foreach (var (key, value) in globalCollection.GlobalProperties)
            {
                globalProperties[key] = value;
            }

            _projectCollection = new ProjectCollection(globalProperties, Array.Empty<ILogger>(), globalCollection.ToolsetLocations);
        }

        private void AppendProperty(string name, string key)
        {
            var property = _msbuildProject.GetProperty(name);
            if (property is null) return;
            _options.Add(key is null ? property.EvaluatedValue : Combine(key, property.EvaluatedValue));
        }

        private void AppendKeyByProperty(string propertyName, string trueKey, string falseKey)
        {
            var property = _msbuildProject.GetProperty(propertyName);
            if (property is null) return;

            if (!string.IsNullOrWhiteSpace(trueKey) && !string.IsNullOrWhiteSpace(falseKey))
            {
                _options.Add(property.EvaluatedValue.IsTrue() ? trueKey : falseKey);
            }
            else if (!string.IsNullOrWhiteSpace(falseKey))
            {
                if (property.EvaluatedValue.IsFalse())
                {
                    _options.Add(falseKey);
                }
            }
            else if (!string.IsNullOrWhiteSpace(trueKey))
            {
                if (property.EvaluatedValue.IsTrue())
                {
                    _options.Add(trueKey);
                }
            }
        }

        private void AppendItems(string itemName, string key)
        {
            foreach (var item in _msbuildProject.GetItems(itemName))
            {
                _options.Add(Combine(key, item.EvaluatedInclude));
            }
        }

        private void AppendPropertyAsEnumeration(string propertyName, string key, params char[] delimeters)
        {
            var property = _msbuildProject.GetProperty(propertyName);
            if (property is null) return;

            var builder = StringBuilderCache.Acquire();
            builder.Append(key);
            var needToAppend = false;
            foreach (var item in property.EvaluatedValue.Split(delimeters, StringSplitOptions.RemoveEmptyEntries))
            {
                if (needToAppend)
                {
                    builder.Append(',');
                }
                builder.Append(item);
                needToAppend = true;
            };
            _options.Add(StringBuilderCache.Release(builder));
        }

        private string Combine(string str1, string str2) =>
            StringBuilderCache.Release(StringBuilderCache.Acquire().Append(str1).Append(str2));
    }

    /// <remarks>
    /// Extracts options by reflection from the loaded into memory VisualStudioWorkspace and IProvideProjectSite
    /// </remarks>
    internal class FscOptionsBuilder2
    {
        private readonly Microsoft.CodeAnalysis.Workspace workspace;
        private readonly Microsoft.CodeAnalysis.Project project;

        public FscOptionsBuilder2(Microsoft.CodeAnalysis.Workspace workspace, Microsoft.CodeAnalysis.Project project)
        {
            this.workspace = workspace;
            this.project = project;
        }

        public string[] Build()
        {
            var projectIdParameter = Expression.Parameter(typeof(Microsoft.CodeAnalysis.ProjectId));
            var call = Expression.Call(Expression.Constant(workspace), "GetHierarchy", Array.Empty<Type>(), projectIdParameter);
            var vsHierarchy = Expression.Lambda<Func<Microsoft.CodeAnalysis.ProjectId, object>>(call, projectIdParameter).Compile()(project.Id);

            if (vsHierarchy is null) return Array.Empty<string>();

            var projectSite = Expression.Call(Expression.Constant(vsHierarchy), "Microsoft-VisualStudio-FSharp-Editor-IProvideProjectSite-GetProjectSite", Array.Empty<Type>());
            var compilationOptions = Expression.Lambda<Func<string[]>>(Expression.PropertyOrField(projectSite, "CompilationOptions")).Compile()();
            return compilationOptions;
        }
    }
}