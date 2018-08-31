using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Label : CSharpIdentifierTests
    {
        [Test]
        public void LabelTest()
        {
            @"Declarations\Label.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.LabelName.ClassifyAt(131, 10),
                    CSharpNames.LabelName.ClassifyAt(171, 11));
        }
    }
}