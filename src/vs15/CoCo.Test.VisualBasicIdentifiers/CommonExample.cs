using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers
{
    internal class CommonExample : VisualBasicIdentifierTests
    {
        [Test]
        public void CommonTest()
        {
            // NOTE: just are checking that a some random identifiers are classified
            GetClassifications(@"Example.vb").AssertContains(
                VisualBasicNames.NamespaceName.ClassifyAt(10, 11),
                VisualBasicNames.PropertyName.ClassifyAt(159, 6),
                VisualBasicNames.ParameterName.ClassifyAt(337, 6),
                VisualBasicNames.FunctionName.ClassifyAt(597, 12),
                VisualBasicNames.FieldName.ClassifyAt(854, 15),
                VisualBasicNames.LocalVariableName.ClassifyAt(1272, 4),
                VisualBasicNames.SubName.ClassifyAt(1527, 3));
        }
    }
}