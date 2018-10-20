using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Types : CSharpIdentifierTests
    {
        [Test]
        public void TypeParameterTest()
        {
            GetContext(@"Declarations\Types\TypeParameter.cs").GetClassifications().AssertContains(
                CSharpNames.TypeParameterName.ClassifyAt(84, 3),
                CSharpNames.TypeParameterName.ClassifyAt(124, 6));
        }

        [Test]
        public void ClassTest()
        {
            GetContext(@"Declarations\Types\ClassType.cs").GetClassifications().AssertContains(
                CSharpNames.ClassName.ClassifyAt(70, 9));
        }

        [Test]
        public void StructureTest()
        {
            GetContext(@"Declarations\Types\StructureType.cs").GetClassifications().AssertContains(
                CSharpNames.StructureName.ClassifyAt(71, 13));
        }

        [Test]
        public void InterfaceTest()
        {
            GetContext(@"Declarations\Types\InterfaceType.cs").GetClassifications().AssertContains(
                CSharpNames.InterfaceName.ClassifyAt(74, 13));
        }

        [Test]
        public void EnumTest()
        {
            GetContext(@"Declarations\Types\EnumType.cs").GetClassifications().AssertContains(
                CSharpNames.EnumName.ClassifyAt(69, 8));
        }

        [Test]
        public void DelegateTest()
        {
            GetContext(@"Declarations\Types\DelegateType.cs").GetClassifications().AssertContains(
                CSharpNames.DelegateName.ClassifyAt(78, 6));
        }
    }
}