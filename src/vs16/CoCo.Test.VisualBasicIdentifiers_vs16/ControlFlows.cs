using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers
{
    internal class ControlFlows : VisualBasicIdentifierTests
    {
        [Test]
        public void ControlFlowsTest_DoLoop()
        {
            GetContext(@"ControlFlows\DoLoop.vb").GetClassifications().AssertContains(
                VisualBasicNames.ControlFlowName.ClassifyAt(49, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(52, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(72, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(84, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(87, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(109, 8),
                VisualBasicNames.ControlFlowName.ClassifyAt(118, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(126, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(138, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(141, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(163, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(168, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(176, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(188, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(196, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(201, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(223, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(226, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(246, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(258, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(266, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(271, 5));
        }

        [Test]
        public void ControlFlowsTest_Exit()
        {
            GetContext(@"ControlFlows\Exit.vb").GetClassifications().AssertNotContains(
                VisualBasicNames.ControlFlowName.ClassifyAt(43, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(48, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(119, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(124, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(169, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(174, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(198, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(203, 8),
                VisualBasicNames.ControlFlowName.ClassifyAt(283, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(288, 8),
                VisualBasicNames.ControlFlowName.ClassifyAt(349, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(354, 8));
        }

        [Test]
        public void ControlFlowsTest_For()
        {
            var classifications = GetContext(@"ControlFlows\For.vb").GetClassifications();

            classifications.AssertNotContains(
                VisualBasicNames.ControlFlowName.ClassifyAt(48, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(73, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(85, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(117, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(129, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(156, 8),
                VisualBasicNames.ControlFlowName.ClassifyAt(165, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(174, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(186, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(213, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(218, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(227, 4));

            classifications.AssertNotContains(
                VisualBasicNames.ControlFlowName.ClassifyAt(62, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(99, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(105, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(143, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(200, 2));
        }

        [Test]
        public void ControlFlowsTest_Foreach()
        {
            var classifications = GetContext(@"ControlFlows\Foreach.vb").GetClassifications();

            classifications.AssertNotContains(
                VisualBasicNames.ControlFlowName.ClassifyAt(50, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(54, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(83, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(95, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(99, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(130, 8),
                VisualBasicNames.ControlFlowName.ClassifyAt(139, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(148, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(160, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(164, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(195, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(200, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(209, 4));

            classifications.AssertNotContains(
                VisualBasicNames.ControlFlowName.ClassifyAt(65, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(110, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(175, 2));
        }

        [Test]
        public void ControlFlowsTest_Goto()
        {
            GetContext(@"ControlFlows\Goto.vb").GetClassifications().AssertNotContains(
                VisualBasicNames.ControlFlowName.ClassifyAt(43, 4));
        }

        [Test]
        public void ControlFlowsTest_If()
        {
            GetContext(@"ControlFlows\If.vb").GetClassifications().AssertNotContains(
                VisualBasicNames.ControlFlowName.ClassifyAt(78, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(90, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(124, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(136, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(161, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(195, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(207, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(219, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(223, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(235, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(247, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(259, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(271, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(275, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(287, 2),
                VisualBasicNames.ControlFlowName.ClassifyAt(299, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(311, 6),
                VisualBasicNames.ControlFlowName.ClassifyAt(327, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(339, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(351, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(355, 2));
        }

        [Test]
        public void ControlFlowsTest_Iterator()
        {
            GetContext(@"ControlFlows\Iterator.vb").GetClassifications().AssertNotContains(
                VisualBasicNames.ControlFlowName.ClassifyAt(89, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(102, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(115, 5));
        }

        [Test]
        public void ControlFlowsTest_Return()
        {
            GetContext(@"ControlFlows\Return.vb").GetClassifications().AssertNotContains(
                VisualBasicNames.ControlFlowName.ClassifyAt(45, 6),
                VisualBasicNames.ControlFlowName.ClassifyAt(104, 6));
        }

        [Test]
        public void ControlFlowsTest_Select()
        {
            GetContext(@"ControlFlows\Select.vb").GetClassifications().AssertNotContains(
                VisualBasicNames.ControlFlowName.ClassifyAt(51, 6),
                VisualBasicNames.ControlFlowName.ClassifyAt(58, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(74, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(95, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(100, 6),
                VisualBasicNames.ControlFlowName.ClassifyAt(114, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(134, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(154, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(159, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(169, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(173, 6));
        }

        [Test]
        public void ControlFlowsTest_Throw()
        {
            GetContext(@"ControlFlows\Throw.vb").GetClassifications().AssertNotContains(
                VisualBasicNames.ControlFlowName.ClassifyAt(77, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(162, 5));
        }

        [Test]
        public void ControlFlowsTest_While()
        {
            GetContext(@"ControlFlows\While.vb").GetClassifications().AssertNotContains(
                VisualBasicNames.ControlFlowName.ClassifyAt(50, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(70, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(74, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(87, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(109, 8),
                VisualBasicNames.ControlFlowName.ClassifyAt(118, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(129, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(133, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(146, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(168, 4),
                VisualBasicNames.ControlFlowName.ClassifyAt(173, 5),
                VisualBasicNames.ControlFlowName.ClassifyAt(184, 3),
                VisualBasicNames.ControlFlowName.ClassifyAt(188, 5));
        }
    }
}