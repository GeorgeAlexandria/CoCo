using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Members : CSharpIdentifierTests
    {
        // TODO: type members
        [Test]
        public void MemberTest_InstanceField()
        {
            GetClassifications(@"Declarations\Members\InstanceField.cs")
                .AssertContains(CSharpNames.FieldName.ClassifyAt(113, 5));
        }

        [Test]
        public void MemberTest_InstanceProperty()
        {
            GetClassifications(@"Declarations\Members\InstanceProperty.cs")
                .AssertContains(CSharpNames.PropertyName.ClassifyAt(116, 10));
        }

        [Test]
        public void MemberTest_InstanceEvent()
        {
            GetClassifications(@"Declarations\Members\InstanceEvent.cs")
                .AssertContains(CSharpNames.EventName.ClassifyAt(145, 7));
        }

        [Test]
        public void MemberTest_ConstantMember()
        {
            GetClassifications(@"Declarations\Members\ConstantMember.cs")
                .AssertContains(CSharpNames.ConstantFieldName.ClassifyAt(124, 5));
        }

        // TODO: add a special classifications for Type' members ?
        [Test]
        public void MemberTest_TypeField()
        {
            GetClassifications(@"Declarations\Members\TypeField.cs")
                .AssertContains(CSharpNames.FieldName.ClassifyAt(123, 5));
        }

        [Test]
        public void MemberTest_TypeProperty()
        {
            GetClassifications(@"Declarations\Members\TypeProperty.cs")
                .AssertContains(CSharpNames.PropertyName.ClassifyAt(126, 8));
        }

        [Test]
        public void MemberTest_TypeEvent()
        {
            GetClassifications(@"Declarations\Members\TypeEvent.cs")
                .AssertContains(CSharpNames.EventName.ClassifyAt(155, 7));
        }
    }
}