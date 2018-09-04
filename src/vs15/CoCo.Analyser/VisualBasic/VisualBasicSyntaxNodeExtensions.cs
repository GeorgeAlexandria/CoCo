using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CoCo.Analyser.VisualBasic
{
    internal static class VisualBasicSyntaxNodeExtensions
    {
        public static SyntaxNode HandleNode(this SyntaxNode node) =>
           node.IsKind(SyntaxKind.SimpleArgument) ? (node as SimpleArgumentSyntax).GetExpression() : node;
    }
}