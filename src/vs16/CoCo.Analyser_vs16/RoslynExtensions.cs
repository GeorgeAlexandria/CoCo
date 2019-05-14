using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace CoCo.Analyser
{
    public static class RoslynExtensions
    {
        /// <summary>
        /// Returns the first declartation of type <typeparamref name="T"/>
        /// </summary>
        public static async Task<T> GetDeclarationAsync<T>(this ISymbol symbol, CancellationToken cancellationToken = default)
            where T : SyntaxNode
        {
            foreach (var reference in symbol.DeclaringSyntaxReferences)
            {
                var node = await reference.GetSyntaxAsync(cancellationToken);
                if (node is T castedNode) return castedNode;
            }
            return null;
        }

        public static bool IsExtensionMethod(this IMethodSymbol method) =>
            method.IsExtensionMethod || method.MethodKind == MethodKind.ReducedExtension;

        public static async Task<SyntaxToken> GetIntersectTokenAsync(
            this SyntaxTree syntaxTree, int position, bool findInsideTrivia, CancellationToken cancellationToken)
        {
            if (position >= syntaxTree.Length) return default;

            var root = await syntaxTree.GetRootAsync(cancellationToken);
            var token = root.FindToken(position, findInsideTrivia);
            if (token.Span.Contains(position) || token.Span.End == position) return token;

            // NOTE: if position is the end of previous token => return it.
            token = token.GetPreviousToken();
            return token.Span.End == position ? token : default;
        }

        public static ImmutableArray<Compilation> GetReferencedCompilations(this Compilation compilation)
        {
            var builder = ImmutableArray.CreateBuilder<Compilation>();
            foreach (var reference in compilation.References)
            {
                if (reference is CompilationReference compilationReference)
                {
                    builder.Add(compilationReference.Compilation);
                }
            }

            return builder.TryMoveToImmutable();
        }

        /// <summary>
        /// Retrieves declared symbol from <paramref name="token"/> or go to it parents until doesn't retrieve symbol
        /// </summary>
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

        /// <summary>
        /// Rerieves overriden symbol if <paramref name="symbol"/> is <see cref="IMethodSymbol"/>, <see cref="IPropertySymbol"/> or
        /// <see cref="IEventSymbol"/>, otherwise returns <see langword="null"/>
        /// </summary>
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

        public static Stack<INamedTypeSymbol> AncestorsContainingTypesAndSelf(this INamedTypeSymbol symbol)
        {
            var stack = new Stack<INamedTypeSymbol>();
            var current = symbol;
            while (!(current is null))
            {
                stack.Push(current);
                current = current.ContainingType;
            }
            return stack;
        }

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

        //TODO: Check behavior for document that isn't including in solution
        public static Document GetDocument(this Workspace workspace, SourceText text)
        {
            if (workspace == null) throw new ArgumentException("Input parameter is null", nameof(workspace));
            if (text == null) throw new ArgumentException("Input parameter is null", nameof(text));

            var id = workspace.GetDocumentIdInCurrentContext(text.Container);
            if (id == null) return null;

            return !workspace.CurrentSolution.ContainsDocument(id)
                ? workspace.CurrentSolution.WithDocumentText(id, text, PreservationMode.PreserveIdentity).GetDocument(id)
                : workspace.CurrentSolution.GetDocument(id);
        }
    }
}