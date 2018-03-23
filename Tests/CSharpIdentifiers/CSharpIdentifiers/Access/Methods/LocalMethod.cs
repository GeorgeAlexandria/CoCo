using System;

namespace CSharpIdentifiers.Access.Methods
{
    internal class LocalMethod
    {
        public void Create()
        {
            bool IsPositive(int arg) => arg > 0;

            Console.WriteLine(IsPositive(25));
            Console.WriteLine(IsPositive(-25));
        }
    }
}