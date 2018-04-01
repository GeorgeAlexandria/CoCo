using System;

namespace CSharpIdentifiers.Constructions
{
    internal class SwitchPatternVariable
    {
        public void Create(object input)
        {
            switch (input)
            {
                case int value:
                    Console.WriteLine(value);
                    break;

                case string text:
                    Console.WriteLine(text);
                    break;
            }
        }
    }
}