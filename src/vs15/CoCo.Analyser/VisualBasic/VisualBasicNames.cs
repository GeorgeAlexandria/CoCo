using System.Collections.Immutable;

namespace CoCo.Analyser.VisualBasic
{
    public static class VisualBasicNames
    {
        public const string LocalVariableName = "CoCo VB local variable name";
        public const string RangeVariableName = "CoCo VB range variable name";
        public const string FunctionVariableName = "CoCo VB function variable name";
        public const string FunctionName = "CoCo VB function name";
        public const string SubName = "CoCo VB sub name";
        public const string ExtensionMethodName = "CoCo VB extension method name";
        public const string SharedMethodName = "CoCo VB shared method name";
        public const string FieldName = "CoCo VB field name";
        public const string ConstantFieldName = "CoCo VB constant field name";
        public const string EnumFieldName = "CoCo VB enum field name";
        public const string ParameterName = "CoCo VB parameter name";
        public const string PropertyName = "CoCo VB property name";

        private static ImmutableArray<string> _all;

        public static ImmutableArray<string> All
        {
            get
            {
                if (!_all.IsDefaultOrEmpty) return _all;

                var builder = ImmutableArray.CreateBuilder<string>(10);
                builder.Add(LocalVariableName);
                builder.Add(RangeVariableName);
                builder.Add(FunctionVariableName);
                builder.Add(FunctionName);
                builder.Add(SubName);
                builder.Add(ExtensionMethodName);
                builder.Add(SharedMethodName);
                builder.Add(FieldName);
                builder.Add(ConstantFieldName);
                builder.Add(EnumFieldName);
                builder.Add(ParameterName);
                builder.Add(PropertyName);

                return _all = builder.ToImmutable();
            }
        }
    }
}