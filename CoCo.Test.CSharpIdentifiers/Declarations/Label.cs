using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Label : CSharpIdentifierTests
    {
        [Test]
        public void LabelTest()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Label.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.LabelName.ClassifyAt(131, 10),
                    Names.LabelName.ClassifyAt(171, 11));
        }
    }
}