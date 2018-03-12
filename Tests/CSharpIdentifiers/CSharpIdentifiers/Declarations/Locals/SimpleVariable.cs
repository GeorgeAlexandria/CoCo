namespace CSharpIdentifiers.Declarations.Locals
{
    internal class SimpleVariable
    {
        public void Create()
        {
            var field1 = 5;
            // NOTE: check multi declaration
            int variable1, variable2 = 26;
        }
    }
}