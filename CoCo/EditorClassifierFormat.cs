using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace CoCo.FormatDefinition
{
    // TODO: Need generate a custom options. It gets possibilite to set other available options
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.LocalFieldName)]
    [Name(Names.LocalFieldName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class LocalFieldFormat : ClassificationFormatDefinition
    {
        public LocalFieldFormat()
        {
            DisplayName = "CoCo format: local fields";
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
            DisplayName = "CoCo format: properies";
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
            DisplayName = "CoCo format: fileds";
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
    [ClassificationType(ClassificationTypeNames = Names.EnumFiedName)]
    [Name(Names.EnumFiedName)]
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
    [ClassificationType(ClassificationTypeNames = Names.ConstructorMethodName)]
    [Name(Names.ConstructorMethodName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class ConstructorMethodFormat : ClassificationFormatDefinition
    {
        public ConstructorMethodFormat()
        {
            DisplayName = "CoCo format: constructor method";
        }
    }
}