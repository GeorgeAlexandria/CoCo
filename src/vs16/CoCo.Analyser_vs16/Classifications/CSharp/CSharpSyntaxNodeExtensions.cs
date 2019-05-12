using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CoCo.Analyser.Classifications.CSharp
{
    public static class CSharpSyntaxNodeExtensions
    {
        /// <summary>
        /// If <paramref name="node"/> is one of a few special types then extracts a specific sub node and returns it, 
        /// else returns <paramref name="node"/>
        /// </summary>
        public static SyntaxNode HandleNode(this SyntaxNode node) =>
            node.IsKind(SyntaxKind.Argument) ? (node as ArgumentSyntax).Expression :
            node.IsKind(SyntaxKind.NameMemberCref) ? (node as NameMemberCrefSyntax).Name :
            node.IsKind(SyntaxKind.SimpleBaseType) ? (node as SimpleBaseTypeSyntax).Type :
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
            if (namespaceText.Length != identifierText.Length || !namespaceText.Equals(identifierText)) return true;

            switch (identifierName.Parent)
            {
                case QualifiedNameSyntax qualifiedName when qualifiedName.Left != identifierName: return false;
                case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression != identifierName: return false;

                // NOTE: handle in a xml doc comments, because 
                // cref="A.B.C.D" => (QualifiedCref).(NameMemberCref) => (QualifiedName).(Name) => (A.B.C).(D)
                case NameMemberCrefSyntax nameCref: return false;
                case QualifiedCrefSyntax qualifiedCref when !(qualifiedCref.Container is IdentifierNameSyntax): return false;
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