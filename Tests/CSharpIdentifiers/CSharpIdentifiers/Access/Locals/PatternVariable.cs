using System;

namespace CSharpIdentifiers.Access.Locals
{
    class PatternVariable
    {
        public void Create(object input)
        {
            var res = input is string text && text.Length > 5;
            if (!res)
            {
                Console.WriteLine();
            }
        }
    }
}