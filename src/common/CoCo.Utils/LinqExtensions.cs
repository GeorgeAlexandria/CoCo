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

        /// <summary>
        ///  Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/> using <paramref name="itemSelector"/>
        /// </summary>
        /// <typeparam name="Tin">The type of source's elements</typeparam>
        /// <typeparam name="Tout">The type of destination's elements</typeparam>
        public static HashSet<Tout> ToHashSet<Tin, Tout>(
            this IEnumerable<Tin> enumerable,
            Func<Tin, Tout> itemSelector,
            IEqualityComparer<Tout> comparer = null) =>
            comparer is null
            ? new HashSet<Tout>(enumerable.Select(itemSelector)) :
            new HashSet<Tout>(enumerable.Select(itemSelector), comparer);
    }
}