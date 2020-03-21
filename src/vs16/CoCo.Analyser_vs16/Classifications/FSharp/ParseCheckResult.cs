using FSharp.Compiler.SourceCodeServices;

namespace CoCo.Analyser.Classifications.FSharp
{
    public readonly ref struct ParseCheckResult
    {
        public readonly FSharpParseFileResults ParseResult;
        public readonly FSharpCheckFileResults CheckResult;

        public ParseCheckResult(FSharpParseFileResults parseResult, FSharpCheckFileResults checkResult)
        {
            this.ParseResult = parseResult;
            this.CheckResult = checkResult;
        }

        public bool IsDefault => ParseResult is null || CheckResult is null;
    }
}