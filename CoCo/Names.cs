using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace CoCo
{
    public static partial class Names
    {
        public const string LocalMethodName = "Local method name";

        /// <summary>
        /// Adds names that are supported for the corresponding version of visual studio
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddNames(this ImmutableArray<string>.Builder builder)
        {
            builder.Add(LocalMethodName);
        }
    }
}