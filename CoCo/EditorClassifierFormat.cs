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
    public static class Names
    {
        public const string LocalFieldName = "Local field name";
        public const string ParameterName = "Parameter name";
        public const string NamespaceName = "Namespace name";
    }

    // TODO: Need generate a custom options. It gets possibilite to set other available options
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.LocalFieldName)]
    [Name(Names.LocalFieldName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
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
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class ParameterFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterFormat"/> class.
        /// </summary>
        public ParameterFormat()
        {
            DisplayName = "A parameters"; // Human readable version of the name
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.NamespaceName)]
    [Name(Names.NamespaceName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class NamespaceFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceFormat"/> class.
        /// </summary>
        public NamespaceFormat()
        {
            DisplayName = "A namespaces"; // Human readable version of the name
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }
}