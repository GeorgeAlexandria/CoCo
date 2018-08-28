using System.Collections.Immutable;

namespace CoCo.Analyser
{
    public static class VisualBasicNames
    {
        private static ImmutableArray<string> _all;

        public static ImmutableArray<string> All
        {
            get
            {
                if (!_all.IsDefaultOrEmpty) return _all;

                var builder = ImmutableArray.CreateBuilder<string>(10);

                return _all = builder.ToImmutable();
            }
        }
    }
}