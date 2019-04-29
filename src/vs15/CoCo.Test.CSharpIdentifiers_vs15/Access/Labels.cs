using CoCo.Analyser.Classifications.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class Labels : CSharpIdentifierTests
    {
        [Test]
        public void LabelTest()
        {
            GetContext(@"Access\Label.cs").GetClassifications().AssertContains(
                CSharpNames.LabelName.ClassifyAt(187, 4));
        }
    }
}