using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.VisualBasic.Declarations
{
    internal class Methods : VisualBasicIdentifierTests
    {
        [Test]
        public void MethodTest_Sub()
        {
            GetContext(@"Declarations\Methods\SubMethod.vb").GetClassifications().AssertContains(
                VisualBasicNames.SubName.ClassifyAt(39, 6));
        }

        [Test]
        public void MethodTest_Function()
        {
            GetContext(@"Declarations\Methods\FunctionMethod.vb").GetClassifications().AssertContains(
                VisualBasicNames.FunctionName.ClassifyAt(49, 6));
        }

        [Test]
        public void MethodTest_Extension()
        {
            GetContext(@"Declarations\Methods\ExtensionMethod.vb").GetClassifications().AssertContains(
                VisualBasicNames.ExtensionMethodName.ClassifyAt(104, 6));
        }

        [Test]
        public void MethodTest_Constructor()
        {
            GetContext(@"Declarations\Methods\Constructor.vb")
                .AddInfo(VisualBasicNames.ClassName.DisableInEditor())
                .GetClassifications().AssertIsEmpty();
        }

        [Test]
        public void MethodTest_SharedConstructor()
        {
            GetContext(@"Declarations\Methods\StaticConstructor.vb")
                .AddInfo(VisualBasicNames.ClassName.DisableInEditor())
                .GetClassifications().AssertIsEmpty();
        }

        [Test]
        public void MethodTest_Shared()
        {
            GetContext(@"Declarations\Methods\SharedMethod.vb").GetClassifications().AssertContains(
                VisualBasicNames.SharedMethodName.ClassifyAt(49, 6));
        }

        [Test]
        public void MethodTest_ModuleMethod()
        {
            GetContext(@"Declarations\Methods\ModuleMethod.vb").GetClassifications().AssertContains(
                VisualBasicNames.SharedMethodName.ClassifyAt(43, 6));
        }
    }
}