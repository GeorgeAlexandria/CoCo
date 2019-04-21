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
            GetContext(@"Access\Types\TypeParameter.vb")
                .AddInfo(VisualBasicNames.TypeParameterName.EnableInEditor())
                .GetClassifications().AssertContains(
                    VisualBasicNames.TypeParameterName.ClassifyAt(87, 6));
        }

        [Test]
        public void ClassTest()
        {
            GetContext(@"Access\Types\ClassType.vb")
                .AddInfo(VisualBasicNames.ClassName.EnableInEditor())
                .GetClassifications().AssertContains(
                    VisualBasicNames.ClassName.ClassifyAt(74, 9));
        }

        [Test]
        public void StructureTest()
        {
            GetContext(@"Access\Types\StructureType.vb")
                .AddInfo(VisualBasicNames.StructureName.EnableInEditor())
                .GetClassifications().AssertContains(
                    VisualBasicNames.StructureName.ClassifyAt(102, 13));
        }

        [Test]
        public void ModuleTest()
        {
            GetContext(@"Access\Types\ModuleType.vb")
                .AddInfo(VisualBasicNames.ModuleName.EnableInEditor())
                .GetClassifications().AssertContains(
                    VisualBasicNames.ModuleName.ClassifyAt(121, 10));
        }

        [Test]
        public void InterfaceTest()
        {
            GetContext(@"Access\Types\InterfaceType.vb")
                .AddInfo(VisualBasicNames.InterfaceName.EnableInEditor())
                .GetClassifications().AssertContains(
                    VisualBasicNames.InterfaceName.ClassifyAt(138, 14));
        }

        [Test]
        public void EnumTest()
        {
            GetContext(@"Access\Types\EnumType.vb")
                .AddInfo(VisualBasicNames.EnumName.EnableInEditor())
                .GetClassifications().AssertContains(
                    VisualBasicNames.EnumName.ClassifyAt(122, 6));
        }

        [Test]
        public void DelegateTest()
        {
            GetContext(@"Access\Types\DelegateType.vb")
                .AddInfo(VisualBasicNames.DelegateName.EnableInEditor())
                .GetClassifications().AssertContains(
                    VisualBasicNames.DelegateName.ClassifyAt(113, 6));
        }
    }
}