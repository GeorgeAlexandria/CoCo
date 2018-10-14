using System.Collections.Generic;
using CoCo.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CoCo.Analyser.VisualBasic
{
    internal static class VisualBasicSyntaxNodeExtensions
    {
        public static SyntaxNode HandleNode(this SyntaxNode node) =>
            node.IsKind(SyntaxKind.SimpleArgument) ? (node as SimpleArgumentSyntax).GetExpression() :
            node.IsKind(SyntaxKind.SimpleImportsClause) ? (node as SimpleImportsClauseSyntax).Name :
            node.IsKind(SyntaxKind.CrefReference) ? (node as CrefReferenceSyntax).Name :
            node;

        public static bool IsDescendantXmlDocComment(this SyntaxNode node)
        {
            bool IsXmlKind(SyntaxKind kind) => kind == SyntaxKind.XmlNameAttribute || kind == SyntaxKind.XmlCrefAttribute;

            var current = node;
            while (
                !(current.Parent is null) &&
                !IsXmlKind(current.Parent.Kind()) &&
                !(current.Parent is DirectiveTriviaSyntax) &&
                !(current.Parent is SkippedTokensTriviaSyntax))
            {
                current = current.Parent;
            }

            return !(current.Parent is null || current.Parent is DirectiveTriviaSyntax || current.Parent is SkippedTokensTriviaSyntax);
        }

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
            if (namespaceText.Length != identifierText.Length || !namespaceText.EqualsNoCase(identifierText)) return true;

            // NOTE: identifier is a part of X.Y => namespace
            switch (identifierName.Parent)
            {
                case QualifiedNameSyntax qualifiedName when qualifiedName.Left != identifierName: return false;
                case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression != identifierName: return false;
            }

            // NOTE: collect all namespaces which members are reachibille from the current context
            var namespaces = new HashSet<INamespaceSymbol>
            {
                semanticModel.Compilation.RootNamespace(),
                semanticModel.Compilation.GlobalNamespace
            };

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