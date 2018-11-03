using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using CoCo.Providers;
using CoCo.Services;
using CoCo.Settings;
using CoCo.UI;
using CoCo.UI.Data;
using CoCo.UI.ViewModels;
using Microsoft.VisualStudio.Shell;

namespace CoCo
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideOptionPage(typeof(ClassificationsOption), "CoCo", "Classifications", 0, 0, true)]
    [ProvideOptionPage(typeof(PresetsOption), "CoCo", "Presets", 0, 0, true)]
    [Guid(Guids.Package)]
    public sealed class VsPackage : AsyncPackage
    {
        private static OptionViewModel _optionViewModel;

        private static int _receivedCount;
        private static bool _isApply;

        internal static OptionViewModel ReceiveOption()
        {
            ++_receivedCount;
            if (!(_optionViewModel is null)) return _optionViewModel;

            return _optionViewModel = new OptionViewModel(Receive(), ResetValuesProvider.Instance);
        }

        internal static void SaveOption(OptionViewModel optionViewModel) =>
            _isApply |= ReferenceEquals(optionViewModel, _optionViewModel);

        internal static void ReleaseOption(OptionViewModel optionViewModel)
        {
            if (_receivedCount <= 0 || ReferenceEquals(optionViewModel, _optionViewModel) && --_receivedCount != 0) return;

            if (_isApply)
            {
                Release(_optionViewModel.ExtractData());
                _isApply = false;
            }

            _optionViewModel = null;
        }

        private static Option Receive()
        {
            MigrationService.MigrateSettingsTo_2_0_0();
            var settings = SettingsManager.LoadSettings(Paths.CoCoSettingsFile, MigrationService.Instance);
            var option = OptionService.ToOption(settings);
            FormattingService.SetFormattingOptions(option);
            return option;
        }

        private static void Release(Option option)
        {
            FormattingService.SetFormattingOptions(option);
            AnalyzingService.SetAnalyzingOptions(option);
            var settings = OptionService.ToSettings(option);
            SettingsManager.SaveSettings(settings, Paths.CoCoSettingsFile);
        }
    }

    public abstract class DialogPage : UIElementDialogPage
    {
        private OptionViewModel _childDataContext;

        private FrameworkElement _child;

        protected override UIElement Child => _child ?? (_child = GetChild());

        protected abstract FrameworkElement GetChild();

        protected override void OnActivate(CancelEventArgs e)
        {
            /// NOTE: imitate page's initialization using <see cref="OnActivate"/>
            if (_childDataContext is null)
            {
                _childDataContext = VsPackage.ReceiveOption();
                _child.DataContext = _childDataContext;
            }

            base.OnActivate(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            VsPackage.ReleaseOption(_childDataContext);
            _childDataContext = null;

            base.OnClosed(e);
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            // NOTE: save options only when was clicked Ok button at the options page
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                VsPackage.SaveOption(_childDataContext);
            }
            base.OnApply(e);
        }
    }

    public class ClassificationsOption : DialogPage
    {
        protected override FrameworkElement GetChild() => new ClassificationsControl();
    }

    public class PresetsOption : DialogPage
    {
        protected override FrameworkElement GetChild() => new PresetsControl();
    }
}