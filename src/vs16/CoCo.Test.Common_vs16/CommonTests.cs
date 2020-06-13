using NUnit.Framework;

namespace CoCo.Test.Common
{
    [TestFixture]
    public abstract class CommonTests
    {
        protected abstract MsBuild.ProjectInfo ProjectInfo { get; }

        public static MsBuild.ProjectInfo SetUp(ref string projectPath)
        {
            projectPath = TestHelper.GetPathRelativeToTest(projectPath);
            return MsBuild.MsBuild.GetProject(projectPath);
        }

        protected TestExecutionContext GetContext(string path) => new TestExecutionContext(path, ProjectInfo);
    }
}