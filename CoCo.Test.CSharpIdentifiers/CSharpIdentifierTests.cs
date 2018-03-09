using CoCo.Test.Common;

namespace CoCo.Test.CSharpIdentifiers
{
    internal class CSharpIdentifierTests : CommonTests
    {
        private static string _projectPath = @"Tests\CSharpIdentifiers\CSharpIdentifiers\CSharpIdentifiers.csproj";

        private static ProjectInfo _projectInfo;

        protected override string ProjectPath { get; set; } = _projectPath;

        protected override ProjectInfo ProjectInfo { get; set; } = _projectInfo;

        // NOTE: workaround to initialize project only once for all of instance a derived classes
        static CSharpIdentifierTests()
        {
            _projectInfo = SetUp(ref _projectPath);
        }
    }
}