namespace CSharpIdentifiers.Access.Types
{
    internal class TypeParameter<TValue> where TValue : new()
    {
        private TValue Value => new TValue();
    }
}