namespace CSharpIdentifiers.Access.Member
{
    internal class Field
    {
        private int count = 25;

        public void Create()
        {
            if (count > 10)
            {
                count *= 10;
                System.Console.WriteLine(count);
            }
        }
    }
}