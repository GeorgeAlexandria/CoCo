using System.Collections.Immutable;

namespace CoCo.Analyser
{
    public static class Names
    {
        public const string LocalVariableName = "Local variable name";
        public const string RangeVariableName = "Range variable name";
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
        public const string ConstructorName = "Constructor method name";
        public const string LabelName = "Label name";
        public const string ConstantFieldName = "Constant field name";
        public const string DestructorName = "Destructor method name";

        private static ImmutableArray<string> _all;

        public static ImmutableArray<string> All
        {
            get
            {
                if (!_all.IsDefaultOrEmpty) return _all;

                var builder = ImmutableArray.CreateBuilder<string>();
                builder.Add(LocalVariableName);
                builder.Add(ParameterName);
                builder.Add(NamespaceName);
                builder.Add(ExtensionMethodName);
                builder.Add(MethodName);
                builder.Add(RangeVariableName);
                builder.Add(EventName);
                builder.Add(PropertyName);
                builder.Add(FieldName);
                builder.Add(StaticMethodName);
                builder.Add(EnumFieldName);
                builder.Add(AliasNamespaceName);
                builder.Add(ConstructorName);
                builder.Add(LabelName);
                builder.Add(ConstantFieldName);
                builder.Add(DestructorName);
                return _all = builder.ToImmutable();
            }
        }
    }
}