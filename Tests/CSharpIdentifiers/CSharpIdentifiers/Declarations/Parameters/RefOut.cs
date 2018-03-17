namespace CSharpIdentifiers.Declarations.Parameters
{
    internal class RefOut
    {
        public void Create(out string arg1, ref string arg2)

        {
            arg1 = null;
        }
    }
}