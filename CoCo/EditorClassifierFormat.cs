//------------------------------------------------------------------------------
// <copyright file="EditorClassifierFormat.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

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
            DisplayName = "A local fields";
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
            DisplayName = "A parameters";
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
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
            DisplayName = "A namespaces";
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
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
            DisplayName = "A extension methods";
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
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
            DisplayName = "A methods";
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
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
            DisplayName = "A events";
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
            DisplayName = "A properies";
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
            DisplayName = "A fileds";
        }
    }
}