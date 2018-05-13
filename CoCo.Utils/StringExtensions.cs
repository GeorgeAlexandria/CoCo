using System;

namespace CoCo.Utils
{
    public static class StringExtensions
    {
        /// <summary>
        /// Check that <paramref name="name"/> equals string literal "true"
        /// </summary>
        public static bool IsTrue(this string name) => "true".Equals(name, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Compare two strings by <see cref="StringComparison.OrdinalIgnoreCase"/>
        /// </summary>
        public static bool EqualsNoCase(this string current, string other) =>
            string.Equals(current, other, StringComparison.OrdinalIgnoreCase);
    }
}