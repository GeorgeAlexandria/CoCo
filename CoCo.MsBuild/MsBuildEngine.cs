using System.Collections;
using System.Collections.Generic;
using System.Text;
using CoCo.Logging;
using CoCo.Utils;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;

namespace CoCo.MsBuild
{
    internal sealed class MsBuildEngine : IBuildEngine5
    {
        private readonly Logger _logger;

        private readonly Dictionary<object, object> _objectCacheLivingUntilBuild = new Dictionary<object, object>();

        private readonly Dictionary<object, object> _objectCacheLivingInAppDomain = new Dictionary<object, object>();

        public MsBuildEngine(Logger logger)
        {
            _logger = logger;
        }

        public bool ContinueOnError => false;

        public int LineNumberOfTaskNode => 0;

        public int ColumnNumberOfTaskNode => 0;

        public string ProjectFileOfTaskNode => "";

        public bool IsRunningMultipleNodes => false;

        public bool BuildProjectFile(
            string projectFileName,
            string[] targetNames,
            IDictionary globalProperties,
            IDictionary targetOutputs) =>
            BuildProjectFile(projectFileName, targetNames, globalProperties, targetOutputs, null);

        public bool BuildProjectFile(
            string projectFileName,
            string[] targetNames,
            IDictionary globalProperties,
            IDictionary targetOutputs,
            string toolsVersion) =>
            BuildProjectFilesInParallel(
                new[] { projectFileName },
                targetNames,
                new[] { globalProperties },
                new[] { targetOutputs },
                new[] { toolsVersion },
                true, true);

        public bool BuildProjectFilesInParallel(
            string[] projectFileNames,
            string[] targetNames,
            IDictionary[] globalProperties,
            IDictionary[] targetOutputsPerProject,
            string[] toolsVersion,
            bool useResultsCache,
            bool unloadProjectsOnCompletion)
        {
            var includeTargetOutputs = targetOutputsPerProject != null;

            var result = BuildProjectFilesInParallel(
                projectFileNames,
                targetNames,
                globalProperties,
                new List<string>[projectFileNames.Length],
                toolsVersion,
                includeTargetOutputs);

            if (includeTargetOutputs)
            {
                var actualTargetOutputsPerProject = result.TargetOutputsPerProject;
                for (int i = 0; i < actualTargetOutputsPerProject.Count; ++i)
                {
                    if (targetOutputsPerProject[i] == null)
                    {
                        targetOutputsPerProject[i] = new Dictionary<string, ITaskItem[]>(actualTargetOutputsPerProject[i].Count);
                    }

                    foreach (var item in actualTargetOutputsPerProject[i])
                    {
                        targetOutputsPerProject[i].Add(item.Key, item.Value);
                    }
                }
            }

            return result.Result;
        }

        public BuildEngineResult BuildProjectFilesInParallel(
            string[] projectFileNames,
            string[] targetNames,
            IDictionary[] globalProperties,
            IList<string>[] removeGlobalProperties,
            string[] toolsVersion,
            bool returnTargetOutputs)
        {
            // TODO: constraints
            List<IDictionary<string, ITaskItem[]>> targetOutputsPerProject = null;
            var allSuccess = true;

            if (returnTargetOutputs) targetOutputsPerProject = new List<IDictionary<string, ITaskItem[]>>(projectFileNames.Length);

            for (int i = 0; i < projectFileNames.Length; ++i)
            {
                var projectGlobalProperties = globalProperties[i];
                var properties = new Dictionary<string, string>(projectGlobalProperties.Count);
                if (projectGlobalProperties != null)
                {
                    var removeProjectProperties = removeGlobalProperties[i];
                    foreach (DictionaryEntry item in projectGlobalProperties)
                    {
                        if (item.Key is string key && (removeGlobalProperties == null || !removeProjectProperties.Contains(key)))
                        {
                            properties.Add(key, item.Value as string);
                        }
                    }
                }

                string existingToolsVersion = null;
                foreach (var item in toolsVersion)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        existingToolsVersion = item;
                        break;
                    }
                }

                var project = new Project(projectFileNames[i], properties, existingToolsVersion);
                var result = project.CreateProjectInstance().Build(targetNames, new ILogger[0], out var targetOuputs);
                if (!result) _logger.Error("{0} project was failed to build", projectFileNames[i]);
                allSuccess &= result;

                if (returnTargetOutputs)
                {
                    var projectTargetOutputs = new Dictionary<string, ITaskItem[]>(targetOuputs.Count);
                    foreach (var item in targetOuputs)
                    {
                        projectTargetOutputs.Add(item.Key, item.Value.Items);
                    }

                    targetOutputsPerProject.Add(projectTargetOutputs);
                }
            }

            // TODO: does it need?
            _objectCacheLivingUntilBuild.Clear();
            return new BuildEngineResult(allSuccess, targetOutputsPerProject);
        }

        public void LogCustomEvent(CustomBuildEventArgs e) => _logger.Debug(e.Message);

        public void LogErrorEvent(BuildErrorEventArgs e) => _logger.Error(e.Message);

        public void LogMessageEvent(BuildMessageEventArgs e) => _logger.Info(e.Message);

        public void LogWarningEvent(BuildWarningEventArgs e) => _logger.Warn(e.Message);

        public void LogTelemetry(string eventName, IDictionary<string, string> properties)
        {
            var builder = new StringBuilder(eventName).AppendLine();
            foreach (var item in properties)
            {
                builder.Append("Property ").Append(item.Key).Append(" = ").AppendLine(item.Value);
            }

            _logger.Info(builder.ToString());
        }

        public void Reacquire()
        {
        }

        public void Yield()
        {
        }

        public void RegisterTaskObject(object key, object obj, RegisteredTaskObjectLifetime lifetime, bool allowEarlyCollection)
        {
            var cache = GetCache(lifetime);
            if (cache.TryGetValue(key, out var value))
            {
                cache[key] = obj;
            }
            else
            {
                cache.Add(key, obj);
            }
        }

        public object GetRegisteredTaskObject(object key, RegisteredTaskObjectLifetime lifetime) =>
            GetCache(lifetime).TryGetValue(key, out var value) ? value : null;

        public object UnregisterTaskObject(object key, RegisteredTaskObjectLifetime lifetime) =>
            GetCache(lifetime).TryRemoveValue(key, out var value) ? value : null;

        private IDictionary<object, object> GetCache(RegisteredTaskObjectLifetime lifetime) =>
            lifetime == RegisteredTaskObjectLifetime.Build ? _objectCacheLivingUntilBuild : _objectCacheLivingInAppDomain;
    }
}