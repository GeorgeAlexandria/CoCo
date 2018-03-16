using System.Linq;

namespace CSharpIdentifiers.Declarations.Locals
{
    internal class RangeVariable
    {
        public void Create()
        {
            var result = from item in new[] { 1, 2, 3 }
                         let value = item * 2
                         select item;
        }
    }
}