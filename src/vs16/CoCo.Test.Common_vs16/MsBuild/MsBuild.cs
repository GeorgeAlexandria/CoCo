using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using CoCo.Logging;
using CoCo.Utils;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

namespace CoCo.Test.Common.MsBuild
{
    // NOTE: to know more about input arguments in a common tasks just look at
    // https://github.com/Microsoft/msbuild/blob/master/src/Tasks/Microsoft.Common.CurrentVersion.targets
    public static class MsBuild
    {
        /// TODO: use <see cref="WeakReference{T}"/> when <see cref="ProjectInfo"/> would be take a lot of space
        private static readonly Dictionary<string, ProjectInfo> _cache = new Dictionary<string, ProjectInfo>(16);

        private static readonly string[] searchDelimeters = { Environment.NewLine, ";" };

        private static readonly string[] allowedAssemblyExtensions = { ".dll" };

        /// <summary>
        /// Get existing project for <paramref name="projectPath"/> or create a new
        /// </summary>
        public static ProjectInfo GetProject(string projectPath)
        {
            if (!_cache.TryGetValue(projectPath, out ProjectInfo projectInfo))
            {
                projectInfo = ParseProject(projectPath);
                _cache.Add(projectPath, projectInfo);
            }
            return projectInfo;
        }

        private static ProjectInfo ParseProject(string projectPath)
        {
            var project = new Project(projectPath);

            var assemblyReferences = ResolveAssemblyReferences(project);
            var referencesBuilder = ImmutableArray.CreateBuilder<string>(assemblyReferences.Length);
            foreach (var item in assemblyReferences)
            {
                referencesBuilder.Add(item.ItemSpec);
            }
            var references = referencesBuilder.TryMoveToImmutable();

            var projects = GetProjectReferences(project);
            var compileItems = GetCompileItems(project);
            var imports = GetImports(project);
            var rootNamespace = project.GetPropertyValue("RootNamespace");

            var optionCompare = project.GetPropertyValue("OptionInfer").EqualsNoCase("Text");
            var optionExplicit = project.GetPropertyValue("OptionExplicit").IsOn();
            var optionInfer = project.GetPropertyValue("OptionInfer").IsOn();
            // TODO: strict should be retrieved using "warning as error", because Rolsyn has a three states for the strict option
            var optionStrict = project.GetPropertyValue("OptionStrict").IsOn();

            var outputFilePath = project.GetPropertyValue("TargetPath");
            var language = project.GetPropertyValue("Language");

            return new ProjectInfo(
                projectPath, references, projects, compileItems, imports, outputFilePath, rootNamespace, language,
                optionCompare, optionExplicit, optionInfer, optionStrict);
        }

        // NOTE: https://github.com/Microsoft/msbuild/wiki/ResolveAssemblyReference
        private static ITaskItem[] ResolveAssemblyReferences(Project project)
        {
            var searchPaths = GetSearchPaths(project);
            var frameworkDirectories = GetFrameworkDirectories(project);
            var references = GetReferences(project);
            var explicitReferences = GetAssemblyFiles(project);
            var appConfigFile = GetAppConfigFile(project);

            /// TODO: each project will create the same logger -> use pool of objects in <see cref="LogManager"/>
            using (var logger = LogManager.GetLogger("ResolveReference"))
            {
                logger.Info("Executing ResolveAssemblyReference task...");
                var resolveTask = new ResolveAssemblyReference
                {
                    BuildEngine = new MsBuildEngine(logger),
                    Assemblies = references,
                    AssemblyFiles = explicitReferences,
                    TargetFrameworkVersion = project.GetPropertyValue("TargetFrameworkVersion"),
                    TargetFrameworkMoniker = project.GetPropertyValue("TargetFrameworkMoniker"),
                    SearchPaths = searchPaths,
                    TargetFrameworkDirectories = frameworkDirectories,
                    TargetedRuntimeVersion = project.GetPropertyValue("TargetedRuntimeVersion"),
                    TargetFrameworkMonikerDisplayName = project.GetPropertyValue("TargetFrameworkMonikerDisplayName"),
                    AppConfigFile = appConfigFile,
                    AllowedAssemblyExtensions = allowedAssemblyExtensions,
                    AutoUnify = project.GetPropertyValue("AutoUnifyAssemblyReferences").IsTrue()
                };

                var result = resolveTask.Execute();
                if (result)
                {
                    logger.Info("ResolveAssemblyReference task was successfully done");
                }
                else
                {
                    logger.Error("ResolveAssemblyReference task was failed");
                }
                //ProjectCollection.GlobalProjectCollection.UnloadProject(project);
                return resolveTask.ResolvedFiles;
            }
        }

