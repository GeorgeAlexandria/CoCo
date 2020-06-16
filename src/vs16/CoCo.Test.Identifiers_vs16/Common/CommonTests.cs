using NUnit.Framework;

namespace CoCo.Test.Identifiers.Common
{
    [TestFixture]
    public abstract class CommonTests
    {
        protected abstract ProjectInfo ProjectInfo { get; }

        public static ProjectInfo SetUp(ref string projectPath)
        {
            projectPath = TestHelper.GetPathRelativeToTest(projectPath);
            return MsBuild.GetProject(projectPath);
        }

        protected TestExecutionContext GetContext(string path) => new TestExecutionContext(path, ProjectInfo);
    }
}