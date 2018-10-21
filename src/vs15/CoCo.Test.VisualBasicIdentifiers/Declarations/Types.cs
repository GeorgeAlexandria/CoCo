using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Declarations
{
    internal class Types : VisualBasicIdentifierTests
    {
        [Test]
        public void TypeParameterTest()
        {
            GetContext(@"Declarations\Types\TypeParameter.vb")
                .AddInfo(VisualBasicNames.TypeParameterName.Enable())
                .GetClassifications().AssertContains(
                    VisualBasicNames.TypeParameterName.ClassifyAt(30, 6));
        }

        [Test]
        public void ClassTest()
        {
            GetContext(@"Declarations\Types\ClassType.vb")
                .AddInfo(VisualBasicNames.ClassName.Enable())
                .GetClassifications().AssertContains(
                    VisualBasicNames.ClassName.ClassifyAt(13, 9));
        }

        [Test]
        public void StructureTest()
        {
            GetContext(@"Declarations\Types\StructureType.vb")
                .AddInfo(VisualBasicNames.StructureName.Enable())
                .GetClassifications().AssertContains(
                    VisualBasicNames.StructureName.ClassifyAt(17, 13));
        }

        [Test]
        public void ModuleTest()
        {
            GetContext(@"Declarations\Types\ModuleType.vb")
                .AddInfo(VisualBasicNames.ModuleName.Enable())
                .GetClassifications().AssertContains(
                    VisualBasicNames.ModuleName.ClassifyAt(7, 10));
        }

        [Test]
        public void InterfaceTest()
        {
            GetContext(@"Declarations\Types\InterfaceType.vb")
                .AddInfo(VisualBasicNames.InterfaceName.Enable())
                .GetClassifications().AssertContains(
                    VisualBasicNames.InterfaceName.ClassifyAt(17, 13));
        }

        [Test]
        public void EnumTest()
        {
            GetContext(@"Declarations\Types\EnumType.vb")
                .AddInfo(VisualBasicNames.EnumName.Enable())
                .GetClassifications().AssertContains(
                    VisualBasicNames.EnumName.ClassifyAt(12, 8));
        }

        [Test]
        public void DelegateTest()
        {
            GetContext(@"Declarations\Types\DelegateType.vb")
                .AddInfo(VisualBasicNames.DelegateName.Enable())
                .GetClassifications().AssertContains(
                    VisualBasicNames.DelegateName.ClassifyAt(20, 12));
        }
    }
}