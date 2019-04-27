namespace CoCo.Analyser.Classifications
{
    public static class ClassificationHelper
    {
        public static bool IsSupportedClassification(string classification) =>
            classification == "identifier" || classification == "extension method name" || classification == "field name" ||
            classification == "property name" || classification == "method name" || classification == "local name" ||
            classification == "parameter name" || classification == "event name" || classification == "enum member name" ||
            classification == "constant name" || classification == "class name" || classification == "delegate name" ||
            classification == "enum name" || classification == "interface name" || classification == "module name" ||
            classification == "struct name" || classification == "type parameter name";
    }
}