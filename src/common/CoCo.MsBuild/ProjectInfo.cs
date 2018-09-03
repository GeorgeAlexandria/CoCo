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
            ImmutableArray<string> assemblyReferences,
            ImmutableArray<ProjectInfo> projectReferences,
            ImmutableArray<string> compileItems,
            ImmutableArray<string> imports,
            string rootNamespace)
        {
            AssemblyReferences = assemblyReferences;
            ProjectReferences = projectReferences;
            CompileItems = compileItems;
            ProjectPath = projectPath;
            ProjectName = Path.GetFileNameWithoutExtension(projectPath);
            Imports = imports;
            RootNamespace = rootNamespace;
        }

        public ImmutableArray<string> AssemblyReferences { get; }

        public ImmutableArray<ProjectInfo> ProjectReferences { get; }

        public ImmutableArray<string> CompileItems { get; }

        public ImmutableArray<string> Imports { get; }

        public string ProjectPath { get; }

        public string ProjectName { get; }

        public string RootNamespace { get; }
    }
}