using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using CoCo.Editor;
using CoCo.Settings;
using CoCo.UI;
using CoCo.UI.Data;
using CoCo.UI.ViewModels;
using Microsoft.VisualStudio.Shell;

namespace CoCo
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideOptionPage(typeof(GeneralOptionPage), "CoCo", "General", 0, 0, true)]
    [ProvideOptionPage(typeof(ClassificationsOption), "CoCo", "Classifications", 0, 0, true)]
    [ProvideOptionPage(typeof(PresetsOption), "CoCo", "Presets", 0, 0, true)]
    [Guid(Guids.Package)]
    public sealed class VsPackage : AsyncPackage
    {
        private static ClassificationOptionViewModel _editorOptionViewModel;
        private static GeneralOptionViewModel _generalOptionViewModel;

        private static int _receivedEditorCount;
        private static bool _editorOptionsWereApplied;
        private static bool _generalOptionsWereApplied;

        internal static ClassificationOptionViewModel ReceiveEditorContext()
        {
            ++_receivedEditorCount;
            if (!(_editorOptionViewModel is null)) return _editorOptionViewModel;

            return _editorOptionViewModel = new ClassificationOptionViewModel(ReceiveEditorOption(), ResetValuesProvider.Instance);
        }

        internal static GeneralOptionViewModel ReceiveQuickInfoContext()
        {
            if (!(_generalOptionViewModel is null)) return _generalOptionViewModel;

            return _generalOptionViewModel = new GeneralOptionViewModel(ReceiveGeneralOption());
        }

        internal static void SaveOption(ClassificationOptionViewModel optionViewModel) =>
            _editorOptionsWereApplied |= ReferenceEquals(optionViewModel, _editorOptionViewModel);

        internal static void SaveOption(GeneralOptionViewModel generalOptionViewModel) =>
            _generalOptionsWereApplied |= ReferenceEquals(_generalOptionViewModel, generalOptionViewModel);

        internal static void ReleaseOption(ClassificationOptionViewModel optionViewModel)
        {
            if (_receivedEditorCount <= 0 || ReferenceEquals(optionViewModel, _editorOptionViewModel) && --_receivedEditorCount != 0)
            {
                return;
            }

            if (_editorOptionsWereApplied)
            {
                Release(_editorOptionViewModel.ExtractData());
                _editorOptionsWereApplied = false;
            }

            _editorOptionViewModel = null;
        }

        internal static void ReleaseOption(GeneralOptionViewModel optionViewModel)
        {
            if (!ReferenceEquals(_generalOptionViewModel, optionViewModel)) return;

            if (_generalOptionsWereApplied)
            {
                _generalOptionsWereApplied = false;
                Release(_generalOptionViewModel.ExtractData());
            }

            _generalOptionViewModel = null;
        }

        private static GeneralData ReceiveGeneralOption()
        {
            MigrationService.MigrateSettingsTo_3_1_0();
            var settings = SettingsManager.LoadGeneralSettings(Paths.CoCoGeneralSettingsFile, MigrationService.Instance);
            var option = OptionService.ToOption(settings);
            return option;
        }

        private static ClassificationData ReceiveEditorOption()
        {
            MigrationService.MigrateSettingsTo_2_0_0();
            var settings = SettingsManager.LoadEditorSettings(Paths.CoCoClassificationSettingsFile, MigrationService.Instance);
            var option = OptionService.ToOption(settings);
            FormattingService.SetFormattingOptions(option);
            return option;
        }

        private static void Release(ClassificationData option)
        {
            FormattingService.SetFormattingOptions(option);
            ClassificationChangingService.SetAnalyzingOptions(option);
            var settings = OptionService.ToSettings(option);
            SettingsManager.SaveSettings(settings, Paths.CoCoClassificationSettingsFile);
        }

        private static void Release(GeneralData option)
        {
            GeneralChangingService.SetGeneralOptions(option);
            var settings = OptionService.ToSettings(option);
            SettingsManager.SaveSettings(settings, Paths.CoCoGeneralSettingsFile);
        }
    }

    public abstract class DialogPage<T> : UIElementDialogPage where T : class
    {
        private T _childDataContext;

        private FrameworkElement _child;

        protected override UIElement Child => _child ?? (_child = GetChild());

        protected abstract FrameworkElement GetChild();

        protected abstract T GetChildContext();

        protected abstract void SaveChildContext(T childContext);

        protected abstract void ReleaseChildContext(T childContext);

        protected override void OnActivate(CancelEventArgs e)
        {
            /// NOTE: imitate page's initialization using <see cref="OnActivate"/>
            if (_childDataContext is null)
            {
                _child.DataContext = _childDataContext = GetChildContext();
            }

            base.OnActivate(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            ReleaseChildContext(_childDataContext);
            _childDataContext = null;

            base.OnClosed(e);
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            // NOTE: save options only when was clicked Ok button at the options page
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                SaveChildContext(_childDataContext);
            }
            base.OnApply(e);
        }
    }

    public class ClassificationsOption : DialogPage<ClassificationOptionViewModel>
    {
        protected override FrameworkElement GetChild() => new ClassificationsControl();

        protected override ClassificationOptionViewModel GetChildContext() => VsPackage.ReceiveEditorContext();

        protected override void ReleaseChildContext(ClassificationOptionViewModel childContext) => VsPackage.ReleaseOption(childContext);

        protected override void SaveChildContext(ClassificationOptionViewModel childContext) => VsPackage.SaveOption(childContext);
    }

    public class PresetsOption : DialogPage<ClassificationOptionViewModel>
    {
        protected override FrameworkElement GetChild() => new PresetsControl();

        protected override ClassificationOptionViewModel GetChildContext() => VsPackage.ReceiveEditorContext();

        protected override void ReleaseChildContext(ClassificationOptionViewModel childContext) => VsPackage.ReleaseOption(childContext);

        protected override void SaveChildContext(ClassificationOptionViewModel childContext) => VsPackage.SaveOption(childContext);
    }

    public class GeneralOptionPage : DialogPage<GeneralOptionViewModel>
    {
        protected override FrameworkElement GetChild() => new GeneralControl();

        protected override GeneralOptionViewModel GetChildContext() => VsPackage.ReceiveQuickInfoContext();

        protected override void ReleaseChildContext(GeneralOptionViewModel childContext) => VsPackage.ReleaseOption(childContext);

        protected override void SaveChildContext(GeneralOptionViewModel childContext) => VsPackage.SaveOption(childContext);
    }
}