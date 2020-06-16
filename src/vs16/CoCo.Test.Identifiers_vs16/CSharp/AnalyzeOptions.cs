using CoCo.Analyser.Classifications.CSharp;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.CSharp
{
    internal class AnalyzeOptions : CSharpIdentifierTests
    {
        [Test]
        public void AnalyzeOptionTest_LocalVariable()
        {
            GetContext(@"AnalyzeOptions\LocalVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(145, 5));
        }

        [Test]
        public void AnalyzeOptionTest_DisableLocalVariable()
        {
            GetContext(@"AnalyzeOptions\LocalVariable.cs")
                .AddInfo(
                    CSharpNames.LocalVariableName.DisableInEditor())
                .GetClassifications().AssertNotContains(
                    CSharpNames.LocalVariableName.ClassifyAt(145, 5));
        }

        [Test]
        public void AnalyzeOptionTest_Member()
        {
            GetContext(@"AnalyzeOptions\Member.cs").GetClassifications().AssertContains(
                CSharpNames.PropertyName.ClassifyAt(127, 4),
                CSharpNames.PropertyName.ClassifyAt(182, 4));
        }

        [Test]
        public void AnalyzeOptionTest_DisableMember()
        {
            GetContext(@"AnalyzeOptions\Member.cs")
                .AddInfo(
                    CSharpNames.PropertyName.DisableInEditor())
                .GetClassifications().AssertNotContains(
                    CSharpNames.PropertyName.ClassifyAt(127, 4),
                    CSharpNames.PropertyName.ClassifyAt(182, 4));
        }

        [Test]
        public void AnalyzeOptionTest_DisableMemberInXml()
        {
            GetContext(@"AnalyzeOptions\Member.cs")
                .AddInfo(
                    CSharpNames.PropertyName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    CSharpNames.PropertyName.ClassifyAt(127, 4));
        }

        [Test]
        public void AnalyzeOptionTest_Method()
        {
            GetContext(@"AnalyzeOptions\Method.cs").GetClassifications().AssertContains(
                CSharpNames.MethodName.ClassifyAt(127, 8),
                CSharpNames.MethodName.ClassifyAt(184, 8));
        }

        [Test]
        public void AnalyzeOptionTest_DisableMethod()
        {
            GetContext(@"AnalyzeOptions\Method.cs")
                .AddInfo(
                    CSharpNames.MethodName.DisableInEditor())
                .GetClassifications().AssertNotContains(
                    CSharpNames.MethodName.ClassifyAt(127, 8),
                    CSharpNames.MethodName.ClassifyAt(184, 8));
        }

        [Test]
        public void AnalyzeOptionTest_DisableMethodInXml()
        {
            GetContext(@"AnalyzeOptions\Method.cs")
                .AddInfo(
                    CSharpNames.MethodName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    CSharpNames.MethodName.ClassifyAt(127, 8));
        }

        [Test]
        public void AnalyzeOptionTest_Namespace()
        {
            GetContext(@"AnalyzeOptions\Namespace.cs").GetClassifications().AssertContains(
                CSharpNames.NamespaceName.ClassifyAt(30, 17),
                CSharpNames.NamespaceName.ClassifyAt(48, 14),
                CSharpNames.NamespaceName.ClassifyAt(93, 17),
                CSharpNames.NamespaceName.ClassifyAt(111, 14));
        }

        [Test]
        public void AnalyzeOptionTest_DisableNamespace()
        {
            GetContext(@"AnalyzeOptions\Namespace.cs")
                .AddInfo(
                    CSharpNames.NamespaceName.DisableInEditor())
                .GetClassifications().AssertNotContains(
                    CSharpNames.NamespaceName.ClassifyAt(30, 17),
                    CSharpNames.NamespaceName.ClassifyAt(48, 14),
                    CSharpNames.NamespaceName.ClassifyAt(93, 17),
                    CSharpNames.NamespaceName.ClassifyAt(111, 14));
        }

        [Test]
        public void AnalyzeOptionTest_DisableNamespaceInXml()
        {
            GetContext(@"AnalyzeOptions\Namespace.cs")
                .AddInfo(
                    CSharpNames.NamespaceName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    CSharpNames.NamespaceName.ClassifyAt(30, 17),
                    CSharpNames.NamespaceName.ClassifyAt(48, 14));
        }

        [Test]
        public void AnalyzeOptionTest_AliasNamespace()
        {
            GetContext(@"AnalyzeOptions\AliasNamespace.cs").GetClassifications().AssertContains(
                CSharpNames.AliasNamespaceName.ClassifyAt(6, 2),
                CSharpNames.AliasNamespaceName.ClassifyAt(63, 2));
        }

        [Test]
        public void AnalyzeOptionTest_DisableAliasNamespace()
        {
            GetContext(@"AnalyzeOptions\AliasNamespace.cs")
                .AddInfo(
                    CSharpNames.AliasNamespaceName.DisableInEditor())
                .GetClassifications().AssertNotContains(
                    CSharpNames.AliasNamespaceName.ClassifyAt(6, 2),
                    CSharpNames.AliasNamespaceName.ClassifyAt(63, 2));
        }

        [Test]
        public void AnalyzeOptionTest_DisableAliasNamespaceInXml()
        {
            GetContext(@"AnalyzeOptions\AliasNamespace.cs")
                .AddInfo(
                    CSharpNames.AliasNamespaceName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    CSharpNames.AliasNamespaceName.ClassifyAt(63, 2));
        }

        [Test]
        public void AnalyzeOptionTest_Parameter()
        {
            GetContext(@"AnalyzeOptions\Parameter.cs").GetClassifications().AssertContains(
                CSharpNames.ParameterName.ClassifyAt(109, 9),
                CSharpNames.ParameterName.ClassifyAt(164, 9));
        }

        [Test]
        public void AnalyzeOptionTest_DisableParameter()
        {
            GetContext(@"AnalyzeOptions\Parameter.cs")
                .AddInfo(
                    CSharpNames.ParameterName.DisableInEditor())
                .GetClassifications().AssertNotContains(
                    CSharpNames.ParameterName.ClassifyAt(109, 9),
                    CSharpNames.ParameterName.ClassifyAt(164, 9));
        }

        [Test]
        public void AnalyzeOptionTest_DisableParameterInXml()
        {
            GetContext(@"AnalyzeOptions\Parameter.cs")
                .AddInfo(
                    CSharpNames.ParameterName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    CSharpNames.ParameterName.ClassifyAt(109, 9));
        }
    }
}