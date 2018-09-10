using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CoCo.Analyser.CSharp
{
    public static class CSharpSyntaxNodeExtensions
    {
        public static SyntaxNode HandleNode(this SyntaxNode node) =>
           node.IsKind(SyntaxKind.Argument) ? (node as ArgumentSyntax).Expression : node;

        public static bool IsAliasNamespace(this SyntaxNode node, ISymbol symbol)
        {
            if (!(node is IdentifierNameSyntax identifierName))
            {
                Log.Error($"Node {node} for namespace isn't IdentifierNameSyntax");
                return false;
            }
            switch (identifierName.Parent)
            {
                case QualifiedNameSyntax qualifiedName when qualifiedName.Left != identifierName: return false;
                case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression != identifierName: return false;
            }
            if(!(symbol is INamespaceSymbol namespaceSymbol))
            {
                Log.Error($"Symbol {symbol} isn't INamespaceSymbol");
                return false;
            }

            while (!(namespaceSymbol is null) && !namespaceSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                namespaceSymbol = namespaceSymbol.ContainingNamespace;
            }

            var namespaceText = namespaceSymbol.Name;
            var identifierText = identifierName.Identifier.ValueText;

            // NOTE: identifier is longer than a namespace => alias
            if (identifierText.Length > namespaceText.Length) return true;

            var i = 0;
            for (; i < identifierText.Length; ++i)
            {
                if (!namespaceText[i].Equals(identifierText[i])) return true;
            }

            // NOTE: identifier equals the first level namespace => namespace
            if (i == namespaceText.Length || i < namespaceText.Length && namespaceText[i] == '.') return false;
            return true;
        }
    }
}