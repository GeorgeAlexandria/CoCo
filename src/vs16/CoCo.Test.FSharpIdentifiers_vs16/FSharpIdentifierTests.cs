using CoCo.Test.Common;
using CoCo.Test.Common.MsBuild;

namespace CoCo.Test.FSharpIdentifiers
{
    internal class FSharpIdentifierTests : CommonTests
    {
        private static readonly string _projectPath = @"tests\Identifiers\FSharpIdentifiers\FSharpIdentifiers.fsproj";

        private static readonly ProjectInfo _projectInfo;

        protected override ProjectInfo ProjectInfo => _projectInfo;

        // NOTE: workaround to initialize project only once for all of instance a derived classes
        static FSharpIdentifierTests()
        {
            _projectInfo = SetUp(ref _projectPath);
        }
    }
}