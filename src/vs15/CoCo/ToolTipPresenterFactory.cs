using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace CoCo
{
    [Export(typeof(IToolTipPresenterFactory))]
    [Order(Before = "default")]
    internal class ToolTipPresenterFactory : IToolTipPresenterFactory
    {
        public IToolTipPresenter Create(ITextView textView, ToolTipParameters parameters)
        {
            return new ToolTipPresenter(textView, parameters);
        }
    }
}