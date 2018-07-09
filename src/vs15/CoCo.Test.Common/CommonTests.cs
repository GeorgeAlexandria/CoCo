using CoCo.MsBuild;
using NUnit.Framework;

namespace CoCo.Test.Common
{
    [TestFixture]
    public abstract class CommonTests
    {
        protected abstract string ProjectPath { get; set; }

        protected abstract ProjectInfo ProjectInfo { get; set; }

        public static ProjectInfo SetUp(ref string projectPath)
        {
            projectPath = TestHelper.GetPathRelativeToTest(projectPath);
            return MsBuild.MsBuild.CreateProject(projectPath);
        }
    }
}