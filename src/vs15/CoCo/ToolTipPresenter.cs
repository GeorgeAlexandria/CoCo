using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CoCo.UI;
using CoCo.UI.ViewModels;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;

namespace CoCo
{
    internal class MouseTrackToolTipPresenter : ToolTipPresenter
    {
        /// <summary>
        /// The element above which the tooltip popped
        /// </summary>
        private UIElement capturedElement;

        public MouseTrackToolTipPresenter(
            IViewElementFactoryService viewElementFactoryService, ITextView textView, ToolTipParameters toolTipParameters) :
            base(viewElementFactoryService, textView, toolTipParameters)
        {
        }

        public override void StartOrUpdate(ITrackingSpan applicableToSpan, IEnumerable<object> content)
        {
            if (!DismissOnOutOfView())
            {
                popup.Placement = PlacementMode.Mouse;
                if (!popup.IsVisible)
                {
                    SubscribeOnMove();
                }
                base.StartOrUpdate(applicableToSpan, content);
            }
        }

        public override void Dismiss()
        {
            if (!(capturedElement is null))
            {
                UnsubscribeOnMove();
            }
            base.Dismiss();
        }

        private void SubscribeOnMove()
        {
            if (capturedElement is null && Mouse.DirectlyOver is UIElement uielement)
            {
                uielement.MouseLeave += OnMouseLeave;
                uielement.MouseMove += OnMouseMove;
                capturedElement = uielement;
            }
        }

        private void UnsubscribeOnMove()
        {
            if (!(capturedElement is null))
            {
                capturedElement.MouseLeave -= OnMouseLeave;
                capturedElement.MouseMove -= OnMouseMove;
                capturedElement = null;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e) => DismissOnOutOfView();

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            // NOTE: unsubscribe from the current element and then subscribe to the element under
            // the current cursor position to keep tooltip when it's under the current position
            UnsubscribeOnMove();
            SubscribeOnMove();

            if (capturedElement is null)
            {
                Dismiss();
                return;
            }
            DismissOnOutOfView();
        }

        /// <summary>
        /// Dismiss when the mouse is over outside of view
        /// </summary>
        /// <returns></returns>
        private bool DismissOnOutOfView()
        {
            var wpfTextView = textView as IWpfTextView;
            if (wpfTextView is null || !popup.IsMouseOver && !toolTipParameters.KeepOpen && !wpfTextView.VisualElement.IsMouseOver)
            {
                Dismiss();
                return true;
            }
            return false;
        }
    }

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