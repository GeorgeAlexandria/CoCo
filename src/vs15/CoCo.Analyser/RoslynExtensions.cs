using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace CoCo.Analyser
{
    public static class RoslynExtensions
    {
        public static ISymbol GetDeclaredSymbol(
            this SemanticModel semanticModel, SyntaxToken token, CancellationToken cancellationToken = default)
        {
            var location = token.GetLocation();
            var parent = token.Parent;
            foreach (var item in parent.AncestorsAndSelf())
            {
                var declaredSymbol = semanticModel.GetDeclaredSymbol(item, cancellationToken);
                if (!(declaredSymbol is null) && declaredSymbol.Locations.Contains(location)) return declaredSymbol;
            }
            return null;
        }

        public static ISymbol GetOverriddenSymbol(this ISymbol symbol)
        {
            switch (symbol)
            {
                case IMethodSymbol method:
                    return method.OverriddenMethod;

                case IPropertySymbol property:
                    return property.OverriddenProperty;

                case IEventSymbol @event:
                    return @event.OverriddenEvent;
            }
            return null;
        }

        public static ImmutableArray<ISymbol> GetSymbolOrCandidates(
            this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken = default)
        {
            var info = semanticModel.GetSymbolInfo(node, cancellationToken);
            return
                !(info.Symbol is null) ? ImmutableArray.Create(info.Symbol) :
                info.CandidateSymbols.Length > 0 ? info.CandidateSymbols :
                ImmutableArray<ISymbol>.Empty;
        }

        public static bool IsErrorType(this ISymbol symbol) => (symbol as ITypeSymbol)?.TypeKind == TypeKind.Error;

        /// <summary>
        /// Determines if <paramref name="symbol"/> is awaitable: contains GetAwaiter method which returns
        /// <see cref="System.Runtime.CompilerServices.TaskAwaiter"/>
        /// </summary>
        public static bool IsAwaitable(this ISymbol symbol)
        {
            var typeToCheck = symbol is IMethodSymbol methodSymbol
                ? methodSymbol.ReturnType?.OriginalDefinition
                : symbol as ITypeSymbol;

            if (typeToCheck is null) return false;

            foreach (var item in typeToCheck.GetMembers(WellKnownMemberNames.GetAwaiter))
            {
                if (item is IMethodSymbol awaiter && awaiter.Parameters.Length == 0 &&
                    (awaiter.ReturnType.MetadataName.Equals("System.Runtime.CompilerServices.TaskAwaiter") ||
                    awaiter.ReturnType.MetadataName.Equals("System.Runtime.CompilerServices.TaskAwaiter`1")))
                {
                    return true;
                }
            }
            return false;
        }

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