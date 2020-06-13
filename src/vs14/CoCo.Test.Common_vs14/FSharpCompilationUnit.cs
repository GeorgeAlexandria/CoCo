using CoCo.Utils;
using Microsoft.FSharp.Compiler.SourceCodeServices;

namespace CoCo.Test.Common
{
    internal class FSharpCompilationUnit : CompilationUnit
    {
        public FSharpCompilationUnit(FSharpProjectOptions options) : base(ProgrammingLanguage.FSharp)
        {
            Options = options;
        }

        public FSharpProjectOptions Options { get; }

        public override bool TryGetSourceCode(string compileItemPath, out string code)
        {
            foreach (var item in Options.ProjectFileNames)
            {
                if (item.EqualsNoCase(compileItemPath))
                {
                    code = System.IO.File.ReadAllText(compileItemPath);
                    return true;
                }
            }

            code = default;
            return false;
        }

        public static implicit operator FSharpCompilationUnit(FSharpProjectOptions FSharpCompilationUnit) =>
            new FSharpCompilationUnit(FSharpCompilationUnit);
    }
}