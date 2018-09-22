using System.Collections.Immutable;
using CoCo.Analyser.CSharp;
using CoCo.Analyser.VisualBasic;

namespace CoCo.Analyser
{
    public static class Names
    {
        private static ImmutableDictionary<string, ImmutableArray<string>> _all;

        /// <summary>
        /// Contains all classification names grouped by language
        /// </summary>
        public static ImmutableDictionary<string, ImmutableArray<string>> All
        {
            get
            {
                if (!(_all is null)) return _all;

                return _all = ImmutableDictionary<string, ImmutableArray<string>>.Empty
                    .Add(Languages.CSharp, CSharpNames.All)
                    .Add(Languages.VisualBasic, VisualBasicNames.All);
            }
        }
    }
}