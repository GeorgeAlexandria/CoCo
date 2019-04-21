using System.Collections.Immutable;

namespace CoCo.Analyser.Classifications.VisualBasic
{
    public static class VisualBasicNames
    {
        public const string LocalVariableName = "CoCo visual basic local variable name";
        public const string RangeVariableName = "CoCo visual basic range variable name";
        public const string FunctionVariableName = "CoCo visual basic function variable name";
        public const string FunctionName = "CoCo visual basic function method name";
        public const string SubName = "CoCo visual basic sub method name";
        public const string ExtensionMethodName = "CoCo visual basic extension method name";
        public const string SharedMethodName = "CoCo visual basic shared method name";
        public const string FieldName = "CoCo visual basic field name";
        public const string ConstantFieldName = "CoCo visual basic constant field name";
        public const string EnumFieldName = "CoCo visual basic enum field name";
        public const string ParameterName = "CoCo visual basic parameter name";
        public const string PropertyName = "CoCo visual basic property name";
        public const string WithEventsPropertyName = "CoCo visual basic with events property name";
        public const string NamespaceName = "CoCo visual basic namespace name";
        public const string AliasNamespaceName = "CoCo visual basic alias namespace name";
        public const string StaticLocalVariableName = "CoCo visual basic static local variable name";
        public const string EventName = "CoCo visual basic event name";
        public const string ClassName = "CoCo visual basic class name";
        public const string StructureName = "CoCo visual basic structure name";
        public const string ModuleName = "CoCo visual basic module name";
        public const string InterfaceName = "CoCo visual basic interface name";
        public const string DelegateName = "CoCo visual basic delegate name";
        public const string EnumName = "CoCo visual basic enum name";
        public const string TypeParameterName = "CoCo visual basic type parameter name";

        private static ImmutableArray<string> _all;

        public static ImmutableArray<string> All
        {
            get
            {
                if (!_all.IsDefaultOrEmpty) return _all;

                var builder = ImmutableArray.CreateBuilder<string>(17);
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
                builder.Add(WithEventsPropertyName);
                builder.Add(NamespaceName);
                builder.Add(AliasNamespaceName);
                builder.Add(StaticLocalVariableName);
                builder.Add(EventName);
                builder.Add(ClassName);
                builder.Add(StructureName);
                builder.Add(ModuleName);
                builder.Add(InterfaceName);
                builder.Add(DelegateName);
                builder.Add(EnumName);
                builder.Add(TypeParameterName);

                return _all = builder.TryMoveToImmutable();
            }
        }
    }
}