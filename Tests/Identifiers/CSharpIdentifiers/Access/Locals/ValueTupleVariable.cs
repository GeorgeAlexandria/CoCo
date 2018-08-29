namespace CSharpIdentifiers.Access.Locals
{
    internal class ValueTupleVariable
    {
        public void Create()
        {
            var vector = (1, 2, 3);
            var value = vector.Item1 + vector.Item2;
        }
    }
}