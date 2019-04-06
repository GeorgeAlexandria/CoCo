using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;

namespace CoCo.Analyser
{
    public static class VisualStudioExtensions
    {
        public static string GetLanguage(this ITextBuffer buffer)
        {
            var document = buffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (!(document is null) &&  document.TryGetSyntaxRoot(out var root))
            {
                if (root.Language.Equals(LanguageNames.CSharp)) return Languages.CSharp;
                if (root.Language.Equals(LanguageNames.VisualBasic)) return Languages.VisualBasic;
            }
            return null;
        }
    }
}