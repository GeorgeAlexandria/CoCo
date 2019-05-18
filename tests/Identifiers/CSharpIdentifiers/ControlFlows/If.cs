using System;

namespace CSharpIdentifiers.ControlFlows
{
    internal class If
    {
        public void Create(int arg)
        {
            if (arg < 10)
            {
                Console.WriteLine();
            }
            else if (arg < 25)
            {
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine();
            }
        }
    }
}