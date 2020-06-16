using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.VisualBasic.Access
{
    internal class Methods : VisualBasicIdentifierTests
    {
        [Test]
        public void MethodTest_Extension()
        {
            GetContext(@"Access\Methods\ExtensionMethod.vb").GetClassifications().AssertContains(
                VisualBasicNames.ExtensionMethodName.ClassifyAt(129, 6));
        }

        [Test]
        public void MethodTest_Function()
        {
            GetContext(@"Access\Methods\FunctionMethod.vb").GetClassifications().AssertContains(
                VisualBasicNames.FunctionName.ClassifyAt(105, 6));
        }

        [Test]
        public void MethodTest_Sub()
        {
            GetContext(@"Access\Methods\SubMethod.vb").GetClassifications().AssertContains(
                VisualBasicNames.SubName.ClassifyAt(86, 6));
        }

        [Test]
        public void MethodTest_Shared()
        {
            GetContext(@"Access\Methods\SharedMethod.vb").GetClassifications().AssertContains(
                VisualBasicNames.SharedMethodName.ClassifyAt(98, 6));
        }

        [Test]
        public void MethodTest_ModuleMethod()
        {
            GetContext(@"Access\Methods\ModuleMethod.vb").GetClassifications().AssertContains(
                VisualBasicNames.SharedMethodName.ClassifyAt(97, 3));
        }
    }
}