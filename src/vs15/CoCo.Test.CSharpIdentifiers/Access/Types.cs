using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    class Types : CSharpIdentifierTests
    {
        [Test]
        public void ClassTest()
        {
            GetContext(@"Access\Types\ClassType.cs").GetClassifications().AssertContains(
                CSharpNames.ClassName.ClassifyAt(152, 9));
        }

        [Test]
        public void StructureTest()
        {
            GetContext(@"Access\Types\StructureType.cs").GetClassifications().AssertContains(
                CSharpNames.StructureName.ClassifyAt(157, 13));
        }

        [Test]
        public void InterfaceTest()
        {
            GetContext(@"Access\Types\InterfaceType.cs").GetClassifications().AssertContains(
                CSharpNames.InterfaceName.ClassifyAt(166, 14));
        }

        [Test]
        public void EnumTest()
        {
            GetContext(@"Access\Types\EnumType.cs").GetClassifications().AssertContains(
                CSharpNames.EnumName.ClassifyAt(160, 6));
        }

        [Test]
        public void DelegateTest()
        {
            GetContext(@"Access\Types\DelegateType.cs").GetClassifications().AssertContains(
                CSharpNames.DelegateName.ClassifyAt(144, 6));
        }

        [Test]
        public void TypeParameterTest()
        {
            GetContext(@"Access\Types\TypeParameter.cs").GetClassifications().AssertContains(
                CSharpNames.TypeParameterName.ClassifyAt(92, 6),
                CSharpNames.TypeParameterName.ClassifyAt(131, 6),
                CSharpNames.TypeParameterName.ClassifyAt(151, 6));
        }
    }
}
