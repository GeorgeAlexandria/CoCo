namespace CSharpIdentifiers.Access.Members
{
    internal class ValueTupleFields
    {
        public void Create()
        {
            var tuple = (arg1: 1, arg2: 6);

            var value = tuple.arg1 + tuple.arg2;
        }
    }
}