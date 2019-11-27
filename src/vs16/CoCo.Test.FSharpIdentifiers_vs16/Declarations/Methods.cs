using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.FSharpIdentifiers.Declarations
{
    internal class Methods : FSharpIdentifierTests
    {
        [Test]
        public void FunctionTest()
        {
            GetContext(@"Declarations\Methods\ModuleFunction.fs").GetClassifications().AssertContains(
                FSharpNames.ModuleFunctionName.ClassifyAt(29, 4));
        }

        [Test]
        public void MethodTest()
        {
            GetContext(@"Declarations\Methods\Method.fs").GetClassifications().AssertContains(
                FSharpNames.MethodName.ClassifyAt(46, 3));
        }

        [Test]
        public void StaticMethodTest()
        {
            GetContext(@"Declarations\Methods\StaticMethod.fs").GetClassifications().AssertContains(
                FSharpNames.StaticMethodName.ClassifyAt(56, 3));
        }

        [Test, Ignore("Classify as extension or as regular?")]
        public void IntrinsicExtensionMethodTest()
        {
            GetContext(@"Declarations\Methods\IntrinsicExtensionMethod.fs").GetClassifications().AssertContains(
                FSharpNames.StaticMethodName.ClassifyAt(56, 3));
        }

        [Test]
        public void OptionalExtensionMethodTest()
        {
            GetContext(@"Declarations\Methods\OptionalExtensionMethod.fs").GetClassifications().AssertContains(
                FSharpNames.ExtensionMethodName.ClassifyAt(97, 3));
        }

        [Test]
        public void ExtensionMethodTest()
        {
            GetContext(@"Declarations\Methods\ExtensionMethod.fs").GetClassifications().AssertContains(
                FSharpNames.ExtensionMethodName.ClassifyAt(167, 4));
        }

        [Test]
        public void VirtualMethodsTest()
        {
            GetContext(@"Declarations\Methods\VirtualMethods.fs").GetClassifications().AssertContains(
                FSharpNames.MethodName.ClassifyAt(79, 3),
                FSharpNames.MethodName.ClassifyAt(134, 3),
                FSharpNames.MethodName.ClassifyAt(164, 3));
        }
    }
}