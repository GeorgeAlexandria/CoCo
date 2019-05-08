using System;
using System.Collections.Generic;

namespace CoCo
{
    public enum QuickInfoState
    {
        Disable = 0,
        Extend = 1,
        Override = 2,
    }

    public enum EditorState
    {
        Disable = 0,
        Enable = 1,
    }

    public struct GeneralInfo
    {
        public GeneralInfo(EditorState editorState, QuickInfoState quickInfoState)
        {
            EditorState = editorState;
            QuickInfoState = quickInfoState;
        }

        public EditorState EditorState { get; }

        public QuickInfoState QuickInfoState { get; }
    }

    public delegate void GeneralChangedEventHandler(GeneralChangedEventArgs args);

    public class GeneralChangedEventArgs : EventArgs
    {
        public GeneralChangedEventArgs(IReadOnlyDictionary<string, GeneralInfo> changedOptions)
        {
            Changes = changedOptions;
        }

        public IReadOnlyDictionary<string, GeneralInfo> Changes { get; }
    }
}