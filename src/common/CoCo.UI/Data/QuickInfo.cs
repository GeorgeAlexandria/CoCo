namespace CoCo.UI.Data
{
    public sealed class QuickInfo
    {
        public QuickInfo(string language)
        {
            Language = language;
        }

        public string Language { get; }

        public int State { get; set; }
    }
}