        private static string GetAppConfigFile(Project project)
        {
            // TODO: it wouldn't work for a .netcore|.netstandard
            var primaryList = new List<TaskItem>(64);
            foreach (var projectItem in project.GetItems("None"))
            {
                var metadata = new Dictionary<string, string>(32);
                foreach (var item in projectItem.Metadata)
                {
                    metadata.Add(item.Name, item.EvaluatedValue);
                }
                primaryList.Add(new TaskItem(projectItem.EvaluatedInclude.GetFullPath(project.DirectoryPath), metadata));
            }

            var secondaryList = new List<TaskItem>(64);
            foreach (var projectItem in project.GetItems("Content"))
            {
                var metadata = new Dictionary<string, string>(32);
                foreach (var item in projectItem.Metadata)
                {
                    metadata.Add(item.Name, item.EvaluatedValue);
                }
                secondaryList.Add(new TaskItem(projectItem.EvaluatedInclude.GetFullPath(project.DirectoryPath), metadata));
            }

            using (var logger = LogManager.GetLogger("FindAppConfig"))
            {
                logger.Info("Executing FindAppConfigFile task......");
                var findTask = new FindAppConfigFile
                {
                    BuildEngine = new MsBuildEngine(logger),
                    PrimaryList = primaryList.ToArray(),
                    SecondaryList = secondaryList.ToArray(),
                    TargetPath = project.GetPropertyValue("TargetPath") + ".config"
                };

                var result = findTask.Execute();
                if (result)
                {
                    logger.Info("FindAppConfigFile task was successfully done");
                }
                else
                {
                    logger.Error("FindAppConfigFile task was failed");
                }
                return findTask.AppConfigFile?.ItemSpec;
            }
        }

        private static string[] GetFrameworkDirectories(Project project)
        {
            var frameworkDirectories = new List<string>(8)
            {
               project.GetPropertyValue("FrameworkPathOverride")
            };
            // NOTE: directly add path to a "Facades".
            if (project.GetPropertyValue("ImplicitlyExpandDesignTimeFacades").IsTrue())
            {
                frameworkDirectories.Add(Path.Combine(frameworkDirectories[0], "Facades"));
            }
            return frameworkDirectories.ToArray();
        }

        private static string[] GetSearchPaths(Project project)
        {
            // NOTE: look at the search paths order here:
            // https://github.com/Microsoft/msbuild/blob/master/src/Tasks/AssemblyDependency/ResolveAssemblyReference.cs#L451
            // and here: https://docs.microsoft.com/en-us/visualstudio/msbuild/resolveassemblyreference-task#parameters

            var searchPaths = new List<string>(16);
            var searhPathsProperty = project.GetPropertyValue("AssemblySearchPaths");
            foreach (var item in searhPathsProperty.Split(searchDelimeters, StringSplitOptions.RemoveEmptyEntries))
            {
                var searchPath = item.Trim();
                if (!string.IsNullOrWhiteSpace(searchPath))
                {
                    // NOTE: skip the special search path items by pattern {.+}
                    if (!searchPath.StartsWith("{", StringComparison.Ordinal) || !searchPath.EndsWith("}", StringComparison.Ordinal))
                    {
                        searchPath = searchPath.GetFullPath(project.DirectoryPath);
                    }
                    searchPaths.Add(searchPath);
                }
            }
            return searchPaths.ToArray();
        }

        private static TaskItem[] GetReferences(Project project)
        {
            var references = new List<TaskItem>(64);
            foreach (var reference in project.GetItems("Reference"))
            {
                var metadata = new Dictionary<string, string>(32);
                foreach (var item in reference.Metadata)
                {
                    var value = item.Name.EqualsNoCase("HintPath")
                        ? item.EvaluatedValue.GetFullPath(project.DirectoryPath)
                        : item.EvaluatedValue;
                    metadata.Add(item.Name, value);
                }
                references.Add(new TaskItem(reference.EvaluatedInclude, metadata));
            }

            // NOTE: append references to standard assemblies
            // TODO: it would not be work for .netcore & .netstandard
            references.Add(new TaskItem("mscorlib"));
            references.Add(new TaskItem("System"));
            references.Add(new TaskItem("System.Core"));
            if (project.FullPath.EndsWith(".vbproj"))
            {
                references.Add(new TaskItem("Microsoft.VisualBasic"));
            }

            return references.ToArray();
        }

        private static ImmutableArray<ProjectInfo> GetProjectReferences(Project project)
        {
            var referencesBuilder = ImmutableArray.CreateBuilder<ProjectInfo>(32);
            foreach (var reference in project.GetItems("ProjectReference"))
            {
                referencesBuilder.Add(GetProject(reference.EvaluatedInclude.GetFullPath(project.DirectoryPath)));
            }
            return referencesBuilder.TryMoveToImmutable();
        }

        private static TaskItem[] GetAssemblyFiles(Project project)
        {
            var references = new List<TaskItem>(32);
            foreach (var reference in project.GetItems("_ExplicitReference"))
            {
                var metadata = new Dictionary<string, string>(32);
                foreach (var item in reference.Metadata)
                {
                    metadata.Add(item.Name, item.EvaluatedValue);
                }

                references.Add(new TaskItem(reference.EvaluatedInclude, metadata));
            }
            return references.ToArray();
        }

        private static ImmutableArray<string> GetCompileItems(Project project)
        {
            var compilesBuilder = ImmutableArray.CreateBuilder<string>(256);
            foreach (var compile in project.GetItems("Compile"))
            {
                compilesBuilder.Add(compile.EvaluatedInclude.GetFullPath(project.DirectoryPath));
            }
            return compilesBuilder.TryMoveToImmutable();
        }

        private static ImmutableArray<string> GetImports(Project project)
        {
            var builder = ImmutableArray.CreateBuilder<string>();
            foreach (var item in project.GetItems("Import"))
            {
                builder.Add(item.EvaluatedInclude);
            }
            return builder.TryMoveToImmutable();
        }

        private static bool IsOn(this string value) => value.EqualsNoCase("on");

        private static ImmutableArray<T> TryMoveToImmutable<T>(this ImmutableArray<T>.Builder builder) =>
            builder.Count == builder.Capacity ? builder.MoveToImmutable() : builder.ToImmutable();
    }
}