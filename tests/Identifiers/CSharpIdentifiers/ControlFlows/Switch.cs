using System;

namespace CSharpIdentifiers.ControlFlows
{
    internal class Switch
    {
        public void Create(int arg)
        {
            switch (arg)
            {
                case 1:
                    break;

                case 10:
                    return;

                case 20:
                    break;

                default:
                    Console.WriteLine();
                    break;
            }
        }
    }
}