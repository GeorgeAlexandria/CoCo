using System;

namespace CSharpIdentifiers.ControlFlows
{
    internal class For
    {
        public void Create()
        {
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine();
            }

            for (int i = 0; i < 20; i++)
            {
                continue;
            }

            for (int i = 0; i < 20; i++)
            {
                break;
            }

            for (int i = 0; i < 20; i++)
            {
                return;
            }
        }
    }
}