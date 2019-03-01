namespace CoCo.Analyser.QuickInfo
{
    /// <summary>
    /// Reppresents classification kind and the real part of text
    /// </summary>
    public struct TaggedText
    {
        public string Tag { get; }

        public string Text { get; }

        public TaggedText(string tag, string text)
        {
            Tag = tag;
            Text = text;
        }

        public bool IsDefault => Tag is null || Text is null;
    }
}