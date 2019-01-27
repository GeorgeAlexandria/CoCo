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
        [Import]
        private IViewElementFactoryService _viewElementFactoryService;

        public IToolTipPresenter Create(ITextView textView, ToolTipParameters parameters)
        {
            return new ToolTipPresenter(viewElementFactoryService, textView, parameters);

            return parameters.TrackMouse
                ? new MouseTrackToolTipPresenter(_viewElementFactoryService, textView, parameters)
                : new ToolTipPresenter(_viewElementFactoryService, textView, parameters);
        }
    }
}