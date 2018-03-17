namespace CSharpIdentifiers.Declarations.Locals
{
    internal class ValueTupleVariable
    {
        public void Create()
        {
            var (arg1, arg2) = (5, "asd");
        }
    }
}