using System.Collections.Generic;
using CoCo.Analyser.Classifications.CSharp;
using CoCo.Analyser.Classifications.VisualBasic;

namespace CoCo.Editor
{
    partial class ClassificationManager
    {
        private static readonly IReadOnlyDictionary<string, string> _nonIdentifierClassifications = new Dictionary<string, string>
        {
            [CSharpNames.ClassName] = "class name",
            [CSharpNames.StructureName] = "struct name",
            [CSharpNames.InterfaceName] = "interface name",
            [CSharpNames.EnumName] = "enum name",
            [CSharpNames.DelegateName] = "delegate name",
            [CSharpNames.TypeParameterName] = "type parameter name",

            [CSharpNames.ConstantFieldName] = "constant name",

            // NOTE: ctor name must be after MaxPriority("class name", "struct name"). 
            // Currently "struct name" has more priority
            [CSharpNames.ConstructorName] = "struct name",
            [CSharpNames.DestructorName] = "class name",
            [CSharpNames.EnumFieldName] = "enum member name",
            [CSharpNames.EventName] = "event name",
            [CSharpNames.ExtensionMethodName] = "extension method name",
            [CSharpNames.FieldName] = "field name",
            [CSharpNames.LabelName] = "label name",
            [CSharpNames.LocalMethodName] = "method name",
            [CSharpNames.LocalVariableName] = "local name",
            [CSharpNames.MethodName] = "method name",
            [CSharpNames.NamespaceName] = "namespace name",
            [CSharpNames.ParameterName] = "parameter name",
            [CSharpNames.PropertyName] = "property name",
            [CSharpNames.RangeVariableName] = "local name",
            [CSharpNames.StaticMethodName] = "method name",
            [CSharpNames.ControlFlowName] = "keyword - control",

            [VisualBasicNames.ClassName] = "class name",
            [VisualBasicNames.StructureName] = "struct name",
            [VisualBasicNames.InterfaceName] = "interface name",
            [VisualBasicNames.EnumName] = "enum name",
            [VisualBasicNames.DelegateName] = "delegate name",
            [VisualBasicNames.TypeParameterName] = "type parameter name",
            [VisualBasicNames.ModuleName] = "module name",

            [VisualBasicNames.ConstantFieldName] = "constant name",
            [VisualBasicNames.EnumFieldName] = "enum member name",
            [VisualBasicNames.EventName] = "event name",
            [VisualBasicNames.ExtensionMethodName] = "extension method name",
            [VisualBasicNames.FieldName] = "field name",
            [VisualBasicNames.FunctionName] = "method name",
            [VisualBasicNames.FunctionVariableName] = "local name",
            [VisualBasicNames.LocalVariableName] = "local name",
            [VisualBasicNames.NamespaceName] = "namespace name",
            [VisualBasicNames.ParameterName] = "parameter name",
            [VisualBasicNames.PropertyName] = "property name",
            [VisualBasicNames.RangeVariableName] = "local name",
            [VisualBasicNames.SharedMethodName] = "method name",
            [VisualBasicNames.StaticLocalVariableName] = "local name",
            [VisualBasicNames.SubName] = "method name",
            [VisualBasicNames.WithEventsPropertyName] = "property name",
        };
    }
}