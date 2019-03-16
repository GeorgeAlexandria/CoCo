using System.Collections.Generic;
using System.Collections.Immutable;

namespace CoCo.Analyser
{
    internal static class ImmutableExtensions
    {
        public static void InsertRange<T>(this ImmutableArray<T>.Builder builder, int index, IEnumerable<T> items)
        {
            if (index > builder.Count) return;

            if (index == builder.Count)
            {
                foreach (var item in items)
                {
                    builder.Add(item);
                }
                return;
            }

            foreach (var item in items)
            {
                builder.Insert(index++, item);
            }
        }
    }
}