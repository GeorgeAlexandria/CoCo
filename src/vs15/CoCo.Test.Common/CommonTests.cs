using System.Collections.Generic;
using CoCo.MsBuild;
using NUnit.Framework;

namespace CoCo.Test.Common
{
    [TestFixture]
    public abstract class CommonTests
    {
        protected abstract ProjectInfo ProjectInfo { get; }

        public static ProjectInfo SetUp(ref string projectPath)
        {
            projectPath = TestHelper.GetPathRelativeToTest(projectPath);
            return MsBuild.MsBuild.GetProject(projectPath);
        }

        protected List<SimplifiedClassificationSpan> GetClassifications(string path) =>
            ClassificationHelper.GetClassifications(path, ProjectInfo);

        protected TestExecutionContext GetContext(string path) => new TestExecutionContext(path, ProjectInfo);
    }
}