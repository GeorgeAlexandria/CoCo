using CoCo.Analyser.Classifications.CSharp;
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
            GetContext(@"Declarations\Members\InstanceField.cs").GetClassifications().AssertContains(
                CSharpNames.FieldName.ClassifyAt(113, 5));
        }

        [Test]
        public void MemberTest_InstanceProperty()
        {
            GetContext(@"Declarations\Members\InstanceProperty.cs").GetClassifications().AssertContains(
                CSharpNames.PropertyName.ClassifyAt(116, 10));
        }

        [Test]
        public void MemberTest_InstanceEvent()
        {
            GetContext(@"Declarations\Members\InstanceEvent.cs").GetClassifications().AssertContains(
                CSharpNames.EventName.ClassifyAt(145, 7));
        }

        [Test]
        public void MemberTest_ConstantMember()
        {
            GetContext(@"Declarations\Members\ConstantMember.cs").GetClassifications().AssertContains(
                CSharpNames.ConstantFieldName.ClassifyAt(124, 5));
        }

        // TODO: add a special classifications for Type' members ?
        [Test]
        public void MemberTest_TypeField()
        {
            GetContext(@"Declarations\Members\TypeField.cs").GetClassifications().AssertContains(
                CSharpNames.FieldName.ClassifyAt(123, 5));
        }

        [Test]
        public void MemberTest_TypeProperty()
        {
            GetContext(@"Declarations\Members\TypeProperty.cs").GetClassifications().AssertContains(
                CSharpNames.PropertyName.ClassifyAt(126, 8));
        }

        [Test]
        public void MemberTest_TypeEvent()
        {
            GetContext(@"Declarations\Members\TypeEvent.cs").GetClassifications().AssertContains(
                CSharpNames.EventName.ClassifyAt(155, 7));
        }
    }
}