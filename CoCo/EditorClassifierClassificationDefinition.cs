//------------------------------------------------------------------------------
// <copyright file="EditorClassifierClassificationDefinition.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace CoCo
{
    /// <summary>
    /// Classification type definition export for EditorClassifier
    /// </summary>
    internal static class EditorClassifierClassificationDefinition
    {
        // Disable "The field is never used" compiler's warning. The fields are used by MEF.
#pragma warning disable 169

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.NamespaceName)]
        private static ClassificationTypeDefinition namespaceDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.LocalFieldName)]
        private static ClassificationTypeDefinition localFieldDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.ParameterName)]
        private static ClassificationTypeDefinition parameterDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.ExtensionMethodName)]
        private static ClassificationTypeDefinition extensionMethodDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.MethodName)]
        private static ClassificationTypeDefinition methodDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.EventName)]
        private static ClassificationTypeDefinition eventDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.PropertyName)]
        private static ClassificationTypeDefinition propertyDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.FieldName)]
        private static ClassificationTypeDefinition fieldDefinition;

#pragma warning restore 169
    }
}