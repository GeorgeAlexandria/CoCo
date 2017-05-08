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
        private static ClassificationTypeDefinition fieldDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.ParameterName)]
        private static ClassificationTypeDefinition parameterDefinition;

#pragma warning restore 169
    }
}