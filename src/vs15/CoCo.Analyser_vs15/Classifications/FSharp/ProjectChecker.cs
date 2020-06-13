using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoCo.Utils;
using FSharp.Compiler;
using FSharp.Compiler.SourceCodeServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;

namespace CoCo.Analyser.Classifications.FSharp
{
    internal class ProjectChecker : IProjectChecker
    {
        private enum SourceState
        {
            None,
            IsListening,
            Pending,
            Checked
        }

        private sealed class ProjectCheckInfo
        {
            private readonly FSharpChecker _checker;
            private readonly ReaderWriterLockSlim _checkLock;

            private HashSet<string> _checkedSources;
            private HashSet<string> _pendingSources;
            private Dictionary<string, HashSet<IListener>> _listeners;

            public ProjectCheckInfo()
            {
                // TODO: would be better to use a custom ReferenceResolver implementaion?
                _checker = FSharpChecker.Create(null, true, false, null, null);
                _checker.ProjectChecked.AddHandler(OnProjectChecked);
                _checker.FileChecked.AddHandler(OnSourceChecked);

                _checkLock = new ReaderWriterLockSlim();
            }

            public FSharpAsync<Tuple<FSharpParseFileResults, FSharpCheckFileAnswer>> ParseAndCheckFileInProject(
              FSharpProjectOptions projectOptions, string itemPath, VersionStamp itemVersion, SourceText itemContent)
            {
                return _checker.ParseAndCheckFileInProject(itemPath, itemVersion.GetHashCode(), itemContent.ToString(),
                    projectOptions, null, "CoCo_Classifications");
            }

            public bool TryGetRecentResults(FSharpProjectOptions projectOptions, string itemPath, VersionStamp itemVersion,
                SourceText itemContent, out (FSharpParseFileResults, FSharpCheckFileResults) result)
            {
                var recentResult = _checker.TryGetRecentCheckResultsForFile(itemPath, projectOptions, itemContent.ToString(),
                    "CoCo_GetRecent");
                if (recentResult.IsSome() && recentResult.Value.Item3 == itemVersion.GetHashCode())
                {
                    result = (recentResult.Value.Item1, recentResult.Value.Item2);
                    return true;
                }

                result = default;
                return false;
            }

            public SourceState GetSourceState(string itemPath)
            {
                _checkLock.EnterReadLock();

                var state =
                    _listeners.IsNotNull() && _listeners.ContainsKey(itemPath) ? SourceState.IsListening :
                    _pendingSources.IsNotNull() && _pendingSources.Contains(itemPath) ? SourceState.Pending :
                    _checkedSources.IsNotNull() && _checkedSources.Contains(itemPath) ? SourceState.Checked :
                    SourceState.None;

                _checkLock.ExitReadLock();
                return state;
            }

            public void Listen(string itemPath, IListener listener, IEnumerable<string> pendingSources = null)
            {
                _checkLock.EnterReadLock();

                _listeners ??= new Dictionary<string, HashSet<IListener>>();
                if (!_listeners.TryGetValue(itemPath, out var listeners))
                {
                    listeners = new HashSet<IListener>();
                    _listeners[itemPath] = listeners;
                }
                listeners.Add(listener);

                _pendingSources?.Remove(itemPath);
                if (pendingSources.IsNotNull())
                {
                    _pendingSources ??= new HashSet<string>();
                    foreach (var item in pendingSources)
                    {
                        if (!_listeners.ContainsKey(item))
                        {
                            _pendingSources.Add(item);
                        }
                    }
                }

                _checkLock.ExitReadLock();
            }

            private void OnSourceChecked(object sender, Tuple<string, FSharpOption<object>> args)
            {
                _checkLock.EnterWriteLock();

                _checkedSources ??= new HashSet<string>();
                _pendingSources?.Remove(args.Item1);
                if (_listeners.IsNotNull() && _listeners.TryRemove(args.Item1, out var listeners))
                {
                    foreach (var listener in listeners)
                    {
                        listener.Invoke();
                    }
                }

                _checkLock.ExitWriteLock();
            }

            private void OnProjectChecked(object sender, Tuple<string, FSharpOption<object>> args)
            {
                _checkLock.EnterWriteLock();

                _pendingSources = null;
                if (_listeners.IsNotNull())
                {
                    var temp = _listeners;
                    _listeners = null;
                    foreach (var listeners in temp.Values)
                    {
                        foreach (var listener in listeners)
                        {
                            listener.Invoke();
                        }
                    }
                }

                _checkLock.ExitWriteLock();
            }
        }

        private ProjectChecker()
        {
        }

        public static ProjectChecker Instance { get; } = new ProjectChecker();

        private ProjectCheckInfo info;

