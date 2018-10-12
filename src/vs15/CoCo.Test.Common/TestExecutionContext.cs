using System.Collections.Generic;
using CoCo.MsBuild;

namespace CoCo.Test.Common
{
    public class TestExecutionContext
    {
        private readonly List<SimplifiedClassificationInfo> _infos = new List<SimplifiedClassificationInfo>();
        public string Path;
        public ProjectInfo ProjectInfo;

        public TestExecutionContext(string path, ProjectInfo projectInfo)
        {
            Path = path;
            ProjectInfo = projectInfo;
        }

        public IReadOnlyList<SimplifiedClassificationInfo> Infos => _infos;

        public TestExecutionContext AddInfo(params SimplifiedClassificationInfo[] infos)
        {
            _infos.AddRange(infos);
            return this;
        }

        public List<SimplifiedClassificationSpan> GetClassifications() => ClassificationHelper.GetClassifications(Path, ProjectInfo, _infos);
    }
}