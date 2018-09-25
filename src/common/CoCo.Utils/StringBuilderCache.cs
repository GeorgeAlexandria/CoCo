using System;
using System.Text;

namespace CoCo.Utils
{
    // TODO: take account capacity?
    public static class StringBuilderCache
    {
        [ThreadStatic]
        private static StringBuilder _cache;

        public static StringBuilder Acquire()
        {
            var builder = _cache;
            if (builder is null) return new StringBuilder();

            _cache = null;
            builder.Clear();
            return builder;
        }

        public static string Release(StringBuilder builder)
        {
            var str = builder.ToString();
            _cache = builder;
            return str;
        }
    }
}