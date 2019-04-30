namespace CSharpIdentifiers.Access.Locals
{
    internal class DynamicVariable
    {
        public void Create()
        {
            dynamic text = "sample";

            var length = text.Length;
        }
    }
}