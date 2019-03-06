using System.Collections.Generic;
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
            }

            private readonly SymbolDescriptionProvider _provider;
            private readonly SymbolDisplayPart _lineBreak = new SymbolDisplayPart(SymbolDisplayPartKind.LineBreak, null, "\r\n");

            private SymbolDescriptionKind currentDescription;
            private bool _lineWasBroken;

            private Comment(SymbolDescriptionProvider provider)
            {
                _provider = provider;
            }

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
                    AppendParts(Enumerate(new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, Normalize(text.Value))));
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
                    _lineWasBroken = true;
                    foreach (var childNode in element.Nodes())
                    {
                        Parse(childNode);
                    }
                    _lineWasBroken = true;
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

            private void AppendAttributeParts(XAttribute attribute, string refAttributeName)
            {
                // NOTE: if attribute is expected => get parts from it, otherwise just add it as one text part
                if (refAttributeName == attribute.Name.LocalName)
                {
                    AppendParts(RefToParts(attribute.Value));
                }
                else
                {
                    AppendParts(Enumerate(new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, attribute.Value)));
                }
            }

            private void AppendParts<T>(T parts) where T : IEnumerable<SymbolDisplayPart>
            {
                if (currentDescription != SymbolDescriptionKind.None)
                {
                    if (_lineWasBroken)
                    {
                        _lineWasBroken = false;
                        // NOTE: only one line break doesn't append a new line to quick info
                        AppendParts(Enumerate(_lineBreak));
                        AppendParts(Enumerate(_lineBreak));
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

                return Enumerate(new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, TrimRefPrefix(refValue)));
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
                            if (_provider._description.TryGetValue(currentDescription, out var parts) && parts.Count != 0 ||
                                builder.Length > 0)
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

            private static IEnumerable<SymbolDisplayPart> Enumerate(SymbolDisplayPart part)
            {
                yield return part;
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