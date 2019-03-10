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
    public struct Options
    {
        public Option EditorOption { get; set; }

        public QuickInfoOption QuickInfoOption { get; set; }
    }

    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideOptionPage(typeof(ClassificationsOption), "CoCo", "Classifications", 0, 0, true)]
    [ProvideOptionPage(typeof(PresetsOption), "CoCo", "Presets", 0, 0, true)]
    [ProvideOptionPage(typeof(QuickInfoOptionPage), "CoCo", "Quick info", 0, 0, true)]
    [Guid(Guids.Package)]
    public sealed class VsPackage : AsyncPackage
    {
        private static OptionViewModel _editorOptionViewModel;
        private static QuickInfoOptionViewModel _quickInfoOptionViewModel;

        private static int _receivedEditorCount;
        private static bool _quickInfoWasRealesed;

        private static bool _optionsWereApplied;

        internal static OptionViewModel ReceiveEditorContext()
        {
            ++_receivedEditorCount;
            if (!(_editorOptionViewModel is null)) return _editorOptionViewModel;

            return _editorOptionViewModel = new OptionViewModel(ReceiveEditorOption(), ResetValuesProvider.Instance);
        }

        internal static QuickInfoOptionViewModel ReceiveQuickInfoContext()
        {
            _quickInfoWasRealesed = false;
            if (!(_quickInfoOptionViewModel is null)) return _quickInfoOptionViewModel;

            return _quickInfoOptionViewModel = new QuickInfoOptionViewModel(ReceiveQuickInfoOption());
        }

        internal static void SaveOption(OptionViewModel optionViewModel) =>
            _optionsWereApplied |= ReferenceEquals(optionViewModel, _editorOptionViewModel);

        internal static void SaveOption(QuickInfoOptionViewModel quickInfoOptionViewModel) =>
            _optionsWereApplied |= ReferenceEquals(_quickInfoOptionViewModel, quickInfoOptionViewModel);

        internal static void ReleaseOption(OptionViewModel optionViewModel)
        {
            if (_receivedEditorCount <= 0 || ReferenceEquals(optionViewModel, _editorOptionViewModel) && --_receivedEditorCount != 0)
            {
                return;
            }

            ReleaseOptions();
        }

        internal static void ReleaseOption(QuickInfoOptionViewModel optionViewModel)
        {
            if (_quickInfoWasRealesed || !ReferenceEquals(_quickInfoOptionViewModel, optionViewModel)) return;

            _quickInfoWasRealesed = true;
            ReleaseOptions();
        }

        private static void ReleaseOptions()
        {
            if (!_quickInfoWasRealesed || _receivedEditorCount != 0) return;

            if (_optionsWereApplied)
            {
                var allOptions = new Options
                {
                    EditorOption = _editorOptionViewModel.ExtractData(),
                    QuickInfoOption = _quickInfoOptionViewModel.ExtractData(),
                };
                Release(allOptions);
                _optionsWereApplied = false;
            }

            _editorOptionViewModel = null;
            _quickInfoOptionViewModel = null;
        }

        private static QuickInfoOption ReceiveQuickInfoOption()
        {
            MigrationService.MigrateSettingsTo_2_0_0();
            var settings = SettingsManager.LoadSettings(Paths.CoCoSettingsFile, Paths.CoCoQuickInfoSettingsFile, MigrationService.Instance);
            var option = OptionService.ToQuickInfoOption(settings.QuickInfo);
            return option;
        }

        private static Option ReceiveEditorOption()
        {
            MigrationService.MigrateSettingsTo_2_0_0();
            var settings = SettingsManager.LoadSettings(Paths.CoCoSettingsFile, Paths.CoCoQuickInfoSettingsFile, MigrationService.Instance);
            var option = OptionService.ToEditorOption(settings.Editor);
            FormattingService.SetFormattingOptions(option);
            return option;
        }

        private static void Release(Options option)
        {
            var editorOption = option.EditorOption;
            FormattingService.SetFormattingOptions(editorOption);
            AnalyzingService.SetAnalyzingOptions(editorOption);

            var settings = new Settings.Settings
            {
                Editor = OptionService.ToSettings(editorOption),
                EditorPath = Paths.CoCoSettingsFile,

                QuickInfo = OptionService.ToSettings(option.QuickInfoOption),
                QuickInfoPath = Paths.CoCoQuickInfoSettingsFile,
            };

            SettingsManager.SaveSettings(settings);
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

    public class ClassificationsOption : DialogPage<OptionViewModel>
    {
        protected override FrameworkElement GetChild() => new ClassificationsControl();

        protected override OptionViewModel GetChildContext() => VsPackage.ReceiveEditorContext();

        protected override void ReleaseChildContext(OptionViewModel childContext) => VsPackage.ReleaseOption(childContext);

        protected override void SaveChildContext(OptionViewModel childContext) => VsPackage.SaveOption(childContext);
    }

    public class PresetsOption : DialogPage<OptionViewModel>
    {
        protected override FrameworkElement GetChild() => new PresetsControl();

        protected override OptionViewModel GetChildContext() => VsPackage.ReceiveEditorContext();

        protected override void ReleaseChildContext(OptionViewModel childContext) => VsPackage.ReleaseOption(childContext);

        protected override void SaveChildContext(OptionViewModel childContext) => VsPackage.SaveOption(childContext);
    }

    public class QuickInfoOptionPage : DialogPage<QuickInfoOptionViewModel>
    {
        protected override FrameworkElement GetChild() => new QuickInfoControl();

        protected override QuickInfoOptionViewModel GetChildContext() => VsPackage.ReceiveQuickInfoContext();

        protected override void ReleaseChildContext(QuickInfoOptionViewModel childContext) => VsPackage.ReleaseOption(childContext);

        protected override void SaveChildContext(QuickInfoOptionViewModel childContext) => VsPackage.SaveOption(childContext);
    }
}