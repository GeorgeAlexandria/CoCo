using CoCo.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;

namespace CoCo.Test.Common
{
    internal class RoslynCompilationUnit : CompilationUnit
    {
        public RoslynCompilationUnit(Compilation compilation, ProgrammingLanguage language) : base(language)
        {
            Compilation = compilation;
        }

        public Compilation Compilation { get; }

        public override bool TryGetSourceCode(string compileItemPath, out string code)
        {
            foreach (var syntaxTree in Compilation.SyntaxTrees)
            {
                if (syntaxTree.FilePath.EqualsNoCase(compileItemPath))
                {
                    code = syntaxTree.ToString();
                    return true;
                }
            }
            code = default;
            return false;
        }

        public static implicit operator RoslynCompilationUnit(CSharpCompilation compilation) =>
            new RoslynCompilationUnit(compilation, ProgrammingLanguage.CSharp);

        public static implicit operator RoslynCompilationUnit(VisualBasicCompilation compilation) =>
            new RoslynCompilationUnit(compilation, ProgrammingLanguage.VisualBasic);
    }
}