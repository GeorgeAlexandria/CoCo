using System.Collections.Generic;
using CoCo.UI.Data;

namespace CoCo.QuickInfo
{
    internal sealed class QuickInfoChangingService
    {
        public static readonly QuickInfoChangingService Instance = new QuickInfoChangingService();

        public event QuickInfoChangedEventHandler QuickInfoChanged;

        private QuickInfoChangingService()
        {
        }

        public static void SetQuickInfoOptions(QuickInfoOption option)
        {
            var changes = new Dictionary<string, QuickInfoState>();
            foreach (var item in option.Languages)
            {
                changes[item.Language] = (QuickInfoState)item.State;
            }

            Instance.QuickInfoChanged?.Invoke(new QuickInfoChangedEventArgs(changes));
        }

        public QuickInfoState GetDefaultValue() => QuickInfoState.Disable;
    }
}