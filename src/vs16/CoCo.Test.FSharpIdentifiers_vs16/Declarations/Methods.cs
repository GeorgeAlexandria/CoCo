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
    }
}