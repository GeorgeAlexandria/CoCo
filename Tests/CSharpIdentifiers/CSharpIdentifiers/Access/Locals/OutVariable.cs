using System;

namespace CSharpIdentifiers.Access.Locals
{
    internal class OutVariable
    {
        public void Create()
        {
            var reference = new WeakReference<int[]>(new[] { 1, 2, 9 });

            reference.TryGetTarget(out var variable);

            Console.WriteLine(variable[1]);
        }
    }
}