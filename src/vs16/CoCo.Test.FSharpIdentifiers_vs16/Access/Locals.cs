using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.FSharpIdentifiers.Access
{
    internal class Locals : FSharpIdentifierTests
    {
        [Test]
        public void LocalValueBindingTest()
        {
            GetContext(@"Access\Locals\LocalValueBinding.fs").GetClassifications().AssertContains(
                FSharpNames.LocalBindingValueName.ClassifyAt(76, 4));
        }

        [Test]
        public void LocalFunctionTest()
        {
            GetContext(@"Access\Locals\LocalFunction.fs").GetClassifications().AssertContains(
                FSharpNames.LocalBindingValueName.ClassifyAt(71, 4),
                FSharpNames.LocalBindingValueName.ClassifyAt(97, 4));
        }

        [Test]
        public void MatchTest()
        {
            GetContext(@"Access\Locals\Match.fs").GetClassifications().AssertContains(
                FSharpNames.LocalBindingValueName.ClassifyAt(92, 4),
                FSharpNames.LocalBindingValueName.ClassifyAt(109, 4),
                FSharpNames.LocalBindingValueName.ClassifyAt(240, 4),
                FSharpNames.LocalBindingValueName.ClassifyAt(247, 6),
                FSharpNames.LocalBindingValueName.ClassifyAt(403, 5),   
                FSharpNames.LocalBindingValueName.ClassifyAt(429, 4),
                FSharpNames.LocalBindingValueName.ClassifyAt(498, 5),
                FSharpNames.LocalBindingValueName.ClassifyAt(525, 4),
                FSharpNames.LocalBindingValueName.ClassifyAt(614, 5),
                FSharpNames.LocalBindingValueName.ClassifyAt(622, 5));
        }

        [Test]
        public void ForAndForeachValueTest()
        {
            GetContext(@"Access\Locals\ForAndForeachValue.fs").GetClassifications().AssertContains(
                FSharpNames.LocalBindingValueName.ClassifyAt(99, 4),
                FSharpNames.LocalBindingValueName.ClassifyAt(180, 4));
        }

    }
}