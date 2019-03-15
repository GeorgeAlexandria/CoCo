using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using CoCo.Editor;
using CoCo.QuickInfo;
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
    [ProvideOptionPage(typeof(QuickInfoOptionPage), "CoCo", "Quick info", 0, 0, true)]
    [Guid(Guids.Package)]
    public sealed class VsPackage : AsyncPackage
    {
        private static EditorOptionViewModel _editorOptionViewModel;
        private static QuickInfoOptionViewModel _quickInfoOptionViewModel;

        private static int _receivedEditorCount;
        private static bool _editorOptionsWereApplied;
        private static bool _quickInfoOptionsWereApplied;

        internal static EditorOptionViewModel ReceiveEditorContext()
        {
            ++_receivedEditorCount;
            if (!(_editorOptionViewModel is null)) return _editorOptionViewModel;

            return _editorOptionViewModel = new EditorOptionViewModel(ReceiveEditorOption(), ResetValuesProvider.Instance);
        }

        internal static QuickInfoOptionViewModel ReceiveQuickInfoContext()
        {
            if (!(_quickInfoOptionViewModel is null)) return _quickInfoOptionViewModel;

            return _quickInfoOptionViewModel = new QuickInfoOptionViewModel(ReceiveQuickInfoOption());
        }

        internal static void SaveOption(EditorOptionViewModel optionViewModel) =>
            _editorOptionsWereApplied |= ReferenceEquals(optionViewModel, _editorOptionViewModel);

        internal static void SaveOption(QuickInfoOptionViewModel quickInfoOptionViewModel) =>
            _quickInfoOptionsWereApplied |= ReferenceEquals(_quickInfoOptionViewModel, quickInfoOptionViewModel);

        internal static void ReleaseOption(EditorOptionViewModel optionViewModel)
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

        internal static void ReleaseOption(QuickInfoOptionViewModel optionViewModel)
        {
            if (!ReferenceEquals(_quickInfoOptionViewModel, optionViewModel)) return;

            if (_quickInfoOptionsWereApplied)
            {
                _quickInfoOptionsWereApplied = false;
                Release(_quickInfoOptionViewModel.ExtractData());
            }

            _quickInfoOptionViewModel = null;
        }

        private static QuickInfoOption ReceiveQuickInfoOption()
        {
            var settings = SettingsManager.LoadQuickInfoSettings(Paths.CoCoQuickInfoSettingsFile);
            var option = OptionService.ToOption(settings);
            return option;
        }

        private static EditorOption ReceiveEditorOption()
        {
            MigrationService.MigrateSettingsTo_2_0_0();
            var settings = SettingsManager.LoadEditorSettings(Paths.CoCoSettingsFile, MigrationService.Instance);
            var option = OptionService.ToOption(settings);
            FormattingService.SetFormattingOptions(option);
            return option;
        }

        private static void Release(EditorOption option)
        {
            FormattingService.SetFormattingOptions(option);
            ClassificationChangingService.SetAnalyzingOptions(option);
            var settings = OptionService.ToSettings(option);
            SettingsManager.SaveSettings(settings, Paths.CoCoSettingsFile);
        }

        private static void Release(QuickInfoOption option)
        {
            QuickInfoChangingService.SetQuickInfoOptions(option);
            var settings = OptionService.ToSettings(option);
            SettingsManager.SaveSettings(settings, Paths.CoCoQuickInfoSettingsFile);
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

    public class ClassificationsOption : DialogPage<EditorOptionViewModel>
    {
        protected override FrameworkElement GetChild() => new ClassificationsControl();

        protected override EditorOptionViewModel GetChildContext() => VsPackage.ReceiveEditorContext();

        protected override void ReleaseChildContext(EditorOptionViewModel childContext) => VsPackage.ReleaseOption(childContext);

        protected override void SaveChildContext(EditorOptionViewModel childContext) => VsPackage.SaveOption(childContext);
    }

    public class PresetsOption : DialogPage<EditorOptionViewModel>
    {
        protected override FrameworkElement GetChild() => new PresetsControl();

        protected override EditorOptionViewModel GetChildContext() => VsPackage.ReceiveEditorContext();

        protected override void ReleaseChildContext(EditorOptionViewModel childContext) => VsPackage.ReleaseOption(childContext);

        protected override void SaveChildContext(EditorOptionViewModel childContext) => VsPackage.SaveOption(childContext);
    }

    public class QuickInfoOptionPage : DialogPage<QuickInfoOptionViewModel>
    {
        protected override FrameworkElement GetChild() => new QuickInfoControl();

        protected override QuickInfoOptionViewModel GetChildContext() => VsPackage.ReceiveQuickInfoContext();

        protected override void ReleaseChildContext(QuickInfoOptionViewModel childContext) => VsPackage.ReleaseOption(childContext);

        protected override void SaveChildContext(QuickInfoOptionViewModel childContext) => VsPackage.SaveOption(childContext);
    }
}