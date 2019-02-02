using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using CoCo.UI;
using CoCo.UI.ViewModels;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;

namespace CoCo
{
    internal class ToolTipPresenter : IToolTipPresenter
    {
        private readonly IViewElementFactoryService _viewElementFactoryService;

        protected readonly ITextView textView;
        protected readonly ToolTipParameters toolTipParameters;
        protected readonly Popup popup = new Popup();

        public ToolTipPresenter(
            IViewElementFactoryService viewElementFactoryService,
            ITextView textView,
            ToolTipParameters toolTipParameters)
        {
            this._viewElementFactoryService = viewElementFactoryService ?? throw new ArgumentNullException(nameof(viewElementFactoryService));
            this.textView = textView ?? throw new ArgumentNullException(nameof(textView));
            this.toolTipParameters = toolTipParameters ?? throw new ArgumentNullException(nameof(toolTipParameters));
        }

        public event EventHandler Dismissed;

        public virtual void Dismiss()
        {
            if (!(popup is null))
            {
                popup.Closed -= OnPopupClosed;
                popup.IsOpen = false;
                popup.Child = null;

                textView.TextBuffer.Changed -= OnTextBufferChanged;
            }

            if (Dismissed is null) return;
            Dismissed?.Invoke(this, EventArgs.Empty);
        }

        public virtual void StartOrUpdate(ITrackingSpan applicableToSpan, IEnumerable<object> content)
        {
            if (popup.IsOpen)
            {
                UpdatePopup(content);
                return;
            }
            InitializePopup(content);
        }

        private void InitializePopup(IEnumerable<object> content)
        {
            popup.AllowsTransparency = true;
            UpdatePopup(content);

            popup.Closed += OnPopupClosed;
            textView.TextBuffer.Changed += OnTextBufferChanged;

            popup.IsOpen = true;
            popup.BringIntoView();
        }

        private void UpdatePopup(IEnumerable<object> content)
        {
            IEnumerable<UIElement> ToUIElements(IEnumerable<object> models)
            {
                foreach (var item in models)
                {
                    var element = _viewElementFactoryService.CreateViewElement<UIElement>(textView, item);
                    if (element is null) continue;
                    yield return element;
                }
            }

            var background = VSColorTheme.GetThemedColor(EnvironmentColors.ToolTipColorKey);
            var borderColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolTipBorderColorKey);
            popup.Child = new VsToolTipControl
            {
                DataContext = new VsToolTipViewModel(ToUIElements(content), background.DrawingToMedia(), borderColor.DrawingToMedia()),
            };
        }

        private void OnPopupClosed(object sender, EventArgs e) => Dismiss();

        private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e) => Dismiss();
    }
}