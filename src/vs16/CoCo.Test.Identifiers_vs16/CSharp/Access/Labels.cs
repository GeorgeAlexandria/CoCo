using CoCo.Analyser.Classifications.CSharp;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.CSharp.Access
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