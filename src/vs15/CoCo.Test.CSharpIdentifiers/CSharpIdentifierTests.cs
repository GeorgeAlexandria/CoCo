using CoCo.MsBuild;
using CoCo.Test.Common;

namespace CoCo.Test.CSharpIdentifiers
{
    internal class CSharpIdentifierTests : CommonTests
    {
        private static readonly string _projectPath = @"Tests\Identifiers\CSharpIdentifiers\CSharpIdentifiers.csproj";

        private static readonly ProjectInfo _projectInfo;

        protected override string ProjectPath => _projectPath;

        protected override ProjectInfo ProjectInfo => _projectInfo;

        // NOTE: workaround to initialize project only once for all of instance a derived classes
        static CSharpIdentifierTests()
        {
            _projectInfo = SetUp(ref _projectPath);
        }
    }
}