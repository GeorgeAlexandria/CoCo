using System.Collections.Generic;

namespace CoCo.Settings
{
    public struct EditorSettings
    {
        public ICollection<EditorLanguageSettings> Languages { get; set; }
    }
}