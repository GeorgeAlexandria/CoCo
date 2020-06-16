using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.VisualBasic.XmlDocComment
{
    [TestFixture]
    internal class XmlNode : VisualBasicIdentifierTests
    {
        [Test]
        public void XmlNodeTest_CrefAttribute()
        {
            GetContext(@"XmlDocComment\CrefAttribute.vb").GetClassifications().AssertContains(
                VisualBasicNames.SubName.ClassifyAt(64, 6));
        }

        [Test]
        public void XmlNodeTest_CrefAttributeDisableXml()
        {
            GetContext(@"XmlDocComment\CrefAttribute.vb")
                .AddInfo(
                    VisualBasicNames.SubName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.SubName.ClassifyAt(64, 6));
        }

        [Test]
        public void XmlNodeTest_NameAttribute()
        {
            GetContext(@"XmlDocComment\NameAttribute.vb").GetClassifications().AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(49, 3));
        }

        [Test]
        public void XmlNodeTest_NameAttributeDisableXml()
        {
            GetContext(@"XmlDocComment\NameAttribute.vb")
                .AddInfo(
                    VisualBasicNames.ParameterName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.ParameterName.ClassifyAt(49, 3));
        }

        [Test]
        public void XmlNodeTest_Exception()
        {
            GetContext(@"XmlDocComment\Exception.vb").GetClassifications().AssertContains(
                VisualBasicNames.NamespaceName.ClassifyAt(21, 6));
        }

        [Test]
        public void XmlNodeTest_ExceptionDisableInXml()
        {
            GetContext(@"XmlDocComment\Exception.vb")
                .AddInfo(
                    VisualBasicNames.NamespaceName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.NamespaceName.ClassifyAt(21, 6));
        }

        [Test]
        public void XmlNodeTest_Param()
        {
            GetContext(@"XmlDocComment\Param.vb").GetClassifications().AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(41, 3));
        }

        [Test]
        public void XmlNodeTest_ParamDisableInXml()
        {
            GetContext(@"XmlDocComment\Param.vb")
                .AddInfo(
                    VisualBasicNames.ParameterName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.ParameterName.ClassifyAt(41, 3));
        }

        [Test]
        public void XmlNodeTest_ParamRef()
        {
            GetContext(@"XmlDocComment\ParamRef.vb").GetClassifications().AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(64, 3));
        }

        [Test]
        public void XmlNodeTest_ParamRefDisableInXml()
        {
            GetContext(@"XmlDocComment\ParamRef.vb")
                .AddInfo(
                    VisualBasicNames.ParameterName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.ParameterName.ClassifyAt(64, 3));
        }

        [Test]
        public void XmlNodeTest_Permission()
        {
            GetContext(@"XmlDocComment\Permission.vb").GetClassifications().AssertContains(
                VisualBasicNames.NamespaceName.ClassifyAt(22, 6),
                VisualBasicNames.NamespaceName.ClassifyAt(29, 8));
        }

        [Test]
        public void XmlNodeTest_PermissionDisableInXml()
        {
            GetContext(@"XmlDocComment\Permission.vb")
                .AddInfo(
                    VisualBasicNames.NamespaceName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.NamespaceName.ClassifyAt(22, 6),
                    VisualBasicNames.NamespaceName.ClassifyAt(29, 8));
        }

        [Test]
        public void XmlNodeTest_See()
        {
            GetContext(@"XmlDocComment\See.vb").GetClassifications().AssertContains(
                VisualBasicNames.NamespaceName.ClassifyAt(15, 6));
        }

        [Test]
        public void XmlNodeTest_SeeDisableInXml()
        {
            GetContext(@"XmlDocComment\See.vb")
                .AddInfo(
                    VisualBasicNames.NamespaceName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.NamespaceName.ClassifyAt(15, 6));
        }

        [Test]
        public void XmlNodeTest_SeeAlso()
        {
            GetContext(@"XmlDocComment\SeeAlso.vb").GetClassifications().AssertContains(
                VisualBasicNames.NamespaceName.ClassifyAt(19, 6));
        }

        [Test]
        public void XmlNodeTest_SeeAlsoDisableInXml()
        {
            GetContext(@"XmlDocComment\SeeAlso.vb")
                .AddInfo(
                    VisualBasicNames.NamespaceName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.NamespaceName.ClassifyAt(19, 6));
        }

        // TODO: typeparam currently unsupported
    }
}