using CoCo.Analyser.Classifications.CSharp;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.CSharp.Declarations
{
    internal class Types : CSharpIdentifierTests
    {
        [Test]
        public void TypeParameterTest()
        {
            GetContext(@"Declarations\Types\TypeParameter.cs")
                .AddInfo(CSharpNames.TypeParameterName.EnableInEditor())
                .GetClassifications().AssertContains(
                    CSharpNames.TypeParameterName.ClassifyAt(84, 3),
                    CSharpNames.TypeParameterName.ClassifyAt(124, 6));
        }

        [Test]
        public void ClassTest()
        {
            GetContext(@"Declarations\Types\ClassType.cs")
                .AddInfo(CSharpNames.ClassName.EnableInEditor())
                .GetClassifications().AssertContains(
                    CSharpNames.ClassName.ClassifyAt(70, 9));
        }

        [Test]
        public void StructureTest()
        {
            GetContext(@"Declarations\Types\StructureType.cs")
                .AddInfo(CSharpNames.StructureName.EnableInEditor())
                .GetClassifications().AssertContains(
                    CSharpNames.StructureName.ClassifyAt(71, 13));
        }

        [Test]
        public void InterfaceTest()
        {
            GetContext(@"Declarations\Types\InterfaceType.cs")
                .AddInfo(CSharpNames.InterfaceName.EnableInEditor())
                .GetClassifications().AssertContains(
                    CSharpNames.InterfaceName.ClassifyAt(74, 13));
        }

        [Test]
        public void EnumTest()
        {
            GetContext(@"Declarations\Types\EnumType.cs")
                .AddInfo(CSharpNames.EnumName.EnableInEditor())
                .GetClassifications().AssertContains(
                    CSharpNames.EnumName.ClassifyAt(69, 8));
        }

        [Test]
        public void DelegateTest()
        {
            GetContext(@"Declarations\Types\DelegateType.cs")
                .AddInfo(CSharpNames.DelegateName.EnableInEditor())
                .GetClassifications().AssertContains(
                    CSharpNames.DelegateName.ClassifyAt(78, 6));
        }
    }
}