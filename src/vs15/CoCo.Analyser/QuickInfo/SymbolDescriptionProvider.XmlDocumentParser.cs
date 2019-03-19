using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CoCo.Utils;
using Microsoft.CodeAnalysis;

namespace CoCo.Analyser.QuickInfo
{
    public partial class SymbolDescriptionProvider
    {
        private class XmlDocumentParser
        {
            private static class XmlNames
            {
                public const string CrefAttribute = "cref";
                public const string NameAttribute = "name";
                public const string ParaElement = "para";
                public const string ParameterRefElement = "paramref";
                public const string SeeElement = "see";
                public const string SeeAlsoElement = "seealso";
                public const string SummaryElement = "summary";
                public const string TypeParameterRefElement = "typeparamref";
                public const string ExceptionElement = "exception";

                /// <summary>
                /// Represents a couple of keywords (such null, true, false and so on)
                /// </summary>
                public const string LangwordAttribute = "langword";
            }

            private readonly SymbolDescriptionProvider _provider;
            private readonly ISymbol _symbol;
            private readonly SymbolDisplayPart _lineBreak = new SymbolDisplayPart(SymbolDisplayPartKind.LineBreak, null, "\r\n");

            private bool _indentWasApplied;
            private SymbolDescriptionKind currentDescription;
            private int _lineBrokenCount;

            private Dictionary<SymbolDescriptionKind, int> _indentions;

            private XmlDocumentParser(SymbolDescriptionProvider provider, ISymbol symbol)
            {
                _provider = provider;
                _symbol = symbol;
            }

            private bool HasAnyParts => _provider._description.TryGetValue(currentDescription, out var parts) && parts.Count > 0;

            public static void Parse(SymbolDescriptionProvider provider, ISymbol symbol)
            {
                var rawXml = "<i>" + symbol.GetDocumentationCommentXml() + "</i>";
                var comment = new XmlDocumentParser(provider, symbol);

                XDocument doc = null;
                try
                {
                    doc = XDocument.Parse(rawXml);
                }
                catch (XmlException)
                {
                    return;
                }

                comment.Parse(doc);
            }

            private void Parse(XNode node)
            {
                if (node.NodeType == XmlNodeType.Comment) return;

                if (node is XText text)
                {
                    AppendParts(_provider.CreatePart(SymbolDisplayPartKind.Text, Normalize(text.Value)).Enumerate());
                    return;
                }

                var element = (node as XDocument)?.Root ?? (XElement)node;

                var name = element.Name.LocalName;
                if (name == XmlNames.SummaryElement)
                {
                    // TODO: does changing description effect on a line breaking?
                    var oldDescription = currentDescription;
                    currentDescription = SymbolDescriptionKind.Additional;

                    foreach (var childNode in element.Nodes())
                    {
                        Parse(childNode);
                    }

                    currentDescription = oldDescription;
                    return;
                }

                if (name == XmlNames.ParaElement)
                {
                    _lineBrokenCount = 2;
                    foreach (var childNode in element.Nodes())
                    {
                        Parse(childNode);
                    }
                    _lineBrokenCount = 2;
                    return;
                }

                if (name == XmlNames.ExceptionElement)
                {
                    AppendExceptionParts(element);
                    return;
                }

                if (name == XmlNames.SeeElement || name == XmlNames.SeeAlsoElement)
                {
                    foreach (var attribute in element.Attributes())
                    {
                        AppendAttributeParts(name, attribute, XmlNames.CrefAttribute);
                    }
                    return;
                }

                if (name == XmlNames.ParameterRefElement || name == XmlNames.TypeParameterRefElement)
                {
                    foreach (var attribute in element.Attributes())
                    {
                        AppendAttributeParts(name, attribute, XmlNames.NameAttribute);
                    }
                    return;
                }

                foreach (var childNode in element.Nodes())
                {
                    Parse(childNode);
                }
            }

            private void AppendExceptionParts(XElement element)
            {
                if (_indentions is null)
                {
                    _indentions = new Dictionary<SymbolDescriptionKind, int>();
                }

                var oldDescription = currentDescription;
                currentDescription = SymbolDescriptionKind.Exceptions;

                if (!HasAnyParts)
                {
                    AppendParts(_provider.CreatePart(SymbolDisplayPartKind.Text, "\r\nExceptions:").Enumerate());
                    _indentions[SymbolDescriptionKind.Exceptions] = 2;
                    _lineBrokenCount = 1;
                }

                foreach (var attribute in element.Attributes())
                {
                    AppendAttributeParts(element.Name.LocalName, attribute, XmlNames.CrefAttribute);
                }

                _indentWasApplied = false;

                _lineBrokenCount = 1;
                ++_indentions[SymbolDescriptionKind.Exceptions];
                foreach (var childNode in element.Nodes())
                {
                    Parse(childNode);
                }
                --_indentions[SymbolDescriptionKind.Exceptions];
                _lineBrokenCount = 1;

                currentDescription = oldDescription;
            }

