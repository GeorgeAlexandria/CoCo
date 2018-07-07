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
            ICollection<string> references,
            ICollection<ProjectInfo> projectReferences,
            ICollection<string> compileItems)
        {
            var builder = ImmutableArray.CreateBuilder<string>(references.Count);
            foreach (var item in references)
            {
                builder.Add(item);
            }
            References = builder.MoveToImmutable();

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

        public ImmutableArray<string> References { get; }

        public ImmutableArray<ProjectInfo> ProjectReferences { get; }

        public ImmutableArray<string> CompileItems { get; }

        public string ProjectPath { get; }

        public string ProjectName { get; }
    }
}