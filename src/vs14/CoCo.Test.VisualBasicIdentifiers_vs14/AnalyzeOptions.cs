using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers
{
    internal class AnalyzeOptions : VisualBasicIdentifierTests
    {
        [Test]
        public void AnalyzeOptionTest_LocalVariable()
        {
            GetContext(@"AnalyzeOptions\LocalVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(61, 5));
        }

        [Test]
        public void AnalyzeOptionTest_DisableLocalVariable()
        {
            GetContext(@"AnalyzeOptions\LocalVariable.vb")
                .AddInfo(
                    VisualBasicNames.LocalVariableName.DisableInEditor())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.LocalVariableName.ClassifyAt(61, 5));
        }

        [Test]
        public void AnalyzeOptionTest_Member()
        {
            GetContext(@"AnalyzeOptions\Member.vb").GetClassifications().AssertContains(
                VisualBasicNames.PropertyName.ClassifyAt(57, 4),
                VisualBasicNames.PropertyName.ClassifyAt(102, 4));
        }

        [Test]
        public void AnalyzeOptionTest_DisableMember()
        {
            GetContext(@"AnalyzeOptions\Member.vb")
                .AddInfo(
                    VisualBasicNames.PropertyName.DisableInEditor())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.PropertyName.ClassifyAt(57, 4),
                    VisualBasicNames.PropertyName.ClassifyAt(102, 4));
        }

        [Test]
        public void AnalyzeOptionTest_DisableMemberInXml()
        {
            GetContext(@"AnalyzeOptions\Member.vb")
                .AddInfo(
                    VisualBasicNames.PropertyName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.PropertyName.ClassifyAt(57, 4));
        }

        [Test]
        public void AnalyzeOptionTest_Method()
        {
            GetContext(@"AnalyzeOptions\Method.vb").GetClassifications().AssertContains(
                VisualBasicNames.FunctionName.ClassifyAt(57, 8),
                VisualBasicNames.FunctionName.ClassifyAt(135, 8));
        }

        [Test]
        public void AnalyzeOptionTest_DisableMethod()
        {
            GetContext(@"AnalyzeOptions\Method.vb")
                .AddInfo(
                    VisualBasicNames.FunctionName.DisableInEditor())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.FunctionName.ClassifyAt(57, 8),
                    VisualBasicNames.FunctionName.ClassifyAt(135, 8));
        }

        [Test]
        public void AnalyzeOptionTest_DisableMethodInXml()
        {
            GetContext(@"AnalyzeOptions\Method.vb")
                .AddInfo(
                    VisualBasicNames.FunctionName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.FunctionName.ClassifyAt(57, 8));
        }

        [Test]
        public void AnalyzeOptionTest_Namespace()
        {
            GetContext(@"AnalyzeOptions\Namespace.vb").GetClassifications().AssertContains(
                VisualBasicNames.NamespaceName.ClassifyAt(10, 14),
                VisualBasicNames.NamespaceName.ClassifyAt(62, 14));
        }

        [Test]
        public void AnalyzeOptionTest_DisableNamespace()
        {
            GetContext(@"AnalyzeOptions\Namespace.vb")
                .AddInfo(
                    VisualBasicNames.NamespaceName.DisableInEditor())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.NamespaceName.ClassifyAt(10, 14),
                    VisualBasicNames.NamespaceName.ClassifyAt(62, 14));
        }

        [Test]
        public void AnalyzeOptionTest_DisableNamespaceInXml()
        {
            GetContext(@"AnalyzeOptions\Namespace.vb")
                .AddInfo(
                    VisualBasicNames.NamespaceName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.NamespaceName.ClassifyAt(62, 14));
        }

        [Test]
        public void AnalyzeOptionTest_Parameter()
        {
            GetContext(@"AnalyzeOptions\ParameterOption.vb").GetClassifications().AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(51, 5),
                VisualBasicNames.ParameterName.ClassifyAt(88, 5));
        }

        [Test]
        public void AnalyzeOptionTest_DisableParameter()
        {
            GetContext(@"AnalyzeOptions\ParameterOption.vb")
                .AddInfo(
                    VisualBasicNames.ParameterName.DisableInEditor())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.ParameterName.ClassifyAt(51, 5),
                    VisualBasicNames.ParameterName.ClassifyAt(88, 5));
        }

        [Test]
        public void AnalyzeOptionTest_DisableParameterInXml()
        {
            GetContext(@"AnalyzeOptions\ParameterOption.vb")
                .AddInfo(
                    VisualBasicNames.ParameterName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.ParameterName.ClassifyAt(51, 5));
        }
    }
}