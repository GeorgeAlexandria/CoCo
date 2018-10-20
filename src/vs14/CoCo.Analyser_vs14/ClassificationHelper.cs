namespace CoCo.Analyser
{
    public static class ClassificationHelper
    {
        public static bool IsSupportedClassification(string classification) =>
            classification == "identifier" || classification == "class name" || classification == "delegate name" ||
            classification == "enum name" || classification == "interface name" || classification == "module name" ||
            classification == "struct name" || classification == "type parameter name";
    }
}