using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Access
{
    internal class Methods : VisualBasicIdentifierTests
    {
        [Test]
        public void MethodTest_Extension()
        {
            GetClassifications(@"Access\Methods\ExtensionMethod.vb")
                .AssertContains(VisualBasicNames.ExtensionMethodName.ClassifyAt(129, 6));
        }

        [Test]
        public void MethodTest_Function()
        {
            GetClassifications(@"Access\Methods\FunctionMethod.vb")
                .AssertContains(VisualBasicNames.FunctionName.ClassifyAt(105, 6));
        }

        [Test]
        public void MethodTest_Sub()
        {
            GetClassifications(@"Access\Methods\SubMethod.vb")
                .AssertContains(VisualBasicNames.SubName.ClassifyAt(86, 6));
        }
    }
}