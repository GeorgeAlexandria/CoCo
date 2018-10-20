using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Access
{
    internal class Types : VisualBasicIdentifierTests
    {
        [Test]
        public void TypeParameterTest()
        {
            GetContext(@"Access\Types\TypeParameter.vb").GetClassifications().AssertContains(
                VisualBasicNames.TypeParameterName.ClassifyAt(87, 6));
        }

        [Test]
        public void ClassTest()
        {
            GetContext(@"Access\Types\ClassType.vb").GetClassifications().AssertContains(
                VisualBasicNames.ClassName.ClassifyAt(74, 9));
        }

        [Test]
        public void StructureTest()
        {
            GetContext(@"Access\Types\StructureType.vb").GetClassifications().AssertContains(
                VisualBasicNames.StructureName.ClassifyAt(102, 13));
        }

        [Test]
        public void ModuleTest()
        {
            GetContext(@"Access\Types\ModuleType.vb").GetClassifications().AssertContains(
                VisualBasicNames.ModuleName.ClassifyAt(121, 10));
        }

        [Test]
        public void InterfaceTest()
        {
            GetContext(@"Access\Types\InterfaceType.vb").GetClassifications().AssertContains(
                VisualBasicNames.InterfaceName.ClassifyAt(138, 14));
        }

        [Test]
        public void EnumTest()
        {
            GetContext(@"Access\Types\EnumType.vb").GetClassifications().AssertContains(
                VisualBasicNames.EnumName.ClassifyAt(122, 6));
        }

        [Test]
        public void DelegateTest()
        {
            GetContext(@"Access\Types\DelegateType.vb").GetClassifications().AssertContains(
                VisualBasicNames.DelegateName.ClassifyAt(113, 6));
        }
    }
}