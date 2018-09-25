using System.Collections.Immutable;

namespace CoCo.Analyser.CSharp
{
    public static class CSharpNames
    {
        public const string LocalVariableName = "CoCo csharp local variable name";
        public const string RangeVariableName = "CoCo csharp range variable name";
        public const string ParameterName = "CoCo csharp parameter name";
        public const string NamespaceName = "CoCo csharp namespace name";
        public const string ExtensionMethodName = "CoCo csharp extension method name";
        public const string MethodName = "CoCo csharp method name";
        public const string EventName = "CoCo csharp event name";
        public const string PropertyName = "CoCo csharp property name";
        public const string FieldName = "CoCo csharp field name";
        public const string StaticMethodName = "CoCo csharp static method name";
        public const string EnumFieldName = "CoCo csharp enum field name";
        public const string AliasNamespaceName = "CoCo csharp alias namespace name";
        public const string ConstructorName = "CoCo csharp constructor method name";
        public const string LabelName = "CoCo csharp label name";
        public const string ConstantFieldName = "CoCo csharp constant field name";
        public const string DestructorName = "CoCo csharp destructor method name";

        private static ImmutableArray<string> _all;

        public static ImmutableArray<string> All
        {
            get
            {
                if (!_all.IsDefaultOrEmpty) return _all;

                var builder = ImmutableArray.CreateBuilder<string>(17);
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