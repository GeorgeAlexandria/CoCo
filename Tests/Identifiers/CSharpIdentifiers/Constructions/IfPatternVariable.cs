namespace CSharpIdentifiers.Constructions
{
    internal class IfPatternVariable
    {
        public void Create()
        {
            object variable = "";
            if (variable is string value)
            {
                int.TryParse(value, out var _);
            }
        }
    }
}