using System.Collections.Generic;
using FSharp.Compiler.SourceCodeServices;
using Microsoft.FSharp.Control;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser.Classifications.FSharp
{
    internal class FSharpClassifierService
    {
        public static FSharpClassifierService Instance { get; } = new FSharpClassifierService();

        private FSharpClassifierService()
        {
        }

        public IList<ClassificationSpan> GetClassificationSpans(FSharpParseFileResults parseResults, FSharpCheckFileResults checkResults)
        {
            var result = new List<ClassificationSpan>();
            var symbolsUse = FSharpAsync.RunSynchronously(checkResults.GetAllUsesOfAllSymbolsInFile(), null, null);

            // TODO: iterate symbols isn't allowed to classify keywords => process untyped AST in the future
            foreach (var item in symbolsUse)
            {
                var symbol = item.Symbol;

                // TODO: match symbols
                switch (symbol)
                {
                    case FSharpActivePatternCase _:
                        break;

                    case FSharpEntity _:
                        break;

                    case FSharpField _:
                        break;

                    case FSharpGenericParameter _:
                        break;

                    case FSharpMemberOrFunctionOrValue _:
                        break;

                    case FSharpParameter _:
                        break;

                    case FSharpStaticParameter _:
                        break;

                    case FSharpUnionCase _:
                        break;

                    default:
                        break;
                }
            }

            return result;
        }
    }
}