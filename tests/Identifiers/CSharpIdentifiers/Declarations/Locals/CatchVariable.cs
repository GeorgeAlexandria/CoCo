using System;

namespace CSharpIdentifiers.Declarations.Locals
{
    internal class CatchVariable
    {
        public void Create()
        {
            try
            {
                throw null;
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}