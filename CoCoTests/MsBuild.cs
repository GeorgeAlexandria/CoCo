using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

namespace CoCoTests
{
    internal static class MsBuild
    {
        private static string[] searchDelimeters = { Environment.NewLine, ";" };

        private static string[] allowedAssemblyExtensions = { ".dll" };

        public static List<string> ResolveAssemblyReferences(string projectPath)
        {
            var project = new Project(projectPath);

            var searchPaths = GetSearchPaths(project);
            var frameworkDirectories = GetFrameworkDirectories(project);
            var references = GetReferences(project);
            var appConfigFile = GetAppConfigFile(project);

            // TODO: add reference to mscorlib
            var resolveTask = new ResolveAssemblyReference
            {
                BuildEngine = new MsBuildEngine(),
                Assemblies = references,
                TargetFrameworkVersion = project.GetPropertyValue("TargetFrameworkVersion"),
                TargetFrameworkMoniker = project.GetPropertyValue("TargetFrameworkMoniker"),
                SearchPaths = searchPaths,
                TargetFrameworkDirectories = frameworkDirectories,
                TargetedRuntimeVersion = project.GetPropertyValue("TargetedRuntimeVersion"),
                TargetFrameworkMonikerDisplayName = project.GetPropertyValue("TargetFrameworkMonikerDisplayName"),
                AppConfigFile = appConfigFile,
                AllowedAssemblyExtensions = allowedAssemblyExtensions
            };

            // TODO: Log if it failed
            resolveTask.Execute();
            var referencesAssemblies = new List<string>(resolveTask.ResolvedFiles.Length);
            foreach (var item in resolveTask.ResolvedFiles)
            {
                referencesAssemblies.Add(item.ItemSpec);
            }
            return referencesAssemblies;
        }

        private static string GetAppConfigFile(Project project)
        {
            // TODO: it wouldn't work for a .netcore|.netstandard
            var primaryList = new List<TaskItem>(64);
            foreach (var projectItem in project.Items.Where(x => x.GetMetadataValue("SubType") == "Designer"))
            {
                var metadata = new Dictionary<string, string>(64);
                foreach (var item in projectItem.Metadata)
                {
                    metadata.Add(item.Name, item.EvaluatedValue);
                }
                primaryList.Add(new TaskItem(projectItem.EvaluatedInclude.GetFullPath(project.DirectoryPath), metadata));
            }

            var findTask = new FindAppConfigFile
            {
                BuildEngine = new MsBuildEngine(),
                PrimaryList = primaryList.ToArray(),
                SecondaryList = new TaskItem[0],
                // NOTE: assume that the appconfig would be at this value
                TargetPath = project.GetPropertyValue("_GenerateBindingRedirectsIntermediateAppConfig").GetFullPath(project.DirectoryPath)
            };

            return findTask.Execute() ? findTask.AppConfigFile.ItemSpec : string.Empty;
        }

        private static string[] GetFrameworkDirectories(Project project)
        {
            var frameworkDirectories = new List<string>(8)
            {
               project.GetPropertyValue("FrameworkPathOverride")
            };
            // NOTE: directly add path to a "Facades".
            // TODO: Exists the other way to do that?
            if (string.Equals("true", project.GetPropertyValue("ImplicitlyExpandDesignTimeFacades"), StringComparison.OrdinalIgnoreCase))
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
                var metadata = new Dictionary<object, string>(64);
                foreach (var item in reference.Metadata)
                {
                    var value = string.Equals(item.Name, "HintPath", StringComparison.OrdinalIgnoreCase)
                        ? item.EvaluatedValue.GetFullPath(project.DirectoryPath)
                        : item.EvaluatedValue;
                    metadata.Add(item.Name, value);
                }
                references.Add(new TaskItem(reference.EvaluatedInclude, metadata));
            }
            return references.ToArray();
        }
    }
}