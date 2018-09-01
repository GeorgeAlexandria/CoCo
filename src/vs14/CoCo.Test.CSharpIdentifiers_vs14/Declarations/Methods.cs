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
            GetClassifications(@"Declarations\Methods\Constructor.cs")
                .AssertContains(CSharpNames.ConstructorName.ClassifyAt(107, 11));
        }

        [Test]
        public void MethodTest_Destructor()
        {
            GetClassifications(@"Declarations\Methods\Destructor.cs")
                .AssertContains(CSharpNames.DestructorName.ClassifyAt(100, 10));
        }

        [Test]
        public void MethodTest()
        {
            GetClassifications(@"Declarations\Methods\Method.cs")
                .AssertContains(CSharpNames.MethodName.ClassifyAt(107, 6));
        }

        [Test]
        public void MethodTest_StaticMethod()
        {
            GetClassifications(@"Declarations\Methods\StaticMethod.cs")
                .AssertContains(CSharpNames.StaticMethodName.ClassifyAt(120, 3));
        }

        [Test]
        public void MethodTest_ExtensionMethod()
        {
            GetClassifications(@"Declarations\Methods\ExtensionMethod.cs")
                .AssertContains(CSharpNames.ExtensionMethodName.ClassifyAt(131, 12));
        }

        [Test]
        public void MethodTest_StaticConstructor()
        {
            // NOTE: .cctor doesn't have a special classification, so it will be recognized as static method
            GetClassifications(@"Declarations\Methods\StaticConstructor.cs")
                .AssertContains(CSharpNames.StaticMethodName.ClassifyAt(113, 17));
        }
    }
}