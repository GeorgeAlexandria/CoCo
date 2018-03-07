using System.Collections.Generic;
using System.Collections.Immutable;

namespace CoCoTests
{
    // TODO: struct?
    internal class ProjectInfo
    {
        public ProjectInfo(ICollection<string> references, ICollection<ProjectInfo> projectReferences)
        {
            var builder = ImmutableArray.CreateBuilder<string>(references.Count);
            foreach (var item in references)
            {
                builder.Add(item);
            }
            References = builder.MoveToImmutable();

            var builderProjects = ImmutableArray.CreateBuilder<ProjectInfo>(projectReferences.Count);
            foreach (var item in projectReferences)
            {
                builderProjects.Add(item);
            }
            ProjectReferences = builderProjects.MoveToImmutable();
        }

        public ImmutableArray<string> References { get; set; }

        public ImmutableArray<ProjectInfo> ProjectReferences { get; set; }
    }
}