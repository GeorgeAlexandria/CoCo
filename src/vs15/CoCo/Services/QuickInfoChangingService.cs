using System;
using System.Collections.Generic;
using CoCo.UI.Data;

namespace CoCo.Services
{
    public enum QuickInfoState
    {
        Disable = 0,
        Extend = 1,
        Override = 2,
    }

    public delegate void QuickInfoChangedEventHandler(QuickInfoChangedEventArgs args);

    public class QuickInfoChangedEventArgs : EventArgs
    {
        public QuickInfoChangedEventArgs(IReadOnlyDictionary<string, QuickInfoState> changedOptions)
        {
            Changes = changedOptions;
        }

        public IReadOnlyDictionary<string, QuickInfoState> Changes { get; }
    }

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
    }
}