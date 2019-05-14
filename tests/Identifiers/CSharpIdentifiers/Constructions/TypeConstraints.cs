using System;
using System.Collections.Generic;

namespace CSharpIdentifiers.Constructions
{
    internal class TypeConstraints1<T> where T : IEnumerable<IEnumerable<int>>
    {
    }

    internal class TypeConstraints2<T> where T : Exception
    {
    }

    internal class TypeConstraints3<T> where T : System.Exception
    {
    }
}