using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;
using CoCoLog;

namespace CoCoTests
{
    // NOTE: describe how to retrieve input arguments
    // http://source.roslyn.io/#MSBuildFiles/C/ProgramFiles(x86)/MSBuild/14.0/bin_/amd64/Microsoft.Common.CurrentVersion.targets,1820
    // https://github.com/Microsoft/msbuild/wiki/ResolveAssemblyReference
    // TODO: fix arguments in tasks
    internal static class MsBuild
    {
        private static string[] searchDelimeters = { Environment.NewLine, ";" };

        private static string[] allowedAssemblyExtensions = { ".dll" };

        /// TODO: should return not a references, but <see cref="ProjectInfo"/>
        public static List<string> ResolveAssemblyReferences(string projectPath)
        {
            var project = new Project(projectPath);

            var searchPaths = GetSearchPaths(project);
            var frameworkDirectories = GetFrameworkDirectories(project);
            var references = GetReferences(project);
            var explicitReferences = GetExplicitReference(project);
            var appConfigFile = GetAppConfigFile(project);

            using (var logger = LogManager.GetLogger("ResolveReference"))
            {
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
                    AutoUnify = "true".Equals(project.GetPropertyValue("AutoUnifyAssemblyReferences"), StringComparison.OrdinalIgnoreCase)
                };

                var result = resolveTask.Execute();
                if (!result) logger.Error("Resolve reference task was failed");
                var referencesAssemblies = new List<string>(resolveTask.ResolvedFiles.Length);
                foreach (var item in resolveTask.ResolvedFiles)
                {
                    referencesAssemblies.Add(item.ItemSpec);
                }

                ProjectCollection.GlobalProjectCollection.UnloadProject(project);
                return referencesAssemblies;
            }
        }

        private static string GetAppConfigFile(Project project)
        {
            // TODO: it wouldn't work for a .netcore|.netstandard
            var primaryList = new List<TaskItem>(64);
            foreach (var projectItem in project.GetItems("None"))
            {
                var metadata = new Dictionary<string, string>(64);
                foreach (var item in projectItem.Metadata)
                {
                    metadata.Add(item.Name, item.EvaluatedValue);
                }
                primaryList.Add(new TaskItem(projectItem.EvaluatedInclude.GetFullPath(project.DirectoryPath), metadata));
            }

            var secondaryList = new List<TaskItem>(64);
            foreach (var projectItem in project.GetItems("Content"))
            {
                var metadata = new Dictionary<string, string>(64);
                foreach (var item in projectItem.Metadata)
                {
                    metadata.Add(item.Name, item.EvaluatedValue);
                }
                secondaryList.Add(new TaskItem(projectItem.EvaluatedInclude.GetFullPath(project.DirectoryPath), metadata));
            }

            using (var logger = LogManager.GetLogger("FindAppConfig"))
            {
                var findTask = new FindAppConfigFile
                {
                    BuildEngine = new MsBuildEngine(logger),
                    PrimaryList = primaryList.ToArray(),
                    SecondaryList = secondaryList.ToArray(),
                    // NOTE: assume that the appconfig would be at this value
                    TargetPath = project.GetPropertyValue("TargetPath") + ".config"
                };

                var result = findTask.Execute();
                if (!result) logger.Error("Find appconfig task was failed");
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

        private static TaskItem[] GetExplicitReference(Project project)
        {
            var references = new List<TaskItem>(32);
            foreach (var reference in project.GetItems("_ExplicitReference"))
            {
                var metadata = new Dictionary<object, string>(64);
                foreach (var item in reference.Metadata)
                {
                    metadata.Add(item.Name, item.EvaluatedValue);
                }

                references.Add(new TaskItem(reference.EvaluatedInclude, metadata));
            }
            return references.ToArray();
        }
    }
}