using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Enum : CSharpIdentifierTests
    {
        [Test]
        public void EnumTest()
        {
            GetContext(@"Declarations\EnumDeclaration.cs").GetClassifications().AssertContains(
                CSharpNames.EnumFieldName.ClassifyAt(84, 4),
                CSharpNames.EnumFieldName.ClassifyAt(99, 5),
                CSharpNames.EnumFieldName.ClassifyAt(115, 6),
                CSharpNames.EnumFieldName.ClassifyAt(132, 6));
        }
    }
}