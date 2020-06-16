using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Utilities;

namespace CoCo.Test.Identifiers.Common
{
    internal class ContentType : IContentType
    {
        private readonly string _contentType;

        private readonly List<IContentType> _baseTypes = new List<IContentType>();

        public ContentType(string contentType)
        {
            _contentType = contentType;
        }

        public string TypeName => _contentType;

        public string DisplayName => _contentType;

        public IEnumerable<IContentType> BaseTypes => _baseTypes;

        public bool IsOfType(string type)
        {
            if (string.Compare(_contentType, type, StringComparison.OrdinalIgnoreCase) == 0) return true;

            foreach (var baseType in _baseTypes)
            {
                if (baseType.IsOfType(type)) return true;
            }

            return false;
        }
    }
}