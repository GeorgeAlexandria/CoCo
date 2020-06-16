using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.FSharp.Declarations
{
    internal class Locals : FSharpIdentifierTests
    {
        [Test]
        public void LocalValue()
        {
            GetContext(@"Declarations\Locals\LocalValueBinding.fs").GetClassifications().AssertContains(
                FSharpNames.LocalBindingValueName.ClassifyAt(52, 5));
        }

        [Test]
        public void LocalFunction()
        {
            GetContext(@"Declarations\Locals\LocalFunction.fs").GetClassifications().AssertContains(
                FSharpNames.LocalBindingValueName.ClassifyAt(48, 8));
        }

        [Test]
        public void Match()
        {
            GetContext(@"Declarations\Locals\Match.fs").GetClassifications().AssertContains(
                FSharpNames.LocalBindingValueName.ClassifyAt(78, 4),
                FSharpNames.LocalBindingValueName.ClassifyAt(84, 4),
                FSharpNames.LocalBindingValueName.ClassifyAt(197, 4),
                FSharpNames.LocalBindingValueName.ClassifyAt(203, 6),
                FSharpNames.LocalBindingValueName.ClassifyAt(211, 4),
                FSharpNames.LocalBindingValueName.ClassifyAt(234, 4),
                FSharpNames.LocalBindingValueName.ClassifyAt(362, 5),
                FSharpNames.LocalBindingValueName.ClassifyAt(385, 4),
                FSharpNames.LocalBindingValueName.ClassifyAt(449, 5),
                FSharpNames.LocalBindingValueName.ClassifyAt(473, 4),
                FSharpNames.LocalBindingValueName.ClassifyAt(551, 5),
                FSharpNames.LocalBindingValueName.ClassifyAt(558, 5),
                FSharpNames.LocalBindingValueName.ClassifyAt(581, 3));
        }

        [Test]
        public void For()
        {
            GetContext(@"Declarations\Locals\ForValue.fs").GetClassifications().AssertContains(
                FSharpNames.LocalBindingValueName.ClassifyAt(48, 4));
        }

        [Test]
        public void Foreach()
        {
            GetContext(@"Declarations\Locals\ForeachValue.fs").GetClassifications().AssertContains(
                FSharpNames.LocalBindingValueName.ClassifyAt(52, 4));
        }

        [Test]
        public void TryWith()
        {
            GetContext(@"Declarations\Locals\TryWithValue.fs").GetClassifications().AssertContains(
                FSharpNames.LocalBindingValueName.ClassifyAt(107, 3));
        }

        [Test]
        public void StaticLetBinding()
        {
            GetContext(@"Declarations\Locals\StaticLetBinding.fs").GetClassifications().AssertContains(
                FSharpNames.LocalBindingValueName.ClassifyAt(57, 5),
                FSharpNames.LocalBindingValueName.ClassifyAt(97, 6));
        }

        [Test]
        public void UseBinding()
        {
            GetContext(@"Declarations\Locals\UseBinding.fs").GetClassifications().AssertContains(
                FSharpNames.LocalBindingValueName.ClassifyAt(41, 4));
        }
    }
}