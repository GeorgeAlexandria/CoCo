using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class Enums : CSharpIdentifierTests
    {
        [Test]
        public void EnumTest()
        {
            GetContext(@"Access\EnumField.cs").GetClassifications().AssertContains(
                CSharpNames.EnumFieldName.ClassifyAt(152, 9),
                CSharpNames.EnumFieldName.ClassifyAt(209, 3));
        }
    }
}