using System;

namespace CSharpIdentifiers.Constructions
{
    class Throw
    {
        public void Create()
        {
            var exception = new ArgumentNullException();
            throw exception;
        }
    }
}