using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;

namespace CoCo.MsBuild
{
    [DebuggerDisplay("{ProjectName}")]
    public class ProjectInfo
    {
        internal ProjectInfo(
            string projectPath,
            ICollection<string> assemblyReferences,
            ICollection<ProjectInfo> projectReferences,
            ICollection<string> compileItems)
        {
            var builder = ImmutableArray.CreateBuilder<string>(assemblyReferences.Count);
            foreach (var item in assemblyReferences)
            {
                builder.Add(item);
            }
            AssemblyReferences = builder.MoveToImmutable();

            var builderProjects = ImmutableArray.CreateBuilder<ProjectInfo>(projectReferences.Count);
            foreach (var item in projectReferences)
            {
                builderProjects.Add(item);
            }
            ProjectReferences = builderProjects.MoveToImmutable();

            var builderCompile = ImmutableArray.CreateBuilder<string>(compileItems.Count);
            foreach (var item in compileItems)
            {
                builderCompile.Add(item);
            }

            CompileItems = builderCompile.MoveToImmutable();
            ProjectPath = projectPath;
            ProjectName = Path.GetFileNameWithoutExtension(projectPath);
        }

        public ImmutableArray<string> AssemblyReferences { get; }

        public ImmutableArray<ProjectInfo> ProjectReferences { get; }

        public ImmutableArray<string> CompileItems { get; }

        public string ProjectPath { get; }

        public string ProjectName { get; }
    }
}