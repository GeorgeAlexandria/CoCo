using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace CoCo.FormatDefinition
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Names.LocalMethodName)]
    [Name(Names.LocalMethodName)]
    [UserVisible(true)]
    [Order(After = PredefinedClassificationTypeNames.Identifier)]
    internal sealed class LocalMethodFormat : ClassificationFormatDefinition
    {
        public LocalMethodFormat()
        {
            DisplayName = "CoCo format: local method";
        }
    }
}