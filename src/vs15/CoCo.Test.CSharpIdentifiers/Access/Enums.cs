using CoCo.Analyser;
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
            GetClassifications(@"Access\EnumField.cs").AssertContains(
                CSharpNames.EnumFieldName.ClassifyAt(152, 9),
                CSharpNames.EnumFieldName.ClassifyAt(209, 3));
        }
    }
}