using System.Diagnostics;

namespace CoCo.Settings
{
    [DebuggerDisplay("{Name}")]
    public struct QuickInfoLanguageSettings
    {
        public string Name { get; set; }

        public int? State { get; set; }
    }
}