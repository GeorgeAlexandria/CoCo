using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.FSharpIdentifiers.Access
{
    internal class Types : FSharpIdentifierTests
    {
        [Test]
        public void InterfaceImplementationTest()
        {
            GetContext(@"Access\Types\InterfaceImplementation.fs").GetClassifications().AssertContains(
                FSharpNames.InterfaceName.ClassifyAt(114, 5),
                FSharpNames.MethodName.ClassifyAt(144, 3));
        }

        [Test]
        public void UnionCasesTest()
        {
            GetContext(@"Access\Types\UnionCases.fs").GetClassifications().AssertContains(
                FSharpNames.UnionName.ClassifyAt(111, 5),
                FSharpNames.UnionName.ClassifyAt(117, 5),
                FSharpNames.UnionName.ClassifyAt(139, 5),
                FSharpNames.UnionName.ClassifyAt(145, 6),
                FSharpNames.UnionName.ClassifyAt(212, 5),
                FSharpNames.UnionName.ClassifyAt(233, 6));
        }

        [Test]
        public void TypeParameterTest()
        {
            GetContext(@"Access\Types\TypeParameterType.fs").GetClassifications().AssertContains(
                FSharpNames.TypeParameterName.ClassifyAt(94, 7));
        }

        [Test]
        public void ExceptionTest()
        {
            GetContext(@"Access\Types\ExceptionType.fs").GetClassifications().AssertContains(
                FSharpNames.ClassName.ClassifyAt(60, 6));
        }

        [Test]
        public void PostfixTypesTest()
        {
            GetContext(@"Access\Types\PostfixType.fs").GetClassifications().AssertContains(
                FSharpNames.ClassName.ClassifyAt(67, 4),
                FSharpNames.ClassName.ClassifyAt(72, 4));
        }

        [Test]
        public void TypeAsExpression()
        {
            GetContext(@"Access\Types\TypeAsExpression.fs").GetClassifications().AssertContains(
                FSharpNames.ClassName.ClassifyAt(62, 4));
        }
    }
}