using System.Collections.Generic;
using System.Linq;

namespace CSharpIdentifiers.Access.Methods
{
    internal class ExtensionMethod
    {
        public void Create()
        {
            var collection = new List<int> { { 10 }, { 2 }, { 3 } };
            var selection = collection.Where(x => x > 2).ToList();
        }
    }
}