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
            GetContext(@"Access\Methods\ExtensionMethod.cs").GetClassifications().AssertContains(
                CSharpNames.ExtensionMethodName.ClassifyAt(297, 5),
                CSharpNames.ExtensionMethodName.ClassifyAt(315, 6));
        }

        [Test]
        public void MethodTest()
        {
            GetContext(@"Access\Methods\Method.cs").GetClassifications().AssertContains(
                CSharpNames.MethodName.ClassifyAt(146, 9));
        }

        [Test]
        public void MethodTest_Static()
        {
            GetContext(@"Access\Methods\StaticMethod.cs").GetClassifications().AssertContains(
                CSharpNames.StaticMethodName.ClassifyAt(165, 9));
        }
    }
}