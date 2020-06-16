namespace CoCo.Test.Identifiers.Common
{
    internal abstract class CompilationUnit
    {
        public ProgrammingLanguage Language { get; }

        protected CompilationUnit(ProgrammingLanguage language)
        {
            Language = language;
        }

        public abstract bool TryGetSourceCode(string compileItemPath, out string code);
    }
}