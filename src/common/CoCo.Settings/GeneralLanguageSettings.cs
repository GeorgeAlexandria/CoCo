using System.Diagnostics;

namespace CoCo.Settings
{
    [DebuggerDisplay("{Name}")]
    public struct GeneralLanguageSettings
    {
        public string Name { get; set; }

        public int? QuickInfoState { get; set; }

        public int? EditorState { get; set; }
    }
}