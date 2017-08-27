using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace CoCo
{
    // TODO: Need generate a custom options. It gets possibilite to set other available options
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.LocalFieldName)]
    [Name(Names.LocalFieldName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class LocalFieldFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFieldFormat"/> class.
        /// </summary>
        public LocalFieldFormat()
        {
            DisplayName = "CoCo format: local fields";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.ParameterName)]
    [Name(Names.ParameterName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class ParameterFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterFormat"/> class.
        /// </summary>
        public ParameterFormat()
        {
            DisplayName = "CoCo format: parameters";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.NamespaceName)]
    [Name(Names.NamespaceName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class NamespaceFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceFormat"/> class.
        /// </summary>
        public NamespaceFormat()
        {
            DisplayName = "CoCo format: namespaces";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.ExtensionMethodName)]
    [Name(Names.ExtensionMethodName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class ExtensionMethodFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionMethodFormat"/> class.
        /// </summary>
        public ExtensionMethodFormat()
        {
            DisplayName = "CoCo format: extension methods";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.MethodName)]
    [Name(Names.MethodName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class MethodFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodFormat"/> class.
        /// </summary>
        public MethodFormat()
        {
            DisplayName = "CoCo format: methods";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.EventName)]
    [Name(Names.EventName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class EventFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventFormat"/> class.
        /// </summary>
        public EventFormat()
        {
            DisplayName = "CoCo format: events";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.PropertyName)]
    [Name(Names.PropertyName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class PropertyFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyFormat"/> class.
        /// </summary>
        public PropertyFormat()
        {
            DisplayName = "CoCo format: properies";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.FieldName)]
    [Name(Names.FieldName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class FieldFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldFormat"/> class.
        /// </summary>
        public FieldFormat()
        {
            DisplayName = "CoCo format: fileds";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.StaticMethodName)]
    [Name(Names.StaticMethodName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class StaticMethodFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticMethodFormat"/> class.
        /// </summary>
        public StaticMethodFormat()
        {
            DisplayName = "CoCo format: static methods";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.EnumFiedName)]
    [Name(Names.EnumFiedName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class EnumFieldFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumFieldFormat"/> class.
        /// </summary>
        public EnumFieldFormat()
        {
            DisplayName = "CoCo format: enum field";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.AliasNamespaceName)]
    [Name(Names.AliasNamespaceName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class AliasNamespaceFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AliasNamespaceFormat"/> class.
        /// </summary>
        public AliasNamespaceFormat()
        {
            DisplayName = "CoCo format: alias namespace field";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.ConstructorMethodName)]
    [Name(Names.ConstructorMethodName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class ConstructorMethodFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorMethodFormat"/> class.
        /// </summary>
        public ConstructorMethodFormat()
        {
            DisplayName = "CoCo format: constructor method field";
        }
    }
}