using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace CoCo.Analyser.QuickInfo
{
    public partial class SymbolDescriptionProvider
    {
        private class Comment
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
            }

            private readonly SymbolDescriptionProvider _provider;
            private readonly SymbolDisplayPart _lineBreak = new SymbolDisplayPart(SymbolDisplayPartKind.LineBreak, null, "\r\n");

            private SymbolDescriptionKind currentDescription;
            private int _lineBrokenCount;

            private Dictionary<SymbolDescriptionKind, int> _indentions;

            private Comment(SymbolDescriptionProvider provider)
            {
                _provider = provider;
            }

            private bool HasAnyParts => _provider._description.TryGetValue(currentDescription, out var parts) && parts.Count > 0;

            public static void Parse(SymbolDescriptionProvider provider, string xml)
            {
                var rawXml = "<i>" + xml + "</i>";
                var comment = new Comment(provider);

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
                    AppendParts(new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, Normalize(text.Value)).Enumerate());
                    return;
                }

                var element = (node as XDocument)?.Root ?? (XElement)node;

                var name = element.Name;
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
                        AppendAttributeParts(attribute, XmlNames.CrefAttribute);
                    }
                    return;
                }

                if (name == XmlNames.ParameterRefElement || name == XmlNames.TypeParameterRefElement)
                {
                    foreach (var attribute in element.Attributes())
                    {
                        AppendAttributeParts(attribute, XmlNames.NameAttribute);
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
                    AppendParts(new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, "\r\nExceptions:").Enumerate());
                    _indentions[SymbolDescriptionKind.Exceptions] = 2;
                    _lineBrokenCount = 1;
                }

                foreach (var attribute in element.Attributes())
                {
                    AppendAttributeParts(attribute, XmlNames.CrefAttribute);
                }

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

            private void AppendAttributeParts(XAttribute attribute, string refAttributeName)
            {
                // NOTE: if attribute is expected => get parts from it, otherwise just add it as one text part
                if (refAttributeName == attribute.Name.LocalName)
                {
                    AppendParts(RefToParts(attribute.Value));
                }
                else
                {
                    AppendParts(new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, attribute.Value).Enumerate());
                }
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
                    }
                    if (!(_indentions is null) && _indentions.TryGetValue(currentDescription, out var indentions) && HasAnyParts)
                    {
                        _provider.AppendParts(currentDescription, _provider.CreateSpaces(indentions));
                    }

                    _provider.AppendParts(currentDescription, parts);
                }
            }

            private IEnumerable<SymbolDisplayPart> RefToParts(string refValue)
            {
                var semanticModel = _provider._semanticModel;
                if (!(semanticModel is null))
                {
                    var symbol = DocumentationCommentId.GetFirstSymbolForDeclarationId(refValue, semanticModel.Compilation);
                    if (!(symbol is null))
                    {
                        return symbol.ToMinimalDisplayParts(semanticModel, _provider._position, _crefFormat);
                    }
                }

                return new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, TrimRefPrefix(refValue)).Enumerate();
            }

            /// <summary>
            /// Normalize <paramref name="text"/> by <see cref="currentDescription"/>
            /// </summary>
            private string Normalize(string text)
            {
                var builder = new StringBuilder();
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

                return builder.ToString();
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