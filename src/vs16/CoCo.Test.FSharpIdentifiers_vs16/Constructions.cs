using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.FSharpIdentifiers
{
    internal class Constructions : FSharpIdentifierTests
    {
        [Test]
        public void ObjectExpressionTest()
        {
            GetContext(@"Constructions\ObjectExpression.fs").GetClassifications().AssertContains(
                FSharpNames.ParameterName.ClassifyAt(98, 4));
        }

        [Test]
        public void RaiseTest()
        {
            GetContext(@"Constructions\Raise.fs").GetClassifications().AssertContains(
                FSharpNames.NamespaceName.ClassifyAt(35, 6),
                FSharpNames.ClassName.ClassifyAt(42, 21));
        }

        [Test]
        public void TypeConstraintsTest()
        {
            GetContext(@"Constructions\TypeConstraints.fs").GetClassifications().AssertContains(
                FSharpNames.TypeParameterName.ClassifyAt(45, 2),
                FSharpNames.NamespaceName.ClassifyAt(51, 6),
                FSharpNames.InterfaceName.ClassifyAt(58, 11),
                FSharpNames.TypeParameterName.ClassifyAt(103, 2),
                FSharpNames.MethodName.ClassifyAt(116, 5),
                FSharpNames.TypeParameterName.ClassifyAt(170, 2),
                FSharpNames.PropertyName.ClassifyAt(183, 5),
                FSharpNames.TypeParameterName.ClassifyAt(229, 2),
                FSharpNames.StaticMethodName.ClassifyAt(249, 5));
        }
    }
}