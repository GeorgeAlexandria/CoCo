using Microsoft.FSharp.Core;

namespace CoCo.Analyser
{
    internal static class FSharpExtensions
    {
        public static bool IsSome<T>(this FSharpOption<T> option) => !(option is null);
    }
}