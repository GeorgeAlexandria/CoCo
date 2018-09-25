using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Declarations
{
    internal class Enum : VisualBasicIdentifierTests
    {
        [Test]
        public void EnumTest()
        {
            GetClassifications(@"Declarations\EnumDeclaration.vb").AssertIsEquivalent(
                VisualBasicNames.EnumFieldName.ClassifyAt(22, 4),
                VisualBasicNames.EnumFieldName.ClassifyAt(30, 5),
                VisualBasicNames.EnumFieldName.ClassifyAt(39, 6),
                VisualBasicNames.EnumFieldName.ClassifyAt(49, 6));
        }
    }
}