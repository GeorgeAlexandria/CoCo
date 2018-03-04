using NUnit.Framework;

namespace CoCoTests
{
    [TestFixture]
    internal abstract class CommonTests
    {
        protected abstract string ProjectPath { get; set; }

        protected ProjectInfo ProjectInfo { get; private set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            ProjectPath = TestHelper.GetPathRelativeToTest(ProjectPath);
            ProjectInfo = new ProjectInfo(MsBuild.ResolveAssemblyReferences(ProjectPath));
        }
    }
}