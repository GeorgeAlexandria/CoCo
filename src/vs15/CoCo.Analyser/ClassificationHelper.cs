namespace CoCo.Analyser
{
    public static class ClassificationHelper
    {
        public static bool IsSupportedClassification(string classification) =>
            classification == "identifier" || classification == "extension method name" || classification == "field name" ||
            classification == "property name" || classification == "method name" || classification == "local name" ||
            classification == "parameter name" || classification == "event name" || classification == "enum member name" ||
            classification == "constant name";
    }
}