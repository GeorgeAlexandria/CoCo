using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Declarations
{
    internal class Members : VisualBasicIdentifierTests
    {
        [Test]
        public void MemberTest_InstansField()
        {
            GetClassifications(@"Declarations\Members\InstanceField.vb")
                .AssertContains(VisualBasicNames.FieldName.ClassifyAt(39, 5));
        }

        [Test]
        public void MemberTest_TypeField()
        {
            GetClassifications(@"Declarations\Members\TypeField.vb")
                .AssertContains(VisualBasicNames.FieldName.ClassifyAt(42, 5));
        }

        [Test]
        public void MemberTest_ModuleField()
        {
            GetClassifications(@"Declarations\Members\ModuleField.vb")
                .AssertContains(VisualBasicNames.FieldName.ClassifyAt(31, 5));
        }

        [Test]
        public void MemberTest_Constant()
        {
            GetClassifications(@"Declarations\Members\ConstantMember.vb")
                .AssertContains(VisualBasicNames.ConstantFieldName.ClassifyAt(46, 8));
        }
    }
}