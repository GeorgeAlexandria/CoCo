using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace CoCo
{
    internal static partial class ClassificationDefinitions
    {
        // Disable "The field is never used" compiler's warning. The fields are used by MEF.
#pragma warning disable 169

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Names.LocalMethodName)]
        private static ClassificationTypeDefinition localMethodDefinition;

#pragma warning restore 169
    }
}