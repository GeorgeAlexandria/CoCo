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
            @"Access\EnumField.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.EnumFieldName.ClassifyAt(152, 9),
                    CSharpNames.EnumFieldName.ClassifyAt(209, 3));
        }
    }
}