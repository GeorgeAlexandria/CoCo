namespace CSharpIdentifiers.Declarations.Parameters
{
    internal class RefOutIn
    {
        public void Create(out string arg1, ref string arg2, in int arg3)
        {
            arg1 = null;
        }
    }
}