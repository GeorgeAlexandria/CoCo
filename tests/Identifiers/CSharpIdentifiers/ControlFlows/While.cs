using System;

namespace CSharpIdentifiers.ControlFlows
{
    internal class While
    {
        public void Create(int arg)
        {
            while (arg < 10)
            {
                Console.WriteLine();
            }

            do
            {
                Console.WriteLine();
            } while (arg < 10);

            while (true)
            {
                break;
            }

            while (arg < 10)
            {
                continue;
            }

            while (true)
            {
                return;
            }
        }
    }
}