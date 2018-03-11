using System.Collections.Immutable;

namespace CoCo
{
    public static class Names
    {
        public const string LocalFieldName = "Local field name";
        public const string ParameterName = "Parameter name";
        public const string NamespaceName = "Namespace name";
        public const string ExtensionMethodName = "Extension method name";
        public const string MethodName = "Method name";
        public const string EventName = "Event name";
        public const string PropertyName = "Property name";
        public const string FieldName = "Field name";
        public const string StaticMethodName = "Static method name";
        public const string EnumFieldName = "Enum field name";
        public const string AliasNamespaceName = "Alias namespace name";
        public const string ConstructorMethodName = "Constructor method name";

        private static ImmutableArray<string> _all;

        public static ImmutableArray<string> All
        {
            get
            {
                if (!_all.IsDefaultOrEmpty) return _all;

                var builder = ImmutableArray.CreateBuilder<string>();
                builder.Add(LocalFieldName);
                builder.Add(ParameterName);
                builder.Add(NamespaceName);
                builder.Add(ExtensionMethodName);
                builder.Add(MethodName);
                builder.Add(EventName);
                builder.Add(PropertyName);
                builder.Add(FieldName);
                builder.Add(StaticMethodName);
                builder.Add(EnumFieldName);
                builder.Add(AliasNamespaceName);
                builder.Add(ConstructorMethodName);
                return _all = builder.ToImmutable();
            }
        }
    }
}