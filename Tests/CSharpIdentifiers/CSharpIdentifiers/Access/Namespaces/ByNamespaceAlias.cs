// Check the same alias name as the last level namespace
using Collections = System.Collections;
using Generics = System.Collections.Generic;

// Check the alias for the first level namespace
using Sys = System;

namespace CSharpIdentifiers.Access.Namespace
{
    internal class ByNamespaceAlias
    {
        public void Create()
        {
            var list = new Generics.List<int>();
            var list2 = new Collections.Generic.List<int>();
            var error = new Sys.ArgumentOutOfRangeException();
        }
    }
}