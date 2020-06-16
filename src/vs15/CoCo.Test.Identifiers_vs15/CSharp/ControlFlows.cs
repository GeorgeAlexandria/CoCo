using CoCo.Analyser.Classifications.CSharp;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.CSharp
{
    internal class ControlFlows : CSharpIdentifierTests
    {
        [Test]
        public void ControlFlowsTest_For()
        {
            GetContext(@"ControlFlows\For.cs").GetClassifications().AssertContains(
                CSharpNames.ControlFlowName.ClassifyAt(146, 3),
                CSharpNames.ControlFlowName.ClassifyAt(258, 3),
                CSharpNames.ControlFlowName.ClassifyAt(319, 8),
                CSharpNames.ControlFlowName.ClassifyAt(359, 3),
                CSharpNames.ControlFlowName.ClassifyAt(420, 5),
                CSharpNames.ControlFlowName.ClassifyAt(457, 3),
                CSharpNames.ControlFlowName.ClassifyAt(518, 6));
        }

        [Test]
        public void ControlFlowsTest_Foreach()
        {
            var classifications = GetContext(@"ControlFlows\Foreach.cs").GetClassifications();

            classifications.AssertNotContains(
                CSharpNames.ControlFlowName.ClassifyAt(195, 2),
                CSharpNames.ControlFlowName.ClassifyAt(264, 2),
                CSharpNames.ControlFlowName.ClassifyAt(360, 2),
                CSharpNames.ControlFlowName.ClassifyAt(453, 2));

            classifications.AssertContains(
                CSharpNames.ControlFlowName.ClassifyAt(177, 7),
                CSharpNames.ControlFlowName.ClassifyAt(246, 7),
                CSharpNames.ControlFlowName.ClassifyAt(302, 8),
                CSharpNames.ControlFlowName.ClassifyAt(342, 7),
                CSharpNames.ControlFlowName.ClassifyAt(398, 5),
                CSharpNames.ControlFlowName.ClassifyAt(435, 7),
                CSharpNames.ControlFlowName.ClassifyAt(491, 6));
        }

        [Test]
        public void ControlFlowsTest_Goto()
        {
            GetContext(@"ControlFlows\Goto.cs").GetClassifications().AssertContains(
                CSharpNames.ControlFlowName.ClassifyAt(147, 4));
        }

        [Test]
        public void ControlFlowsTest_If()
        {
            GetContext(@"ControlFlows\If.cs").GetClassifications().AssertContains(
                CSharpNames.ControlFlowName.ClassifyAt(152, 2),
                CSharpNames.ControlFlowName.ClassifyAt(247, 4),
                CSharpNames.ControlFlowName.ClassifyAt(252, 2),
                CSharpNames.ControlFlowName.ClassifyAt(347, 4));
        }

        [Test]
        public void ControlFlowsTest_Iterator()
        {
            GetContext(@"ControlFlows\Iterator.cs").GetClassifications().AssertContains(
                CSharpNames.ControlFlowName.ClassifyAt(183, 5),
                CSharpNames.ControlFlowName.ClassifyAt(189, 6),
                CSharpNames.ControlFlowName.ClassifyAt(214, 5),
                CSharpNames.ControlFlowName.ClassifyAt(220, 6),
                CSharpNames.ControlFlowName.ClassifyAt(245, 5),
                CSharpNames.ControlFlowName.ClassifyAt(251, 5));
        }

        [Test]
        public void ControlFlowsTest_Return()
        {
            GetContext(@"ControlFlows\Return.cs").GetClassifications().AssertContains(
                CSharpNames.ControlFlowName.ClassifyAt(132, 6),
                CSharpNames.ControlFlowName.ClassifyAt(207, 6));
        }

        [Test]
        public void ControlFlowsTest_Switch()
        {
            GetContext(@"ControlFlows\Switch.cs").GetClassifications().AssertContains(
                CSharpNames.ControlFlowName.ClassifyAt(156, 6),
                CSharpNames.ControlFlowName.ClassifyAt(201, 4),
                CSharpNames.ControlFlowName.ClassifyAt(230, 5),
                CSharpNames.ControlFlowName.ClassifyAt(256, 4),
                CSharpNames.ControlFlowName.ClassifyAt(286, 6),
                CSharpNames.ControlFlowName.ClassifyAt(313, 4),
                CSharpNames.ControlFlowName.ClassifyAt(343, 5),
                CSharpNames.ControlFlowName.ClassifyAt(369, 7),
                CSharpNames.ControlFlowName.ClassifyAt(441, 5));
        }

        [Test]
        public void ControlFlowsTest_Throw()
        {
            GetContext(@"ControlFlows\Throw.cs").GetClassifications().AssertContains(
                CSharpNames.ControlFlowName.ClassifyAt(148, 5),
                CSharpNames.ControlFlowName.ClassifyAt(239, 5),
                CSharpNames.ControlFlowName.ClassifyAt(404, 5));
        }

        [Test]
        public void ControlFlowsTest_While()
        {
            GetContext(@"ControlFlows\While.cs").GetClassifications().AssertContains(
                CSharpNames.ControlFlowName.ClassifyAt(155, 5),
                CSharpNames.ControlFlowName.ClassifyAt(255, 2),
                CSharpNames.ControlFlowName.ClassifyAt(326, 5),
                CSharpNames.ControlFlowName.ClassifyAt(359, 5),
                CSharpNames.ControlFlowName.ClassifyAt(404, 5),
                CSharpNames.ControlFlowName.ClassifyAt(441, 5),
                CSharpNames.ControlFlowName.ClassifyAt(490, 8),
                CSharpNames.ControlFlowName.ClassifyAt(530, 5),
                CSharpNames.ControlFlowName.ClassifyAt(575, 6));
        }

        [Test]
        public void ControlFlowsTest_NotDefault()
        {
            GetContext(@"ControlFlows\Default.cs").GetClassifications().AssertNotContains(
                CSharpNames.ControlFlowName.ClassifyAt(141, 7),
                CSharpNames.ControlFlowName.ClassifyAt(220, 7));
        }
    }
}