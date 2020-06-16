using CoCo.Test.Identifiers.Common;

namespace CoCo.Test.Identifiers.VisualBasic
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