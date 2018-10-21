using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class Types : CSharpIdentifierTests
    {
        [Test]
        public void ClassTest()
        {
            GetContext(@"Access\Types\ClassType.cs")
                .AddInfo(CSharpNames.ClassName.Enable())
                .GetClassifications().AssertContains(
                    CSharpNames.ClassName.ClassifyAt(152, 9));
        }

        [Test]
        public void StructureTest()
        {
            GetContext(@"Access\Types\StructureType.cs")
                .AddInfo(CSharpNames.StructureName.Enable())
                .GetClassifications().AssertContains(
                    CSharpNames.StructureName.ClassifyAt(157, 13));
        }

        [Test]
        public void InterfaceTest()
        {
            GetContext(@"Access\Types\InterfaceType.cs")
                .AddInfo(CSharpNames.InterfaceName.Enable())
                .GetClassifications().AssertContains(
                    CSharpNames.InterfaceName.ClassifyAt(166, 14));
        }

        [Test]
        public void EnumTest()
        {
            GetContext(@"Access\Types\EnumType.cs")
                .AddInfo(CSharpNames.EnumName.Enable())
                .GetClassifications().AssertContains(
                    CSharpNames.EnumName.ClassifyAt(160, 6));
        }

        [Test]
        public void DelegateTest()
        {
            GetContext(@"Access\Types\DelegateType.cs")
                .AddInfo(CSharpNames.DelegateName.Enable())
                .GetClassifications().AssertContains(
                    CSharpNames.DelegateName.ClassifyAt(144, 6));
        }

        [Test]
        public void TypeParameterTest()
        {
            GetContext(@"Access\Types\TypeParameter.cs")
                .AddInfo(CSharpNames.TypeParameterName.Enable())
                .GetClassifications().AssertContains(
                    CSharpNames.TypeParameterName.ClassifyAt(92, 6),
                    CSharpNames.TypeParameterName.ClassifyAt(131, 6),
                    CSharpNames.TypeParameterName.ClassifyAt(151, 6));
        }
    }
}