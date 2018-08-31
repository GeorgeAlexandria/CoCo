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
            @"Declarations\Methods\Constructor.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.ConstructorName.ClassifyAt(107, 11));
        }

        [Test]
        public void MethodTest_Destructor()
        {
            @"Declarations\Methods\Destructor.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.DestructorName.ClassifyAt(100, 10));
        }

        [Test]
        public void MethodTest()
        {
            @"Declarations\Methods\Method.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.MethodName.ClassifyAt(107, 6));
        }

        [Test]
        public void MethodTest_StaticMethod()
        {
            @"Declarations\Methods\StaticMethod.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.StaticMethodName.ClassifyAt(120, 3));
        }

        [Test]
        public void MethodTest_ExtensionMethod()
        {
            @"Declarations\Methods\ExtensionMethod.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.ExtensionMethodName.ClassifyAt(131, 12));
        }

        [Test]
        public void MethodTest_StaticConstructor()
        {
            // NOTE: .cctor doesn't have a special classification, so it will be recognized as static method
            @"Declarations\Methods\StaticConstructor.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.StaticMethodName.ClassifyAt(113, 17));
        }
    }
}