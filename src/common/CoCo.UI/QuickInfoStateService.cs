using System.Collections.Generic;

namespace CoCo.UI
{
    public static class QuickInfoStateService
    {
        public static readonly Dictionary<string, int> SupportedStateByNames = new Dictionary<string, int>
        {
            ["Disable"] = 0,
            ["Extend"] = 1,
            ["Override"] = 2,
        };

        public static readonly Dictionary<int, string> SupportedState = new Dictionary<int, string>
        {
            [0] = "Disable",
            [1] = "Extend",
            [2] = "Override",
        };
    }
}