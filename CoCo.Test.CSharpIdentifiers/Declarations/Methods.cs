using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Methods : CSharpIdentifierTests
    {
        [Test]
        public void MethodTest_Constructor()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Methods\Constructor.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.ConstructorMethodName.ClassifyAt(107, 11));
        }

        [Test]
        public void MethodTest()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Methods\Method.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.MethodName.ClassifyAt(107, 6));
        }

        [Test]
        public void MethodTest_StaticMethod()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Methods\StaticMethod.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.StaticMethodName.ClassifyAt(120, 3));
        }

        [Test]
        public void MethodTest_ExtensionMethod()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Methods\ExtensionMethod.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.ExtensionMethodName.ClassifyAt(131, 12));
        }
    }
}