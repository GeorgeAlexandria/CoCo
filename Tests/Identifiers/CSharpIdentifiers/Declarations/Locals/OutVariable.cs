using System;

namespace CSharpIdentifiers.Declarations.Locals
{
    internal class OutVariable
    {
        public void Create()
        {
            var reference = new WeakReference<object>(5);

            reference.TryGetTarget(out var variable);
        }
    }
}