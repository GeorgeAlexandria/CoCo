using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.FSharpIdentifiers.Declarations
{
    internal class Types : FSharpIdentifierTests
    {
        [Test]
        public void TopLevelModuleTest()
        {
            GetContext(@"Declarations\Types\TopLevelModule.fs").GetClassifications().AssertContains(
                FSharpNames.ModuleName.ClassifyAt(7, 14));
        }

        [Test]
        public void LocalModuleTest()
        {
            GetContext(@"Declarations\Types\LocalModule.fs").GetClassifications().AssertContains(
                FSharpNames.ModuleName.ClassifyAt(25, 6));
        }

        [Test]
        public void NestedModuleTest()
        {
            GetContext(@"Declarations\Types\NestedModule.fs").GetClassifications().AssertContains(
                FSharpNames.ModuleName.ClassifyAt(30, 12));
        }

        [Test]
        public void RecursiveModuleTest()
        {
            GetContext(@"Declarations\Types\RecursiveModule.fs").GetClassifications().AssertContains(
                FSharpNames.ModuleName.ClassifyAt(11, 15));
        }

        [Test]
        public void ClassTest()
        {
            GetContext(@"Declarations\Types\ClassType.fs").GetClassifications().AssertContains(
                FSharpNames.ClassName.ClassifyAt(25, 4));
        }

        [Test]
        public void RecursiveClassTest()
        {
            GetContext(@"Declarations\Types\RecursiveClassType.fs").GetClassifications().AssertContains(
                FSharpNames.ClassName.ClassifyAt(34, 4),
                FSharpNames.ClassName.ClassifyAt(73, 5));
        }

        [Test]
        public void StructureTest()
        {
            GetContext(@"Declarations\Types\StructureType.fs").GetClassifications().AssertContains(
                FSharpNames.StructureName.ClassifyAt(29, 4),
                FSharpNames.StructureName.ClassifyAt(77, 5));
        }

        [Test]
        public void RecordTest()
        {
            GetContext(@"Declarations\Types\RecordType.fs").GetClassifications().AssertContains(
                FSharpNames.RecordName.ClassifyAt(26, 4));
        }

        [Test, Ignore("Union case currently isn't supported")]
        public void UnionTest()
        {
            GetContext(@"Declarations\Types\UnionType.fs").GetClassifications().AssertContains(
                FSharpNames.UnionName.ClassifyAt(25, 4),
                FSharpNames.UnionName.ClassifyAt(39, 4),
                FSharpNames.UnionName.ClassifyAt(58, 4),
                FSharpNames.UnionName.ClassifyAt(66, 4),
                FSharpNames.UnionName.ClassifyAt(73, 4));
        }
    }
}