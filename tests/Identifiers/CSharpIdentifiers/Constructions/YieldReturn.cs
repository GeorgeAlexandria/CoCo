using System.Collections.Generic;

namespace CSharpIdentifiers.Constructions
{
    internal class YieldReturn
    {
        public IEnumerable<int> GetValues()
        {
            var first = 255;

            yield return first;
            foreach (var item in new[] { 1, 2 })
            {
                yield return item;
            }
        }
    }
}