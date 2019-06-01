namespace CSharpIdentifiers.ControlFlows
{
    internal class Foreach
    {
        public void Create()
        {
            var a = new int[] { 1, 2, 5 };
            foreach (var item in a)
            {
            }

            foreach (var item in a)
            {
                continue;
            }

            foreach (var item in a)
            {
                break;
            }

            foreach (var item in a)
            {
                return;
            }
        }
    }
}