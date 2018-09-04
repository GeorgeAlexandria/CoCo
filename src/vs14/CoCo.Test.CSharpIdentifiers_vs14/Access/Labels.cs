using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class Labels : CSharpIdentifierTests
    {
        [Test]
        public void LabelTest()
        {
            GetClassifications(@"Access\Label.cs")
                .AssertContains(CSharpNames.LabelName.ClassifyAt(187, 4));
        }
    }
}