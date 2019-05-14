using System.Collections.Generic;
using CoCo.Analyser.Editor;
using CoCo.UI.Data;

namespace CoCo
{
    internal sealed class GeneralChangingService : IEditorChangingService
    {
        public static readonly GeneralChangingService Instance = new GeneralChangingService();

        public event GeneralChangedEventHandler GeneralChanged;
        public event EditorChangedEventHandler EditorOptionsChanged;

        private GeneralChangingService()
        {
        }

        public static void SetGeneralOptions(GeneralOption option)
        {
            var changes = new Dictionary<string, GeneralInfo>(option.Languages.Count);
            var editorChanges = new Dictionary<string, bool>(option.Languages.Count);
            foreach (var item in option.Languages)
            {
                changes[item.Name] = new GeneralInfo((EditorState)item.EditorState, (QuickInfoState)item.QuickInfoState);
                editorChanges[item.Name] = (EditorState)item.EditorState == EditorState.Enable;
            }

            Instance.GeneralChanged?.Invoke(new GeneralChangedEventArgs(changes));
            Instance.EditorOptionsChanged?.Invoke(new EditorChangedEventArgs(editorChanges));
        }

        public QuickInfoState GetDefaultQuickInfoState() => QuickInfoState.Disable;

        public EditorState GetDefaultEditorState() => EditorState.Enable;
    }
}