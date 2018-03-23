namespace CSharpIdentifiers.Access.Parameters
{
    internal class RefInOut
    {
        public void Create(out int arg1, ref int arg2, in int arg3)
        {
            arg1 = arg2;
            arg1 = arg3;
        }
    }
}