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
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Methods\ExtensionMethod.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.ExtensionMethodName.ClassifyAt(297, 5),
                    Names.ExtensionMethodName.ClassifyAt(315, 6));
        }

        [Test]
        public void MethodTest()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Methods\Method.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.MethodName.ClassifyAt(146, 9));
        }

        [Test]
        public void MethodTest_Static()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Methods\StaticMethod.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.StaticMethodName.ClassifyAt(165, 9));
        }
    }
}