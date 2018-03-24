using System;

namespace CSharpIdentifiers.Constructions
{
    internal class ForForeachControlVariable
    {
        public void Create()
        {
            for (int field = 0; field < 10; field++)
            {
                field += field > 5 ? 1 : 0;
            }

            foreach (var item in new[] { 1, 2 })
            {
                Console.WriteLine(item > 25 ? item : 0);
            }
        }
    }
}