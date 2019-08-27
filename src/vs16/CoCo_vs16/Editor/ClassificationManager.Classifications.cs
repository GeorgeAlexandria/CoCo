using System.Collections.Generic;
using System.Linq;
using CoCo.Analyser.Classifications.CSharp;
using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Utils;

namespace CoCo.Editor
{
    partial class ClassificationManager
    {
        private static Dictionary<string, IEnumerable<string>> _classificationDependents = new Dictionary<string, IEnumerable<string>>
        {
            [CSharpNames.ClassName] = new[] { "class name", "static symbol" },
            [CSharpNames.StructureName] = "struct name".Enumerate(),
            [CSharpNames.InterfaceName] = "interface name".Enumerate(),
            [CSharpNames.EnumName] = "enum name".Enumerate(),
            [CSharpNames.DelegateName] = "delegate name".Enumerate(),
            [CSharpNames.TypeParameterName] = "type parameter name".Enumerate(),

            [CSharpNames.ConstantFieldName] = new[] { "constant name", "static symbol" },
            [CSharpNames.ConstructorName] = new[] { "class name", "struct name" },
            [CSharpNames.DestructorName] = "class name".Enumerate(),
            [CSharpNames.EnumFieldName] = "enum member name".Enumerate(),
            [CSharpNames.EventName] = new[] { "event name", "static symbol" },
            [CSharpNames.ExtensionMethodName] = new[] { "extension method name", "static symbol" },
            [CSharpNames.FieldName] = new[] { "field name", "static symbol" },
            [CSharpNames.LabelName] = "label name".Enumerate(),
            [CSharpNames.LocalMethodName] = "method name".Enumerate(),
            [CSharpNames.LocalVariableName] = "local name".Enumerate(),
            [CSharpNames.MethodName] = new[] { "method name", "static symbol" },
            [CSharpNames.NamespaceName] = "namespace name".Enumerate(),
            [CSharpNames.ParameterName] = "parameter name".Enumerate(),
            [CSharpNames.PropertyName] = new[] { "property name", "static symbol" },
            [CSharpNames.RangeVariableName] = "local name".Enumerate(),
            [CSharpNames.StaticMethodName] = new[] { "method name", "static symbol" },
            [CSharpNames.ControlFlowName] = "keyword - control".Enumerate(),

            [VisualBasicNames.ClassName] = new[] { "class name", "static symbol" },
            [VisualBasicNames.StructureName] = "struct name".Enumerate(),
            [VisualBasicNames.InterfaceName] = "interface name".Enumerate(),
            [VisualBasicNames.EnumName] = "enum name".Enumerate(),
            [VisualBasicNames.DelegateName] = "delegate name".Enumerate(),
            [VisualBasicNames.TypeParameterName] = "type parameter name".Enumerate(),
            [VisualBasicNames.ModuleName] = "module name".Enumerate(),

            [VisualBasicNames.ConstantFieldName] = new[] { "constant name", "static symbol" },
            [VisualBasicNames.EnumFieldName] = "enum member name".Enumerate(),
            [VisualBasicNames.EventName] = new[] { "event name", "static symbol" },
            [VisualBasicNames.ExtensionMethodName] = new[] { "extension method name", "static symbol" },
            [VisualBasicNames.FieldName] = new[] { "field name", "static symbol" },
            [VisualBasicNames.FunctionName] = new[] { "method name", "static symbol" },
            [VisualBasicNames.FunctionVariableName] = "local name".Enumerate(),
            [VisualBasicNames.LocalVariableName] = "local name".Enumerate(),
            [VisualBasicNames.NamespaceName] = "namespace name".Enumerate(),
            [VisualBasicNames.ParameterName] = "parameter name".Enumerate(),
            [VisualBasicNames.PropertyName] = new[] { "property name", "static symbol" },
            [VisualBasicNames.RangeVariableName] = "local name".Enumerate(),
            [VisualBasicNames.SharedMethodName] = new[] { "method name", "static symbol" },
            [VisualBasicNames.StaticLocalVariableName] = "local name".Enumerate(),
            [VisualBasicNames.SubName] = new[] { "method name", "static symbol" },
            [VisualBasicNames.WithEventsPropertyName] = new[] { "property name", "static symbol" },
            [VisualBasicNames.ControlFlowName] = "keyword - control".Enumerate(),
        };

        private static Dictionary<string, string> _nonIdentifierClassifications;
        private static IReadOnlyDictionary<string, string> NonIdentifierClassifications
        {
            get
            {
                if (_nonIdentifierClassifications is null)
                {
                    var registryService = ServicesProvider.Instance.RegistryService;
                    var formatMap = ServicesProvider.Instance.FormatMapService.GetClassificationFormatMap(category: "text");

                    _nonIdentifierClassifications = new Dictionary<string, string>();

                    // NOTE: get max priority from all of dependent classifications 
                    foreach (var (name, classifications) in _classificationDependents)
                    {
                        var maxPriority = -1;
                        string maxName = null;
                        foreach (var classification in classifications)
                        {
                            var position = GetClassificationPosition(registryService, formatMap, classification);
                            if (maxPriority < position)
                            {
                                maxPriority = position;
                                maxName = classification;
                            }
                        }
                        _nonIdentifierClassifications[name] = maxName;
                    }
                    _classificationDependents = null;
                }
                return _nonIdentifierClassifications;
            }
        }
    }
}