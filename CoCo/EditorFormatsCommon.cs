using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace CoCo.FormatDefinition
{
    // TODO: Need generate a custom options. It gets possibilite to set other available options
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.LocalVariableName)]
    [Name(Names.LocalVariableName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class LocalVariableFormat : ClassificationFormatDefinition
    {
        public LocalVariableFormat()
        {
            DisplayName = "CoCo format: local variables";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.RangeVariableName)]
    [Name(Names.RangeVariableName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class RangeVariableFormat : ClassificationFormatDefinition
    {
        public RangeVariableFormat()
        {
            DisplayName = "CoCo format: range variables";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.ParameterName)]
    [Name(Names.ParameterName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class ParameterFormat : ClassificationFormatDefinition
    {
        public ParameterFormat()
        {
            DisplayName = "CoCo format: parameters";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.NamespaceName)]
    [Name(Names.NamespaceName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class NamespaceFormat : ClassificationFormatDefinition
    {
        public NamespaceFormat()
        {
            DisplayName = "CoCo format: namespaces";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.ExtensionMethodName)]
    [Name(Names.ExtensionMethodName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class ExtensionMethodFormat : ClassificationFormatDefinition
    {
        public ExtensionMethodFormat()
        {
            DisplayName = "CoCo format: extension methods";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.MethodName)]
    [Name(Names.MethodName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class MethodFormat : ClassificationFormatDefinition
    {
        public MethodFormat()
        {
            DisplayName = "CoCo format: methods";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.EventName)]
    [Name(Names.EventName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class EventFormat : ClassificationFormatDefinition
    {
        public EventFormat()
        {
            DisplayName = "CoCo format: events";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.PropertyName)]
    [Name(Names.PropertyName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class PropertyFormat : ClassificationFormatDefinition
    {
        public PropertyFormat()
        {
            DisplayName = "CoCo format: properties";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.FieldName)]
    [Name(Names.FieldName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class FieldFormat : ClassificationFormatDefinition
    {
        public FieldFormat()
        {
            DisplayName = "CoCo format: fields";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.StaticMethodName)]
    [Name(Names.StaticMethodName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class StaticMethodFormat : ClassificationFormatDefinition
    {
        public StaticMethodFormat()
        {
            DisplayName = "CoCo format: static methods";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.EnumFieldName)]
    [Name(Names.EnumFieldName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class EnumFieldFormat : ClassificationFormatDefinition
    {
        public EnumFieldFormat()
        {
            DisplayName = "CoCo format: enum field";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.AliasNamespaceName)]
    [Name(Names.AliasNamespaceName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class AliasNamespaceFormat : ClassificationFormatDefinition
    {
        public AliasNamespaceFormat()
        {
            DisplayName = "CoCo format: alias namespace";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.ConstructorName)]
    [Name(Names.ConstructorName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class ConstructorFormat : ClassificationFormatDefinition
    {
        public ConstructorFormat()
        {
            DisplayName = "CoCo format: constructors";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.LabelName)]
    [Name(Names.LabelName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class LabelFormat : ClassificationFormatDefinition
    {
        public LabelFormat()
        {
            DisplayName = "CoCo format: label";
        }
    }
}