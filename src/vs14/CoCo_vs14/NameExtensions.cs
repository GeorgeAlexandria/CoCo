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
            if (name.Length < 6) throw new ArgumentException("Name must contains more than 6 characters");

            var builder = new StringBuilder();

            // NOTE: Upper the first char
            builder.Append(char.ToUpper(name[0]));

            // NOTE: append all remaining characters excluding the " name" suffix
            for (int i = 1; i < name.Length - 5; ++i)
            {
                builder.Append(name[i]);
            }
            return builder.ToString();
        }
    }
}