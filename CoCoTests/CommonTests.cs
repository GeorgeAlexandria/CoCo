using NUnit.Framework;

namespace CoCo.Test.Common
{
    [TestFixture]
    public abstract class CommonTests
    {
        protected abstract string ProjectPath { get; set; }

        protected ProjectInfo ProjectInfo { get; private set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            ProjectPath = TestHelper.GetPathRelativeToTest(ProjectPath);
            ProjectInfo = MsBuild.CreateProject(ProjectPath);
        }
    }
}