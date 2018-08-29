using System;

namespace CSharpIdentifiers.Declarations.Parameters
{
    internal class DelegateParameter
    {
        public Func<int, bool> @Delegate = delegate (int value) { return true; };
    }
}