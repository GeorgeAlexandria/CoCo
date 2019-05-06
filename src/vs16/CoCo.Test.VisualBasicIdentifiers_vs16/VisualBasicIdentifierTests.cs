using CoCo.MsBuild;
using CoCo.Test.Common;

namespace CoCo.Test.VisualBasicIdentifiers
{
    internal class VisualBasicIdentifierTests : CommonTests
    {
        private static readonly string _projectPath = @"tests\Identifiers\VisualBasicIdentifiers\VisualBasicIdentifiers.vbproj";

        private static readonly ProjectInfo _projectInfo;

        protected override ProjectInfo ProjectInfo => _projectInfo;

        // NOTE: workaround to initialize project only once for all of instance a derived classes
        static VisualBasicIdentifierTests()
        {
            _projectInfo = SetUp(ref _projectPath);
        }
    }
}