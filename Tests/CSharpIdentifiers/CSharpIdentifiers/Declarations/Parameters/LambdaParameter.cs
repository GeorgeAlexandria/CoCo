using System;

namespace CSharpIdentifiers.Declarations.Parameters
{
    internal class LambdaParameter
    {
        public Func<int, bool> Func => (int value) => true;
    }
}