using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;

namespace CoCo.Test.Common
{
    public class CompilationUnit
    {
        public CompilationUnit(Compilation compilation, ProgrammingLanguage language)
        {
            Language = language;
            Compilation = compilation;
        }

        public Compilation Compilation { get; }

        public ProgrammingLanguage Language { get; }

        public static implicit operator CompilationUnit(CSharpCompilation compilation) =>
            new CompilationUnit(compilation, ProgrammingLanguage.CSharp);

        public static implicit operator CompilationUnit(VisualBasicCompilation compilation) =>
            new CompilationUnit(compilation, ProgrammingLanguage.VisualBasic);
    }
}