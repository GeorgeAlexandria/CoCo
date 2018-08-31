using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class Methods : CSharpIdentifierTests
    {
        [Test]
        public void MethodTest_Extension()
        {
            @"Access\Methods\ExtensionMethod.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.ExtensionMethodName.ClassifyAt(297, 5),
                    CSharpNames.ExtensionMethodName.ClassifyAt(315, 6));
        }

        [Test]
        public void MethodTest_Local()
        {
            @"Access\Methods\LocalMethod.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.LocalMethodName.ClassifyAt(226, 10),
                    CSharpNames.LocalMethodName.ClassifyAt(274, 10));
        }

        [Test]
        public void MethodTest()
        {
            @"Access\Methods\Method.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.MethodName.ClassifyAt(146, 9));
        }

        [Test]
        public void MethodTest_Static()
        {
            @"Access\Methods\StaticMethod.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.StaticMethodName.ClassifyAt(165, 9));
        }
    }
}