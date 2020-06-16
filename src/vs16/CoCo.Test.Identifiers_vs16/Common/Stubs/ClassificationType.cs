using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Test.Identifiers.Common
{
    internal class ClassificationType : IClassificationType
    {
        private readonly List<IClassificationType> _baseClassifications = new List<IClassificationType>();

        public ClassificationType(string classification)
        {
            Classification = classification;
        }

        public string Classification { get; }

        public bool IsOfType(string type)
        {
            if (string.Compare(Classification, type, System.StringComparison.OrdinalIgnoreCase) == 0) return true;

            foreach (var classification in _baseClassifications)
            {
                if (classification.IsOfType(type)) return true;
            }

            return false;
        }

        public IClassificationType AddBaseTypes(IEnumerable<IClassificationType> baseTypes)
        {
            _baseClassifications.AddRange(baseTypes);
            return this;
        }

        public IEnumerable<IClassificationType> BaseTypes => _baseClassifications;
    }
}