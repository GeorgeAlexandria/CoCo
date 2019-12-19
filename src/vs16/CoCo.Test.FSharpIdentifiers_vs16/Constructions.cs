using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.FSharpIdentifiers
{
    class Constructions : FSharpIdentifierTests
    {
        [Test]
        public void ObjectExpressionTest()
        {
            GetContext(@"Constructions\ObjectExpression.fs").GetClassifications().AssertContains(
                FSharpNames.ParameterName.ClassifyAt(98, 4));
        }
    }
}
