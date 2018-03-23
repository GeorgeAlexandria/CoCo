using System;

namespace CSharpIdentifiers.Access.Parameters
{
    internal class Delegate
    {
        public void Create()
        {
            Func<int, bool> isPositive = delegate (int arg)
            {
                return arg > 0;
            };
            Console.WriteLine(isPositive(25));
        }
    }
}