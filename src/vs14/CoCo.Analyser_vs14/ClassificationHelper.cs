namespace CoCo.Analyser
{
    public static class ClassificationHelper
    {
        public static bool IsSupportedClassification(string classification) =>
            classification == "identifier";
    }
}