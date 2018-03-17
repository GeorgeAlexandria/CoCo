namespace CSharpIdentifiers.Declarations.Locals
{
    internal class UsingVariable
    {
        public void Create()
        {
            using (var stream = new System.IO.StreamWriter("path"))
            {
            }
        }
    }
}