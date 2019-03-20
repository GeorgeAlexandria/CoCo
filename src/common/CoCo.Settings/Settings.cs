namespace CoCo.Settings
{
    public struct Settings
    {
        public EditorSettings Editor { get; set; }

        public string EditorPath { get; set; }

        public QuickInfoSettings QuickInfo { get; set; }

        public string QuickInfoPath { get; set; }
    }
}