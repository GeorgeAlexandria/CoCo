using System;

namespace CSharpIdentifiers.Access.Parameters
{
    internal class Optional
    {
        public void Create(string text = "")
        {
            Console.WriteLine(text);
        }
    }
}