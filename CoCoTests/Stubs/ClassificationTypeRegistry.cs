using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCoTests.Stubs
{
    internal class ClassificationTypeRegistry : IClassificationTypeRegistryService
    {
        public IClassificationType CreateClassificationType(string type, IEnumerable<IClassificationType> baseTypes) =>
            new ClassificationType(type).AddBaseTypes(baseTypes);

        public IClassificationType CreateTransientClassificationType(IEnumerable<IClassificationType> baseTypes) => throw new NotImplementedException();

        public IClassificationType CreateTransientClassificationType(params IClassificationType[] baseTypes) => throw new NotImplementedException();

        public IClassificationType GetClassificationType(string type) => new ClassificationType(type);
    }
}