using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class Methods : CSharpIdentifierTests
    {
        [Test]
        public void MethodTest_Extension()
        {
            GetClassifications(@"Access\Methods\ExtensionMethod.cs").AssertContains(
                CSharpNames.ExtensionMethodName.ClassifyAt(297, 5),
                CSharpNames.ExtensionMethodName.ClassifyAt(315, 6));
        }

        [Test]
        public void MethodTest()
        {
            GetClassifications(@"Access\Methods\Method.cs")
                .AssertContains(CSharpNames.MethodName.ClassifyAt(146, 9));
        }

        [Test]
        public void MethodTest_Static()
        {
            GetClassifications(@"Access\Methods\StaticMethod.cs")
                .AssertContains(CSharpNames.StaticMethodName.ClassifyAt(165, 9));
        }
    }
}