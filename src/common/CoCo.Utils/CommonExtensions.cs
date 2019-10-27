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
        public static void Deconstruct<T1, T2>(this Tuple<T1, T2> tuple, out T1 first, out T2 second)
        {
            first = tuple.Item1;
            second = tuple.Item2;
        }

        /// <summary>
        /// Deconstructs input <paramref name="tuple"/> to (<paramref name="first"/>, <paramref name="second"/>, <paramref name="third"/>)
        /// </summary>
        public static void Deconstruct<T1, T2, T3>(this Tuple<T1, T2, T3> tuple, out T1 first, out T2 second, out T3 third)
        {
            first = tuple.Item1;
            second = tuple.Item2;
            third = tuple.Item3;
        }
    }
}