        public FSharpProjectOptions GetOptions(Workspace workspace, Document document)
        {
            if (string.IsNullOrWhiteSpace(document.Project.FilePath))
            {
                var checker = FSharpChecker.Create(null, true, false, null, null);
                var task = checker.GetProjectOptionsFromScript(document.FilePath, document.GetTextAsync().Result.ToString(),
                    null, null, null, null, null, null, "CoCo_script_options");
                return FSharpAsync.RunSynchronously(task, null, null).Item1;
            }

            return GetOptions(workspace, document.Project);
        }

        public ParseCheckResult ParseAndCheckFileInProject(IListener listener, FSharpProjectOptions projectOptions, string itemPath,
            SourceText itemContent, VersionStamp itemVersion)
        {
            // TODO: check must be splited by project path, by target framework and configuration

            // NOTE:
            // if source was pending or listening => start to listen it
            // Try to get previous result by content hash or try to get check result by timeout, and if timeout will end => start to listen source

            var state = info.GetSourceState(itemPath);
            // NOTE: one source may have a several different listeners (a several instance of window items),
            // so if source currently is listened then add current listener
            if (state == SourceState.Pending || state == SourceState.IsListening)
            {
                info.Listen(itemPath, listener);
            }
            else
            {
                if (info.TryGetRecentResults(projectOptions, itemPath, itemVersion, itemContent, out var resent))
                {
                    return new ParseCheckResult(resent.Item1, resent.Item2);
                }

                var pendingSources = state == SourceState.None ? GetSources(projectOptions) : null;
                info.Listen(itemPath, listener, pendingSources);

                var result = info.ParseAndCheckFileInProject(projectOptions, itemPath, itemVersion, itemContent);

                // NOTE: assume that 150 miliseconds will be not so long to wait a response from the checker
                var timeoutTask = Task.Delay(150);

                // NOTE: use delay instead of cancellation token because checker retrieves state of token not so often as is wanted
                var completedTask = Task.WhenAny(FSharpAsync.StartAsTask(result, null, null), timeoutTask).Result;
                if (completedTask != timeoutTask)
                {
                    var (parseResult, checkAnswer) = (completedTask as Task<Tuple<FSharpParseFileResults, FSharpCheckFileAnswer>>)
                        .Result.ToValueTuple();
                    if (checkAnswer.IsSucceeded && checkAnswer is FSharpCheckFileAnswer.Succeeded succeeded)
                    {
                        var checkResult = succeeded.Item;
                        return new ParseCheckResult(parseResult, checkResult);
                    }
                }
            }

            return default;
        }

        private FSharpProjectOptions GetOptions(Workspace workspace, Project project)
        {
            var referencedProjectsOptions = new List<Tuple<string, FSharpProjectOptions>>();
            foreach (var item in project.ProjectReferences)
            {
                var referencedProject = project.Solution.GetProject(item.ProjectId);
                if (string.Equals(project.Language, "F#"))
                {
                    var projectOptions = GetOptions(workspace, referencedProject);
                    referencedProjectsOptions.Add((referencedProject.OutputFilePath, projectOptions).ToTuple());
                }
            }

            var options = new List<string>();
            foreach (var item in new FscOptionsBuilder(project.FilePath).Build())
            {
                if (!item.StartsWith("-r:"))
                {
                    options.Add(item);
                }
            }
            foreach (var item in project.ProjectReferences)
            {
                options.Add("-r:" + project.Solution.GetProject(item.ProjectId).OutputFilePath);
            }
            foreach (var item in project.MetadataReferences.OfType<PortableExecutableReference>())
            {
                options.Add("-r:" + item.FilePath);
            }

            return new FSharpProjectOptions(
                project.FilePath,
                project.Id.Id.ToString("D").ToLowerInvariant(),
                project.Documents.Select(x => x.FilePath).ToArray(),
                options.ToArray(),
                referencedProjectsOptions.ToArray(),
                false,
                SourceFile.MustBeSingleFileProject(Path.GetFileName(project.FilePath)),
                DateTime.Now,
                null,
                FSharpList<Tuple<Range.range, string>>.Empty,
                null,
                FSharpOption<long>.Some(project.Version.GetHashCode()));
        }

        private HashSet<string> GetSources(FSharpProjectOptions projectOptions)
        {
            var set = new HashSet<string>();
            void Collect(FSharpProjectOptions options)
            {
                foreach (var item in options.SourceFiles)
                {
                    if (item.EndsWith(".fs") || item.EndsWith(".fsi"))
                    {
                        set.Add(item);
                    }
                }
                foreach (var item in options.ReferencedProjects)
                {
                    Collect(item.Item2);
                }
            }

            Collect(projectOptions);
            return set;
        }
    }
}