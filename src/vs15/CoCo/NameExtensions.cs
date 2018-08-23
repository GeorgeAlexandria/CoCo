using System;
using System.Text;

namespace CoCo
{
    public static class NameExtensions
    {
        /// <summary>
        /// Converts the input <paramref name="name"/> to corresponding display name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToDisplayName(this string name)
        {
            if (name.Length < 11) throw new ArgumentException("Name must contains more than 11 characters");

            var builder = new StringBuilder();

            // NOTE: skip "CoCo " prefix and upper the first char
            builder.Append(char.ToUpper(name[5]));

            // NOTE: append all remaining characters excluding the " name" suffix
            for (int i = 6; i < name.Length - 5; ++i)
            {
                builder.Append(name[i]);
            }
            return builder.ToString();
        }
    }
}