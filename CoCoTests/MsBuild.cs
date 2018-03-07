using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoCoLog;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

namespace CoCoTests
{
    // NOTE: describe how to retrieve input arguments
    // http://source.roslyn.io/#MSBuildFiles/C/ProgramFiles(x86)/MSBuild/14.0/bin_/amd64/Microsoft.Common.CurrentVersion.targets,1820
    // https://github.com/Microsoft/msbuild/wiki/ResolveAssemblyReference
    // TODO: fix arguments in tasks
    internal static class MsBuild
    {
        /// TODO: use <see cref="WeakReference{T}"/> when <see cref="ProjectInfo"/> would be take a lot of space
        private static Dictionary<string, ProjectInfo> _cache = new Dictionary<string, ProjectInfo>();

        private static string[] searchDelimeters = { Environment.NewLine, ";" };

        private static string[] allowedAssemblyExtensions = { ".dll" };

        public static ProjectInfo CreateProject(string projectPath)
        {
            ProjectInfo projectInfo = null;
            if (!_cache.TryGetValue(projectPath, out var reference))
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
            var references = new List<string>(assemblyReferences.Length);
            foreach (var item in assemblyReferences)
            {
                references.Add(item.ItemSpec);
            }

            var projectRefereneces = GetProjectReferences(project);
            var projects = new List<ProjectInfo>(projectRefereneces.Count);
            foreach (var item in projectRefereneces)
            {
                var projectInfo = CreateProject(item.ItemSpec);
                projects.Add(projectInfo);
            }

            return new ProjectInfo(references, projects);
        }

        private static ITaskItem[] ResolveAssemblyReferences(Project project)
        {
            var searchPaths = GetSearchPaths(project);
            var frameworkDirectories = GetFrameworkDirectories(project);
            var references = GetReferences(project);
            var explicitReferences = GetAssemblyFiles(project);
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
                    AutoUnify = project.GetPropertyValue("AutoUnifyAssemblyReferences").IsTrue()
                };

                var result = resolveTask.Execute();
                if (!result) logger.Error("Resolve reference task was failed");
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

        private static List<ITaskItem> GetProjectReferences(Project project)
        {
            var references = new List<ITaskItem>(32);
            foreach (var reference in project.GetItems("ProjectReference"))
            {
                var metadata = new Dictionary<object, string>(64);
                foreach (var item in reference.Metadata)
                {
                    metadata.Add(item.Name, item.EvaluatedValue);
                }

                references.Add(new TaskItem(reference.EvaluatedInclude.GetFullPath(project.DirectoryPath), metadata));
            }
            return references;
        }

        private static TaskItem[] GetAssemblyFiles(Project project)
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