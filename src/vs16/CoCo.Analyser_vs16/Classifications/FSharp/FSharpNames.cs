using System.Collections.Immutable;

namespace CoCo.Analyser.Classifications.FSharp
{
    public static class FSharpNames
    {
        public const string LocalBindingValueName = "CoCo fsharp local binding value name";
        public const string LocalVariableName = "CoCo fsharp local variable name";
        public const string ParameterName = "CoCo fsharp parameter name";
        public const string NamespaceName = "CoCo fsharp namespace name";
        public const string UnionName = "CoCo fsharp union name";
        public const string RecordName = "CoCo fsharp record name";
        public const string TypeParameterName = "CoCo fsharp type parameter name";
        public const string ClassName = "CoCo fsharp class name";
        public const string StructureName = "CoCo fsharp structure name";
        public const string ModuleName = "CoCo fsharp module name";
        public const string EnumName = "CoCo fsharp enum name";
        public const string InterfaceName = "CoCo fsharp interface name";
        public const string PropertyName = "CoCo fsharp property name";
        public const string FieldName = "CoCo fsharp field name";
        public const string SelfIdentifierName = "CoCo fsharp self identifier name";
        public const string EnumFieldName = "CoCo fsharp enum field name";

        private static ImmutableArray<string> _all;

        public static ImmutableArray<string> All
        {
            get
            {
                if (!_all.IsDefaultOrEmpty) return _all;

                var builder = ImmutableArray.CreateBuilder<string>();
                builder.Add(LocalBindingValueName);
                builder.Add(LocalVariableName);
                builder.Add(ParameterName);
                builder.Add(NamespaceName);
                builder.Add(UnionName);
                builder.Add(RecordName);
                builder.Add(TypeParameterName);
                builder.Add(ClassName);
                builder.Add(StructureName);
                builder.Add(ModuleName);
                builder.Add(InterfaceName);
                builder.Add(EnumName);
                builder.Add(PropertyName);
                builder.Add(FieldName);
                builder.Add(SelfIdentifierName);
                builder.Add(EnumFieldName);

                return _all = builder.TryMoveToImmutable();
            }
        }
    }
}