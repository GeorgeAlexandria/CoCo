using CoCo.Test.Identifiers.Common;
using CoCo.Test.Identifiers.Common;

namespace CoCo.Test.Identifiers.CSharp
{
    internal class CSharpIdentifierTests : CommonTests
    {
        private static readonly string _projectPath = @"tests\Identifiers\CSharpIdentifiers\CSharpIdentifiers.csproj";

        private static readonly ProjectInfo _projectInfo;

        protected override ProjectInfo ProjectInfo => _projectInfo;

        // NOTE: workaround to initialize project only once for all of instance a derived classes
        static CSharpIdentifierTests()
        {
            _projectInfo = SetUp(ref _projectPath);
        }
    }
}