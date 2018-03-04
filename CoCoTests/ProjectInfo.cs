using System.Collections.Generic;
using System.Collections.Immutable;

namespace CoCoTests
{
    internal class ProjectInfo
    {
        public ProjectInfo(ICollection<string> references)
        {
            var builder = ImmutableArray.CreateBuilder<string>(references.Count);
            foreach (var item in references)
            {
                builder.Add(item);
            }
            References = builder.MoveToImmutable();
        }

        public ImmutableArray<string> References { get; set; }
    }
}