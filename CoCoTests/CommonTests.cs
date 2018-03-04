using NUnit.Framework;

namespace CoCoTests
{
    [TestFixture]
    internal abstract class CommonTests
    {
        protected abstract string ProjectPath { get; set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            ProjectPath = TestHelper.GetPathRelativeToTest(ProjectPath);
        }
    }
}