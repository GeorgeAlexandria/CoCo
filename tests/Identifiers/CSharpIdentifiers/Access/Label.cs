namespace CSharpIdentifiers.Access
{
    internal class Label
    {
        public void Create(int count)
        {
            if (count == 5)
            {
                goto mark;
            }
            else if (count > 6)
            {
                System.Console.WriteLine();
            }

            count += 2;
            mark: count++;
        }
    }
}