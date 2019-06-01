using System.Collections.Generic;

namespace CSharpIdentifiers.ControlFlows
{
    internal class Iterator
    {
        public IEnumerable<int> Create()
        {
            yield return 5;

            yield return 6;

            yield break;
        }
    }
}