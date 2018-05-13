using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Enum : CSharpIdentifierTests
    {
        [Test]
        public void EnumTest()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\EnumDeclaration.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.EnumFieldName.ClassifyAt(84, 4),
                    Names.EnumFieldName.ClassifyAt(99, 5),
                    Names.EnumFieldName.ClassifyAt(115, 6),
                    Names.EnumFieldName.ClassifyAt(132, 6));
        }
    }
}