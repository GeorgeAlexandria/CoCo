namespace CSharpIdentifiers.Declarations.Locals
{
    internal class PatternVariable
    {
        public void Create()
        {
            object variable = 5;

            if (variable is int value)
            {
            }
        }
    }
}