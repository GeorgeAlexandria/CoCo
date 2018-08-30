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
            @"Tests\Identifiers\CSharpIdentifiers\Access\Methods\ExtensionMethod.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.ExtensionMethodName.ClassifyAt(297, 5),
                    CSharpNames.ExtensionMethodName.ClassifyAt(315, 6));
        }

        [Test]
        public void MethodTest()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Access\Methods\Method.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.MethodName.ClassifyAt(146, 9));
        }

        [Test]
        public void MethodTest_Static()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Access\Methods\StaticMethod.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.StaticMethodName.ClassifyAt(165, 9));
        }
    }
}