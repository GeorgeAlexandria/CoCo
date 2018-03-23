using System;

namespace CSharpIdentifiers.Access.Parameters
{
    internal class Variable
    {
        public void Create(params string[] paths)
        {
            Console.WriteLine(paths[0]);
        }
    }
}