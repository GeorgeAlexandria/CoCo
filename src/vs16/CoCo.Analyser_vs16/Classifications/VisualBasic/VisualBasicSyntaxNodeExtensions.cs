using System.Collections.Generic;
using CoCo.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CoCo.Analyser.Classifications.VisualBasic
{
    internal static class VisualBasicSyntaxNodeExtensions
    {
        /// <summary>
        /// Determines if the <paramref name="token"/> is a child of Exit statement
        /// </summary>
        public static bool IsExitStatementKeyword(this SyntaxToken token)
        {
            if (token.Parent is ExitStatementSyntax exitStatement)
            {
                switch ((SyntaxKind)exitStatement.RawKind)
                {
                    case SyntaxKind.ExitDoStatement:
                    case SyntaxKind.ExitForStatement:
                    case SyntaxKind.ExitFunctionStatement:
                    case SyntaxKind.ExitOperatorStatement:
                    case SyntaxKind.ExitPropertyStatement:
                    case SyntaxKind.ExitSelectStatement:
                    case SyntaxKind.ExitSubStatement:
                    case SyntaxKind.ExitTryStatement:
                    case SyntaxKind.ExitWhileStatement:
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if the <paramref name="token"/> is a child of End statement
        /// </summary>
        public static bool IsEndStatementKeyword(this SyntaxToken token)
        {
            if (token.Parent is EndBlockStatementSyntax exitStatement)
            {
                switch ((SyntaxKind)exitStatement.RawKind)
                {
                    case SyntaxKind.EndIfStatement:
                    case SyntaxKind.EndSelectStatement:
                    case SyntaxKind.EndWhileStatement:
                    case SyntaxKind.EndTryStatement:
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if the <paramref name="token"/> is a child of one If statements
        /// </summary>
        public static bool IsIfStatementKeyword(this SyntaxToken token) =>
            token.Parent is IfStatementSyntax || token.Parent is SingleLineIfStatementSyntax;

        /// <summary>
        /// If <paramref name="node"/> is one of a few special types then extracts a specific sub node and returns it,
        /// else returns <paramref name="node"/>
        /// </summary>
        public static SyntaxNode HandleNode(this SyntaxNode node) =>
            node.IsKind(SyntaxKind.SimpleArgument) ? (node as SimpleArgumentSyntax).GetExpression() :
            node.IsKind(SyntaxKind.SimpleImportsClause) ? (node as SimpleImportsClauseSyntax).Name :
            node.IsKind(SyntaxKind.CrefReference) ? (node as CrefReferenceSyntax).Name :
            node.IsKind(SyntaxKind.TypeConstraint) ? (node as TypeConstraintSyntax).Type :
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

            foreach (var item in semanticModel.Compilation.MemberImports())
            {
                if (item is INamespaceSymbol @namespace)
                {
                    namespaces.Add(@namespace);
                }
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