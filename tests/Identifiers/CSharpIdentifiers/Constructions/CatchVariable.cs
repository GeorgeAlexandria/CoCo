using System;

namespace CSharpIdentifiers.Constructions
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
                Console.WriteLine(exception.Message);
                throw;
            }
        }
    }
}