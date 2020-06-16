using CoCo.Analyser.Classifications.CSharp;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.CSharp.XmlDocComment
{
    [TestFixture]
    internal class XmlNode : CSharpIdentifierTests
    {
        [Test]
        public void XmlNodeTest_CrefAttribute()
        {
            GetContext(@"XmlDocComment\CrefAttribute.cs").GetClassifications().AssertContains(
                CSharpNames.MethodName.ClassifyAt(133, 6));
        }

        [Test]
        public void XmlNodeTest_CrefAttributeDisableXml()
        {
            GetContext(@"XmlDocComment\CrefAttribute.cs")
                .AddInfo(
                    CSharpNames.MethodName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    CSharpNames.MethodName.ClassifyAt(134, 6));
        }

        [Test]
        public void XmlNodeTest_NameAttribute()
        {
            GetContext(@"XmlDocComment\NameAttribute.cs").GetClassifications().AssertContains(
                CSharpNames.ParameterName.ClassifyAt(112, 3));
        }

        [Test]
        public void XmlNodeTest_NameAttributeDisableXml()
        {
            GetContext(@"XmlDocComment\NameAttribute.cs")
                .AddInfo(
                    CSharpNames.ParameterName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    CSharpNames.ParameterName.ClassifyAt(112, 3));
        }

        [Test]
        public void XmlNodeTest_Exception()
        {
            GetContext(@"XmlDocComment\Exception.cs").GetClassifications().AssertContains(
                CSharpNames.NamespaceName.ClassifyAt(71, 6));
        }

        [Test]
        public void XmlNodeTest_ExceptionDisableInXml()
        {
            GetContext(@"XmlDocComment\Exception.cs")
                .AddInfo(
                    CSharpNames.NamespaceName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    CSharpNames.NamespaceName.ClassifyAt(71, 6));
        }

        [Test]
        public void XmlNodeTest_Param()
        {
            GetContext(@"XmlDocComment\Param.cs").GetClassifications().AssertContains(
                CSharpNames.ParameterName.ClassifyAt(104, 3));
        }

        [Test]
        public void XmlNodeTest_ParamDisableInXml()
        {
            GetContext(@"XmlDocComment\Param.cs")
                .AddInfo(
                    CSharpNames.ParameterName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    CSharpNames.ParameterName.ClassifyAt(104, 3));
        }

        [Test]
        public void XmlNodeTest_ParamRef()
        {
            GetContext(@"XmlDocComment\ParamRef.cs").GetClassifications().AssertContains(
                CSharpNames.ParameterName.ClassifyAt(133, 3));
        }

        [Test]
        public void XmlNodeTest_ParamRefDisableInXml()
        {
            GetContext(@"XmlDocComment\ParamRef.cs")
                .AddInfo(
                    CSharpNames.ParameterName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    CSharpNames.ParameterName.ClassifyAt(133, 3));
        }

        [Test]
        public void XmlNodeTest_Permission()
        {
            GetContext(@"XmlDocComment\Permission.cs").GetClassifications().AssertContains(
                CSharpNames.NamespaceName.ClassifyAt(72, 6),
                CSharpNames.NamespaceName.ClassifyAt(79, 8));
        }

        [Test]
        public void XmlNodeTest_PermissionDisableInXml()
        {
            GetContext(@"XmlDocComment\Permission.cs")
                .AddInfo(
                    CSharpNames.NamespaceName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    CSharpNames.NamespaceName.ClassifyAt(72, 6),
                    CSharpNames.NamespaceName.ClassifyAt(79, 8));
        }

        [Test]
        public void XmlNodeTest_See()
        {
            GetContext(@"XmlDocComment\See.cs").GetClassifications().AssertContains(
                CSharpNames.NamespaceName.ClassifyAt(84, 6));
        }

        [Test]
        public void XmlNodeTest_SeeDisableInXml()
        {
            GetContext(@"XmlDocComment\See.cs")
                .AddInfo(
                    CSharpNames.NamespaceName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    CSharpNames.NamespaceName.ClassifyAt(84, 6));
        }

        [Test]
        public void XmlNodeTest_SeeAlso()
        {
            GetContext(@"XmlDocComment\SeeAlso.cs").GetClassifications().AssertContains(
                CSharpNames.NamespaceName.ClassifyAt(88, 6));
        }

        [Test]
        public void XmlNodeTest_SeeAlsoDisableInXml()
        {
            GetContext(@"XmlDocComment\SeeAlso.cs")
                .AddInfo(
                    CSharpNames.NamespaceName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    CSharpNames.NamespaceName.ClassifyAt(88, 6));
        }

        // TODO: typeparam currently unsupported
    }
}