            private void AppendAttributeParts(string elementName, XAttribute attribute, string refAttributeName)
            {
                var attributeName = attribute.Name.LocalName;
                // NOTE: if attribute is expected => get parts from it, otherwise just add it as one text part
                if (refAttributeName == attributeName)
                {
                    AppendParts(RefToParts(elementName, attribute.Value));
                }
                else
                {
                    var partKind = attributeName == XmlNames.LangwordAttribute
                        ? SymbolDisplayPartKind.Keyword
                        : SymbolDisplayPartKind.Text;
                    AppendParts(_provider.CreatePart(partKind, attribute.Value).Enumerate());
                }
            }

            private IEnumerable<SymbolDisplayPart> RefToParts(string elementName, string refValue)
            {
                var semanticModel = _provider._semanticModel;
                if (!(semanticModel is null))
                {
                    var symbol = DocumentationCommentId.GetFirstSymbolForDeclarationId(refValue, semanticModel.Compilation);
                    if (!(symbol is null)) return symbol.ToMinimalDisplayParts(semanticModel, _provider._position, _crefFormat);
                    if (TryProcessRef(elementName, refValue, out var part)) return part.Enumerate();
                }
                return _provider.CreatePart(SymbolDisplayPartKind.Text, TrimRefPrefix(refValue)).Enumerate();
            }

            private bool TryProcessRef(string elementName, string refValue, out SymbolDisplayPart part)
            {
                IMethodSymbol method;
                if (elementName == XmlNames.ParameterRefElement)
                {
                    method = _symbol as IMethodSymbol;
                    if (method is null)
                    {
                        part = default;
                        return false;
                    }

                    method = method.IsExtensionMethod()
                        ? method.GetConstructedReducedFrom()
                        : method;

                    foreach (var item in method.Parameters)
                    {
                        if (item.Name.Equals(refValue))
                        {
                            part = new SymbolDisplayPart(SymbolDisplayPartKind.ParameterName, item, TrimRefPrefix(item.Name));
                            return true;
                        }
                    }
                }
                else if (elementName == XmlNames.TypeParameterRefElement)
                {
                    INamedTypeSymbol type = null;
                    method = _symbol as IMethodSymbol;
                    if (!(method is null))
                    {
                        foreach (var item in method.TypeParameters)
                        {
                            if (item.Name.Equals(refValue))
                            {
                                part = new SymbolDisplayPart(SymbolDisplayPartKind.TypeParameterName, item, TrimRefPrefix(item.Name));
                                return true;
                            }
                        }
                        type = method.ContainingType;
                    }

                    type = type ?? _symbol as INamedTypeSymbol;
                    if (type is null)
                    {
                        part = default;
                        return false;
                    }

                    foreach (var item in type.TypeParameters)
                    {
                        if (item.Name.Equals(refValue))
                        {
                            part = new SymbolDisplayPart(SymbolDisplayPartKind.TypeParameterName, item, TrimRefPrefix(item.Name));
                            return true;
                        }
                    }
                }

                part = default;
                return false;
            }

            private void AppendParts<T>(T parts) where T : IEnumerable<SymbolDisplayPart>
            {
                if (currentDescription != SymbolDescriptionKind.None)
                {
                    if (_lineBrokenCount > 0)
                    {
                        if (HasAnyParts)
                        {
                            while (_lineBrokenCount-- > 0) _provider.AppendParts(currentDescription, _lineBreak);
                        }
                        _lineBrokenCount = 0;
                        _indentWasApplied = false;
                    }
                    if (!(_indentions is null) && _indentions.TryGetValue(currentDescription, out var indentions) &&
                        HasAnyParts && !_indentWasApplied)
                    {
                        _indentWasApplied = true;
                        _provider.AppendParts(currentDescription, _provider.CreateSpaces(indentions));
                    }

                    _provider.AppendParts(currentDescription, parts);
                }
            }

            /// <summary>
            /// Normalize <paramref name="text"/> by <see cref="currentDescription"/>
            /// </summary>
            private string Normalize(string text)
            {
                var builder =  StringBuilderCache.Acquire();
                var currentIsWhiteSpace = false;
                foreach (var item in text)
                {
                    if (char.IsWhiteSpace(item))
                    {
                        currentIsWhiteSpace = true;
                    }
                    else
                    {
                        if (currentIsWhiteSpace)
                        {
                            currentIsWhiteSpace = false;
                            // NOTE: skip whitespaces if still doesn't add anything
                            if (HasAnyParts || builder.Length > 0)
                            {
                                builder.Append(' ');
                            }
                        }
                        builder.Append(item);
                    }
                }

                if (currentIsWhiteSpace)
                {
                    builder.Append(' ');
                }

                return StringBuilderCache.Release(builder);
            }

            /// <summary>
            /// Removes ref prefix likes "M:" in "M:Namespace..."
            /// </summary>
            private static string TrimRefPrefix(string value) =>
                value.Length > 1 && value[1] == ':'
                    ? value.Substring(2)
                    : value;
        }
    }
}