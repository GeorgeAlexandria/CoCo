using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace CoCo
{
    /// <summary>
    /// Classification type definition export for <see cref="EditorClassifier"/>
    /// </summary>
    internal static partial class ClassificationDefinitions
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
        [Name(Names.RangeFieldName)]
        private static ClassificationTypeDefinition rangeFieldDefinition;

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

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.StaticMethodName)]
        private static ClassificationTypeDefinition staticMethodDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.EnumFieldName)]
        private static ClassificationTypeDefinition enumFieldDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.AliasNamespaceName)]
        private static ClassificationTypeDefinition aliasNamespaceDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.ConstructorMethodName)]
        private static ClassificationTypeDefinition constructorMethodDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.LabelName)]
        private static ClassificationTypeDefinition labelDefinition;

#pragma warning restore 169
    }
}