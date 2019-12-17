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

        [Test]
        public void UnionTest()
        {
            GetContext(@"Declarations\Types\UnionType.fs").GetClassifications().AssertContains(
                FSharpNames.UnionName.ClassifyAt(25, 4),
                FSharpNames.UnionName.ClassifyAt(39, 4),
                FSharpNames.UnionName.ClassifyAt(58, 4),
                FSharpNames.UnionName.ClassifyAt(66, 4),
                FSharpNames.UnionName.ClassifyAt(73, 4));
        }

        [Test]
        public void UnionTestWithArgument()
        {
            // NOTE: check that the argument in union case will not classified
            GetContext(@"Declarations\Types\UnionTypeWithArgument.fs").GetClassifications().AssertIsEquivalent(
                FSharpNames.ModuleName.ClassifyAt(7, 21),
                FSharpNames.UnionName.ClassifyAt(37, 4),
                FSharpNames.UnionName.ClassifyAt(51, 4));
        }

        [Test]
        public void EnumTest()
        {
            GetContext(@"Declarations\Types\EnumType.fs").GetClassifications().AssertContains(
                FSharpNames.EnumName.ClassifyAt(24, 5),
                FSharpNames.EnumFieldName.ClassifyAt(40, 3),
                FSharpNames.EnumFieldName.ClassifyAt(56, 5));
        }

        [Test]
        public void InterfaceTest()
        {
            GetContext(@"Declarations\Types\InterfaceType.fs").GetClassifications().AssertContains(
                FSharpNames.InterfaceName.ClassifyAt(33, 10));
        }

        [Test]
        public void DelegateTest()
        {
            GetContext(@"Declarations\Types\DelegateType.fs").GetClassifications().AssertContains(
                FSharpNames.DelegateName.ClassifyAt(28, 12));
        }

        [Test]
        public void TypeParameterTest()
        {
            GetContext(@"Declarations\Types\TypeParameterType.fs").GetClassifications().AssertContains(
                FSharpNames.TypeParameterName.ClassifyAt(38, 7));
        }

        [Test]
        public void ExcceptionTest()
        {
            GetContext(@"Declarations\Types\ExceptionType.fs").GetClassifications().AssertContains(
                FSharpNames.ClassName.ClassifyAt(34, 6));
        }
    }
}