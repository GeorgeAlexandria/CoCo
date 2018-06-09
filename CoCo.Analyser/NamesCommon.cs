using System.Collections.Immutable;

namespace CoCo.Analyser
{
    public static partial class Names
    {
        public const string LocalVariableName = "CoCo_Local variable name";
        public const string RangeVariableName = "CoCo_Range variable name";
        public const string ParameterName = "CoCo_Parameter name";
        public const string NamespaceName = "CoCo_Namespace name";
        public const string ExtensionMethodName = "CoCo_Extension method name";
        public const string MethodName = "CoCo_Method name";
        public const string EventName = "CoCo_Event name";
        public const string PropertyName = "CoCo_Property name";
        public const string FieldName = "CoCo_Field name";
        public const string StaticMethodName = "CoCo_Static method name";
        public const string EnumFieldName = "CoCo_Enum field name";
        public const string AliasNamespaceName = "CoCo_Alias namespace name";
        public const string ConstructorName = "CoCo_Constructor method name";
        public const string LabelName = "CoCo_Label name";

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
                builder.AddNames();
                return _all = builder.ToImmutable();
            }
        }
    }
}