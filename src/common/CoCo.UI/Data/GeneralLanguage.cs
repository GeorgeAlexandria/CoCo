using System.Diagnostics;

namespace CoCo.UI.Data
{
    [DebuggerDisplay("{Name}")]
    public sealed class GeneralLanguage
    {
        public GeneralLanguage(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public int QuickInfoState { get; set; }

        public int EditorState { get; set; }
    }
}