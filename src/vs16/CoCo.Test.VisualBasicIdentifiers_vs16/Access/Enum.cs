using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Access
{
    internal class Enum : VisualBasicIdentifierTests
    {
        [Test]
        public void EnumTest()
        {
            GetContext(@"Access\EnumAccess.vb").GetClassifications().AssertContains(
                VisualBasicNames.EnumFieldName.ClassifyAt(77, 9),
                VisualBasicNames.EnumFieldName.ClassifyAt(116, 3));
        }
    }
}