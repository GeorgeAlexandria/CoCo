using System;

namespace CSharpIdentifiers.Access.Member
{
    internal class Property
    {
        public int Value { get; set; } = 27;

        public void Create()
        {
            if (Value < 100)
            {
                Value += 100;
                Console.WriteLine(Value);
            }
        }
    }
}