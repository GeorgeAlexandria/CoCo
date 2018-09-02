using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Declarations
{
    internal class Methods : VisualBasicIdentifierTests
    {
        [Test]
        public void MethodTest_Sub()
        {
            GetClassifications(@"Declarations\Methods\SubMethod.vb")
                .AssertIsEquivalent(VisualBasicNames.SubName.ClassifyAt(39, 6));
        }

        [Test]
        public void MethodTest_Function()
        {
            GetClassifications(@"Declarations\Methods\FunctionMethod.vb")
                .AssertIsEquivalent(VisualBasicNames.FunctionName.ClassifyAt(49, 6));
        }

        [Test]
        public void MethodTest_Extension()
        {
            GetClassifications(@"Declarations\Methods\ExtensionMethod.vb")
                .AssertContains(VisualBasicNames.ExtensionMethodName.ClassifyAt(104, 6));
        }

        [Test]
        public void MethodTest_Constructor()
        {
            GetClassifications(@"Declarations\Methods\Constructor.vb").AssertIsEmpty();
        }

        [Test]
        public void MethodTest_SharedConstructor()
        {
            GetClassifications(@"Declarations\Methods\SharedConstructor.vb").AssertIsEmpty();
        }
    }
}