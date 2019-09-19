using System;
using System.Runtime.CompilerServices;

namespace CoCo.Utils
{
    public static class CommonExtensions
    {
        public static bool IsNumber(this object value) => !(value is null) &&
            value is sbyte || value is byte || value is short || value is ushort ||
            value is int || value is uint || value is long || value is ulong ||
            value is float || value is double || value is decimal;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull<T>(this T obj) where T : class => !(obj is null);

        /// <summary>
        /// Deconstructs input <paramref name="tuple"/> to (<paramref name="first"/>, <paramref name="second"/>)
        /// </summary>
        public static void Deconstruct<K, V>(this Tuple<K, V> tuple, out K first, out V second)
        {
            first = tuple.Item1;
            second = tuple.Item2;
        }
    }
}