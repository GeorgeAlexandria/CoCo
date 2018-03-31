using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CoCo
{
    public static class SyntaxNodeExtensions
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

            // TODO: can it replace to getting the first level namespace (first level can be global namespace)?
            var namespaceText = symbol.ToString();
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