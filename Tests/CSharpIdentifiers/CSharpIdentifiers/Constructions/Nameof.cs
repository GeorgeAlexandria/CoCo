using System;

namespace CSharpIdentifiers.Constructions
{
    internal class Nameof
    {
        private int value;

        public void Create()
        {
            var variable = 5;
            var name = nameof(variable);
            var enumField = nameof(ConsoleKey.Backspace);
            var property = nameof(Console.BufferHeight);
            var field = nameof(value);
        }
    }
}