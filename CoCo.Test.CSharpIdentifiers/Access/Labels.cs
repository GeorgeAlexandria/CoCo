using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class Labels : CSharpIdentifierTests
    {
        [Test]
        public void LabelTest()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Label.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LabelName.ClassifyAt(187, 4));
        }
    }
}