using System.Collections.Generic;
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
                public const string ParameterRefElement = "paramref";
                public const string SeeElement = "see";
                public const string SeeAlsoElement = "seealso";
                public const string SummaryElement = "summary";
                public const string TypeParameterRefElement = "typeparamref";
            }

            private readonly SymbolDescriptionProvider _provider;

            private SymbolDescriptionKind currentDescription;

            private Comment(SymbolDescriptionProvider provider)
            {
                _provider = provider;
            }

            public static void Parse(SymbolDescriptionProvider provider, string xml)
            {
                var rawXml = "<i>" + xml + "</i>";
                var comment = new Comment(provider);

                // TODO: catch?
                comment.Parse(XDocument.Parse(rawXml));
            }

            private void Parse(XNode node)
            {
                if (node is XText text)
                {
                    AppendParts(Enumerate(new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, text.Value)));
                    return;
                }

                var element = (node as XDocument)?.Root ?? (XElement)node;

                var name = element.Name;
                if (name == XmlNames.SummaryElement)
                {
                    var oldDescription = currentDescription;
                    currentDescription = SymbolDescriptionKind.Additional;

                    foreach (var childNode in element.Nodes())
                    {
                        Parse(childNode);
                    }

                    currentDescription = oldDescription;
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
                // NOTE: if attribute is expected => get parts from it, else just add it as one text part
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

            private static IEnumerable<SymbolDisplayPart> Enumerate(SymbolDisplayPart part)
            {
                yield return part;
            }

            /// <summary>
            /// Removes ref prefix likes "M:" in "M:Namespace..."
            /// </summary>
            private static string TrimRefPrefix(string value) =>
                value.Length >= 2 && value[1] == ':'
                    ? value.Substring(2)
                    : value;
        }
    }
}