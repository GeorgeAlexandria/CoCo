using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CoCo
{
    public static class SyntaxNodeExtensions
    {
        public static SyntaxNode HandleNode(this SyntaxNode node) =>
           node.Kind() == SyntaxKind.Argument ? (node as ArgumentSyntax).Expression : node;
    }
}
