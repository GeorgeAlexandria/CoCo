using System.Collections.Generic;

namespace CoCo.Test.Identifiers.Common
{
    public class TestExecutionContext
    {
        private readonly List<SimplifiedClassificationInfo> _infos = new List<SimplifiedClassificationInfo>();
        private readonly string _path;
        private readonly ProjectInfo _projectInfo;

        public TestExecutionContext(string path, ProjectInfo projectInfo)
        {
            _path = path;
            _projectInfo = projectInfo;
        }

        public TestExecutionContext AddInfo(params SimplifiedClassificationInfo[] infos)
        {
            _infos.AddRange(infos);
            return this;
        }

        public List<SimplifiedClassificationSpan> GetClassifications() =>
            ClassificationHelper.GetClassifications(_path, _projectInfo, _infos);
    }
}