using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CoCo.Analyser.CSharp
{
    public static class CSharpSyntaxNodeExtensions
    {
        public static SyntaxNode HandleNode(this SyntaxNode node) =>
           node.IsKind(SyntaxKind.Argument) ? (node as ArgumentSyntax).Expression : node;

        public static bool IsAliasNamespace(this SyntaxNode node, ISymbol symbol, SemanticModel semanticModel)
        {
            if (!(node is IdentifierNameSyntax identifierName))
            {
                Log.Error($"Node {node} for namespace isn't IdentifierNameSyntax");
                return false;
            }
            if (!(symbol is INamespaceSymbol namespaceSymbol))
            {
                Log.Error($"Symbol {symbol} isn't INamespaceSymbol");
                return false;
            }

            var namespaceText = namespaceSymbol.Name;
            var identifierText = identifierName.Identifier.ValueText;

            // NOTE: identifier doesn't equal the last level namespace => alias
            if (namespaceText.Length != identifierText.Length || !namespaceText.Equals(identifierText)) return true;

            switch (identifierName.Parent)
            {
                case QualifiedNameSyntax qualifiedName when qualifiedName.Left != identifierName: return false;
                case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression != identifierName: return false;
            }

            // NOTE: collect all namespaces which members are reachibille from the current context
            var namespaces = new HashSet<INamespaceSymbol> { semanticModel.Compilation.GlobalNamespace };

            // NOTE: global namespace has a couple of constituent global namespaces and in the C# a some namespaces can have on
            // of these constituent namespaces as containing namespaces, not the outer global => add them to
            foreach (var item in semanticModel.Compilation.GlobalNamespace.ConstituentNamespaces)
            {
                namespaces.Add(item);
            }

            var enclosingNamespace = semanticModel.GetEnclosingSymbol(node.Span.Start)?.ContainingNamespace;
            while (!(enclosingNamespace is null))
            {
                namespaces.Add(enclosingNamespace);
                enclosingNamespace = enclosingNamespace.ContainingNamespace;
            }

            // NOTE: symbol contains in reachibille namespaces => namespace
            if (namespaces.Contains(namespaceSymbol)) return false;
            return namespaceSymbol.ContainingNamespace is null || !namespaces.Contains(namespaceSymbol.ContainingNamespace);
        }
    }
}