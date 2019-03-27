namespace CoCo.Utils
{
    public static class CommonExtensions
    {
        public static bool IsNumber(this object value) => !(value is null) &&
            value is sbyte || value is byte || value is short || value is ushort ||
            value is int || value is uint || value is long || value is ulong ||
            value is float || value is double || value is decimal;
    }
}