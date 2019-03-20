using System;
using System.Collections.Generic;

namespace CoCo.QuickInfo
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
}