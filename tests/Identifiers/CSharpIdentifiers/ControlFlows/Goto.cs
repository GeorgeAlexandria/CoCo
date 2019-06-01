using System;

namespace CSharpIdentifiers.ControlFlows
{
    internal class Goto
    {
        public void Create()
        {
            goto label;

            label: Console.WriteLine();
        }
    }
}