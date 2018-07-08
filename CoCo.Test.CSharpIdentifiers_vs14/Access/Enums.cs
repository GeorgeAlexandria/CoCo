using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class Enums : CSharpIdentifierTests
    {
        [Test]
        public void EnumTest()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\EnumField.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.EnumFieldName.ClassifyAt(152, 9),
                    Names.EnumFieldName.ClassifyAt(209, 3));
        }
    }
}