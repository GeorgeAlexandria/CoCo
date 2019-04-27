using System;
using System.Collections.Generic;

namespace CoCo.Analyser.Editor
{
    internal delegate void EditorChangedEventHandler(EditorChangedEventArgs args);

    public class EditorChangedEventArgs : EventArgs
    {
        public EditorChangedEventArgs(IReadOnlyDictionary<string, bool> changedOptions)
        {
            Changes = changedOptions;
        }

        public IReadOnlyDictionary<string, bool> Changes { get; }
    }
}