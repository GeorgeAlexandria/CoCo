using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Methods : CSharpIdentifierTests
    {
        [Test]
        public void MethodTest_Constructor()
        {
            GetContext(@"Declarations\Methods\Constructor.cs").GetClassifications().AssertContains(
                CSharpNames.ConstructorName.ClassifyAt(107, 11));
        }

        [Test]
        public void MethodTest_Destructor()
        {
            GetContext(@"Declarations\Methods\Destructor.cs").GetClassifications().AssertContains(
                CSharpNames.DestructorName.ClassifyAt(100, 10));
        }

        [Test]
        public void MethodTest()
        {
            GetContext(@"Declarations\Methods\Method.cs").GetClassifications().AssertContains(
                CSharpNames.MethodName.ClassifyAt(107, 6));
        }

        [Test]
        public void MethodTest_StaticMethod()
        {
            GetContext(@"Declarations\Methods\StaticMethod.cs").GetClassifications().AssertContains(
                CSharpNames.StaticMethodName.ClassifyAt(120, 3));
        }

        [Test]
        public void MethodTest_ExtensionMethod()
        {
            GetContext(@"Declarations\Methods\ExtensionMethod.cs").GetClassifications().AssertContains(
                CSharpNames.ExtensionMethodName.ClassifyAt(131, 12));
        }

        [Test]
        public void MethodTest_LocalMethod()
        {
            GetContext(@"Declarations\Methods\LocalMethod.cs").GetClassifications().AssertContains(
                CSharpNames.LocalMethodName.ClassifyAt(150, 3));
        }

        [Test]
        public void MethodTest_StaticConstructor()
        {
            // NOTE: .cctor doesn't have a special classification, so it will be recognized as static method
            GetContext(@"Declarations\Methods\StaticConstructor.cs").GetClassifications().AssertContains(
                CSharpNames.StaticMethodName.ClassifyAt(113, 17));
        }
    }
}