using System.Collections.Generic;

namespace System.Linq
{
    public static class LinqExtensions
    {
        /// <summary>
        ///  Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>
        /// </summary>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> comparer = null) =>
            comparer is null ? new HashSet<T>(enumerable) : new HashSet<T>(enumerable, comparer);
    }
}