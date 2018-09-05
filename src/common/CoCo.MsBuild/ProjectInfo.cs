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
            string rootNamespace,
            bool optionCompare,
            bool optionExplicit,
            bool optionInfer,
            bool optionStrict)
        {
            AssemblyReferences = assemblyReferences;
            ProjectReferences = projectReferences;
            CompileItems = compileItems;
            ProjectPath = projectPath;
            ProjectName = Path.GetFileNameWithoutExtension(projectPath);
            Imports = imports;
            RootNamespace = rootNamespace;
            OptionCompare = optionCompare;
            OptionExplicit = optionExplicit;
            OptionInfer = optionInfer;
            OptionStrict = optionStrict;
        }

        public ImmutableArray<string> AssemblyReferences { get; }

        public ImmutableArray<ProjectInfo> ProjectReferences { get; }

        public ImmutableArray<string> CompileItems { get; }

        public ImmutableArray<string> Imports { get; }

        public string ProjectPath { get; }

        public string ProjectName { get; }

        public string RootNamespace { get; }

        /// <summary>
        /// It's true when compare set to "Text"
        /// </summary>
        public bool OptionCompare { get; }

        /// <summary>
        /// It's true when explicit is enabled
        /// </summary>
        public bool OptionExplicit { get; }

        /// <summary>
        /// It's true when infer is enabled
        /// </summary>
        public bool OptionInfer { get; }

        /// <summary>
        /// It's true when strict is enabled
        /// </summary>
        public bool OptionStrict { get; }
    }
}