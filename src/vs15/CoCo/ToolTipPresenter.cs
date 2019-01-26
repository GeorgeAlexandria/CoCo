using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using CoCo.UI;
using CoCo.UI.ViewModels;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;

namespace CoCo
{
    internal class ToolTipPresenter : IToolTipPresenter
    {
        private readonly IViewElementFactoryService _viewElementFactoryService;
        private readonly ITextView _textView;
        private readonly ToolTipParameters _toolTipParameters;
        private readonly Popup _popup = new Popup();

        public ToolTipPresenter(
            IViewElementFactoryService viewElementFactoryService,
            ITextView textView,
            ToolTipParameters toolTipParameters)
        {
            this._viewElementFactoryService = viewElementFactoryService ?? throw new ArgumentNullException(nameof(viewElementFactoryService));
            this._textView = textView ?? throw new ArgumentNullException(nameof(textView));
            this._toolTipParameters = toolTipParameters ?? throw new ArgumentNullException(nameof(toolTipParameters));
        }

        public event EventHandler Dismissed;

        public virtual void Dismiss()
        {
            if (!(_popup is null))
            {
                _popup.Closed -= OnPopupClosed;
                _popup.IsOpen = false;
                _popup.Child = null;

                _textView.TextBuffer.Changed -= OnTextBufferChanged;
            }

            if (Dismissed is null) return;
            Dismissed(this, EventArgs.Empty);
        }

        public virtual void StartOrUpdate(ITrackingSpan applicableToSpan, IEnumerable<object> content)
        {
            if (_popup.IsOpen)
            {
                UpdatePopup(content);
                return;
            }
            InitializePopup(content);
        }

        private void InitializePopup(IEnumerable<object> content)
        {
            UpdatePopup(content);

            _popup.Closed += OnPopupClosed;
            _textView.TextBuffer.Changed += OnTextBufferChanged;

            _popup.IsOpen = true;
            _popup.BringIntoView();
        }

        private void UpdatePopup(IEnumerable<object> content)
        {
            IEnumerable<UIElement> ToUIElements(IEnumerable<object> models)
            {
                foreach (var item in models)
                {
                    var element = _viewElementFactoryService.CreateViewElement<UIElement>(_textView, item);
                    if (element is null) continue;
                    yield return element;
                }
            }

            _popup.Child = new VsToolTipControl
            {
                DataContext = new VsToolTipViewModel(ToUIElements(content)),
            };
        }

        private void OnPopupClosed(object sender, EventArgs e) => Dismiss();

        private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e) => Dismiss();
    }
}