using System.Collections.Generic;

namespace CoCo.UI.Data
{
    public sealed class QuickInfoOption
    {
        public ICollection<QuickInfo> Languages { get; } = new List<QuickInfo>();
    }
}