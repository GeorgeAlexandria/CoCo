using System.ComponentModel.Composition;
using CoCo.Analyser.CSharp;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace CoCo.Providers
{
    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [ContentType("any")]
    internal sealed class QuickInfoProvider : IAsyncQuickInfoSourceProvider
    {
        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new QuickInfoSource(textBuffer));
        }
    }
}