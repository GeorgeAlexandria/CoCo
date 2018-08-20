using CoCo.Analyser;
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
                .AssertContains(Names.ConstructorName.ClassifyAt(107, 11));
        }

        [Test]
        public void MethodTest_Destructor()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Methods\Destructor.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.DestructorName.ClassifyAt(100, 10));
        }

        [Test]
        public void MethodTest()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Methods\Method.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.MethodName.ClassifyAt(107, 6));
        }

        [Test]
        public void MethodTest_StaticMethod()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Methods\StaticMethod.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.StaticMethodName.ClassifyAt(120, 3));
        }

        [Test]
        public void MethodTest_ExtensionMethod()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Methods\ExtensionMethod.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.ExtensionMethodName.ClassifyAt(131, 12));
        }

        [Test]
        public void MethodTest_LocalMethod()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Methods\LocalMethod.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalMethodName.ClassifyAt(150, 3));
        }

        [Test]
        public void MethodTest_StaticConstructor()
        {
            // NOTE: .cctor doesn't have a special classification, so it will be recognized as static method
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Methods\StaticConstructor.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.StaticMethodName.ClassifyAt(113, 17));
        }
    }
}