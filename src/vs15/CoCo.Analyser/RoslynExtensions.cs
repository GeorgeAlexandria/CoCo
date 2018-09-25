using Microsoft.CodeAnalysis;

namespace CoCo.Analyser
{
    public static class RoslynExtensions
    {
        /// <summary>
        /// Try to retrieve the symbol for <paramref name="node"/>
        /// </summary>
        /// <param name="failureReason">Contains the roslyn failure reason.
        /// It can be not <see cref="CandidateReason.None"/>, when symbol was retrieved</param>
        /// <returns>True if symbol was retrieved else false</returns>
        public static bool TryGetSymbolInfo(
            this SemanticModel semanticModel, SyntaxNode node, out ISymbol symbol, out CandidateReason failureReason)
        {
            var info = semanticModel.GetSymbolInfo(node);
            failureReason = info.CandidateReason;

            symbol = info.Symbol ?? semanticModel.GetDeclaredSymbol(node);
            if (symbol is null &&
                (info.CandidateReason == CandidateReason.Ambiguous ||
                info.CandidateReason == CandidateReason.LateBound ||
                info.CandidateReason == CandidateReason.MemberGroup ||
                info.CandidateReason == CandidateReason.OverloadResolutionFailure) &&
                info.CandidateSymbols.Length > 0)
            {
                /// TODO: would be nice to try match the type of <paramref name="node"/> with
                /// the corresponding ISymbol from all of <see cref="SymbolInfo.CandidateSymbols"/>
                symbol = info.CandidateSymbols[0];
            }

            return !(symbol is null);
        }
    }
}