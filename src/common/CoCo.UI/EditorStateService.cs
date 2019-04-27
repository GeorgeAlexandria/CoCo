using System.Collections.Generic;

namespace CoCo.UI
{
    public static class EditorStateService
    {
        public static readonly Dictionary<string, int> SupportedStateByNames = new Dictionary<string, int>
        {
            ["Disable"] = 0,
            ["Enable"] = 1,
        };

        public static readonly Dictionary<int, string> SupportedState = new Dictionary<int, string>
        {
            [0] = "Disable",
            [1] = "Enable",
        };
    }
}