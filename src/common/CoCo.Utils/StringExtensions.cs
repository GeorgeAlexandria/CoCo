using System;

namespace CoCo.Utils
{
    public static class StringExtensions
    {
        /// <summary>
        /// Check that <paramref name="name"/> equals string literal "true"
        /// </summary>
        public static bool IsTrue(this string name) => "true".EqualsNoCase(name);

        /// <summary>
        /// Check that <paramref name="name"/> equals string literal "false"
        /// </summary>
        public static bool IsFalse(this string name) => "false".EqualsNoCase(name);

        /// <summary>
        /// Compare two strings by <see cref="StringComparison.OrdinalIgnoreCase"/>
        /// </summary>
        public static bool EqualsNoCase(this string current, string other) =>
            string.Equals(current, other, StringComparison.OrdinalIgnoreCase);
    }
}