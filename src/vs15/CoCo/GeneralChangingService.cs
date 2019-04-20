using System.Collections.Generic;
using CoCo.UI.Data;

namespace CoCo
{
    internal sealed class GeneralChangingService
    {
        public static readonly GeneralChangingService Instance = new GeneralChangingService();

        public event GeneralChangedEventHandler GeneralChanged;

        private GeneralChangingService()
        {
        }

        public static void SetGeneralOptions(GeneralData option)
        {
            var changes = new Dictionary<string, GeneralInfo>();
            foreach (var item in option.Languages)
            {
                changes[item.Name] = new GeneralInfo((EditorState)item.EditorState, (QuickInfoState)item.QuickInfoState);
            }

            Instance.GeneralChanged?.Invoke(new GeneralChangedEventArgs(changes));
        }

        public QuickInfoState GetDefaultQuickInfoState() => QuickInfoState.Disable;

        public EditorState GetDefaultEditorState() => EditorState.Enable;
    }
}