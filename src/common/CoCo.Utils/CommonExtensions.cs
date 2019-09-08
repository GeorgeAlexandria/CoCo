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
    }
}