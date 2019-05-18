using System;

namespace CSharpIdentifiers.ControlFlows
{
    internal class Throw
    {
        public void Create()
        {
            throw new Exception();
        }

        public void Create2()
        {
            throw null;
        }

        public void Create3()
        {
            try
            {
            }
            catch
            {
                throw;
            }
        }
    }
}