using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using CoCo.UI;
using CoCo.UI.ViewModels;
using Microsoft.VisualStudio.Shell;

namespace CoCo
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideOptionPage(typeof(DialogOption), "CoCo", "CoCo", 0, 0, true)]
    [ProvideOptionPage(typeof(PresetOption), "CoCo", "Preset", 0, 0, true)]
    [Guid("b933474d-306e-434f-952d-a820c849ed07")]
    public sealed class VsPackage : Package
    {
        private static OptionViewModel _optionViewModel;

        private static int _receivedCount;
        private static bool _isApply;

        internal static OptionViewModel ReceiveOption()
        {
            ++_receivedCount;
            if (!(_optionViewModel is null)) return _optionViewModel;

            var option = OptionProvider.ReceiveOption();
            return _optionViewModel = new OptionViewModel(option);
        }

        internal static void SaveOption(OptionViewModel optionViewModel) =>
            _isApply |= ReferenceEquals(optionViewModel, _optionViewModel);

        internal static void ReleaseOption(OptionViewModel optionViewModel)
        {
            if (_receivedCount <= 0 || ReferenceEquals(optionViewModel, _optionViewModel) && --_receivedCount != 0) return;

            if (_isApply)
            {
                var option = _optionViewModel.ExtractData();
                OptionProvider.ReleaseOption(option);
                _isApply = false;
            }

            _optionViewModel = null;
        }
    }

    public class PresetOption : UIElementDialogPage
    {
        private OptionViewModel _view;

        private PresetsControl _child;

        protected override UIElement Child => _child ?? (_child = new PresetsControl());

        protected override void OnActivate(CancelEventArgs e)
        {
            if (_view is null)
            {
                _view = VsPackage.ReceiveOption();
                _child.DataContext = _view;
            }

            base.OnActivate(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            VsPackage.ReleaseOption(_view);
            _view = null;

            base.OnClosed(e);
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                VsPackage.SaveOption(_view);
            }
            base.OnApply(e);
        }
    }

    public class DialogOption : UIElementDialogPage
    {
        private OptionViewModel _view;

        private OptionControl _child;

        protected override UIElement Child => _child ?? (_child = new OptionControl());

        protected override void OnActivate(CancelEventArgs e)
        {
            if (_view is null)
            {
                _view = VsPackage.ReceiveOption();
                _child.DataContext = _view;
            }

            base.OnActivate(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            VsPackage.ReleaseOption(_view);
            _view = null;

            base.OnClosed(e);
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                VsPackage.SaveOption(_view);
            }
            base.OnApply(e);
        }
    }
}