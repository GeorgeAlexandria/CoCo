using System;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;

namespace CoCo
{
    internal class ToolTipPresenter : IToolTipPresenter
    {
        private readonly ITextView _textView;
        private readonly ToolTipParameters _toolTipParameters;
        private readonly Popup _popup = new Popup();

        public ToolTipPresenter(
            ITextView textView,
            ToolTipParameters toolTipParameters)
        {
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
        }

        private void UpdatePopup(IEnumerable<object> content)
        {
            // TODO: append a custom control as a child of popup and put to it the input content
        }

        private void OnPopupClosed(object sender, EventArgs e) => Dismiss();

        private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e) => Dismiss();
    }
}