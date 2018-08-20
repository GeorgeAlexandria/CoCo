using System.Collections.Immutable;

namespace CoCo.Analyser
{
    public static class Names
    {
        public const string LocalVariableName = "CoCo local variable name";
        public const string RangeVariableName = "CoCo range variable name";
        public const string ParameterName = "CoCo parameter name";
        public const string NamespaceName = "CoCo namespace name";
        public const string ExtensionMethodName = "CoCo extension method name";
        public const string MethodName = "CoCo method name";
        public const string EventName = "CoCo event name";
        public const string PropertyName = "CoCo property name";
        public const string FieldName = "CoCo field name";
        public const string StaticMethodName = "CoCo static method name";
        public const string EnumFieldName = "CoCo enum field name";
        public const string AliasNamespaceName = "CoCo alias namespace name";
        public const string ConstructorName = "CoCo constructor method name";
        public const string LabelName = "CoCo label name";
        public const string LocalMethodName = "CoCo local method name";
        public const string ConstantFieldName = "CoCo constant field name";
        public const string DestructorName = "CoCo destructor method name";

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
                builder.Add(LocalMethodName);
                builder.Add(ConstantFieldName);
                builder.Add(DestructorName);
                return _all = builder.ToImmutable();
            }
        }
    }
}