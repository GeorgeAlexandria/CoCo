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
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Members\InstanceField.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.FieldName.ClassifyAt(113, 5));
        }

        [Test]
        public void MemberTest_InstanceProperty()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Members\InstanceProperty.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.PropertyName.ClassifyAt(116, 10));
        }

        [Test]
        public void MemberTest_InstanceEvent()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Members\InstanceEvent.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.EventName.ClassifyAt(145, 7));
        }
    }
}