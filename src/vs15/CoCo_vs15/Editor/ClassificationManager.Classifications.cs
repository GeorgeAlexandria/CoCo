using System.Collections.Generic;
using CoCo.Analyser.Classifications.CSharp;
using CoCo.Analyser.Classifications.VisualBasic;

namespace CoCo.Editor
{
    partial class ClassificationManager
    {
        private static IReadOnlyDictionary<string, string> NonIdentifierClassifications { get; } = new Dictionary<string, string>
        {
            [CSharpNames.ClassName] = "class name",
            [CSharpNames.StructureName] = "struct name",
            [CSharpNames.InterfaceName] = "interface name",
            [CSharpNames.EnumName] = "enum name",
            [CSharpNames.DelegateName] = "delegate name",
            [CSharpNames.TypeParameterName] = "type parameter name",
            [CSharpNames.ControlFlowName] = "keyword",

            [VisualBasicNames.ClassName] = "class name",
            [VisualBasicNames.StructureName] = "struct name",
            [VisualBasicNames.InterfaceName] = "interface name",
            [VisualBasicNames.EnumName] = "enum name",
            [VisualBasicNames.DelegateName] = "delegate name",
            [VisualBasicNames.TypeParameterName] = "type parameter name",
            [VisualBasicNames.ModuleName] = "module name",
            [VisualBasicNames.ControlFlowName] = "keyword",
        };
    }
}