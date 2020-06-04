using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.FSharpIdentifiers.Access
{
    internal class Methods : FSharpIdentifierTests
    {
        [Test]
        public void ModuleFunctionTest()
        {
            GetContext(@"Access\Methods\ModuleFunction.fs").GetClassifications().AssertContains(
                FSharpNames.ModuleFunctionName.ClassifyAt(58, 4),
                FSharpNames.ModuleFunctionName.ClassifyAt(82, 4));
        }

        [Test]
        public void InstanceAndStaticMethodTest()
        {
            GetContext(@"Access\Methods\Methods.fs").GetClassifications().AssertContains(
                FSharpNames.StaticMethodName.ClassifyAt(127, 4),
                FSharpNames.MethodName.ClassifyAt(153, 4),
                FSharpNames.StaticMethodName.ClassifyAt(177, 4),
                FSharpNames.MethodName.ClassifyAt(201, 4));
        }

        [Test]
        public void ExtensionMethodsTest()
        {
            GetContext(@"Access\Methods\ExtensionMethod.fs").GetClassifications().AssertContains(
                FSharpNames.MethodName.ClassifyAt(248, 4),
                FSharpNames.ExtensionMethodName.ClassifyAt(269, 4),
                FSharpNames.ExtensionMethodName.ClassifyAt(290, 4));
        }
    }
}