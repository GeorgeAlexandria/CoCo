using System.Collections.Generic;
using CoCo.Analyser;
using CoCo.Analyser.CSharp;
using CoCo.Analyser.VisualBasic;
using CoCo.Providers;
using CoCo.Utils;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;

namespace CoCo
{
    public sealed class ClassificationManager
    {
        private static readonly IReadOnlyDictionary<string, string> _nonIdentifierClassifications = new Dictionary<string, string>
        {
            [CSharpNames.ClassName] = "class name",
            [CSharpNames.StructureName] = "struct name",
            [CSharpNames.InterfaceName] = "interface name",
            [CSharpNames.EnumName] = "enum name",
            [CSharpNames.DelegateName] = "delegate name",
            [CSharpNames.TypeParameterName] = "type parameter name",

            [VisualBasicNames.ClassName] = "class name",
            [VisualBasicNames.StructureName] = "struct name",
            [VisualBasicNames.InterfaceName] = "interface name",
            [VisualBasicNames.EnumName] = "enum name",
            [VisualBasicNames.DelegateName] = "delegate name",
            [VisualBasicNames.TypeParameterName] = "type parameter name",
            [VisualBasicNames.ModuleName] = "module name",
        };

        private static Dictionary<string, ICollection<IClassificationType>> _classifications;

        private ClassificationManager()
        {
        }

        public static ClassificationManager Instance { get; } = new ClassificationManager();

        public IClassificationType DefaultIdentifierClassification =>
            ServicesProvider.Instance.RegistryService.GetClassificationType(PredefinedClassificationTypeNames.Identifier);

        /// <summary>
        /// Try to retreive a default classification for <paramref name="name"/> if it isn't <see cref="PredefinedClassificationTypeNames.Identifier"/>
        /// </summary>
        public bool TryGetDefaultNonIdentifierClassification(string name, out IClassificationType classification)
        {
            if (_nonIdentifierClassifications.TryGetValue(name, out var classificationName))
            {
                classification = ServicesProvider.Instance.RegistryService.GetClassificationType(classificationName);
                return !(classification is null);
            }

            classification = null;
            return false;
        }

        /// <returns>
        /// Classifications are grouped by language
        /// </returns>
        public IReadOnlyDictionary<string, ICollection<IClassificationType>> GetClassifications()
        {
            if (_classifications != null) return _classifications;

            _classifications = new Dictionary<string, ICollection<IClassificationType>>();

            var registryService = ServicesProvider.Instance.RegistryService;
            var formatMapService = ServicesProvider.Instance.FormatMapService;

            var formatMap = formatMapService.GetClassificationFormatMap(category: "text");
            var identifierPosition = GetIdentifierPosition(registryService, formatMap);

            foreach (var (language, names) in Names.All)
            {
                var languageClassifications = new List<IClassificationType>();
                foreach (var name in names)
                {
                    var classificationType = registryService.GetClassificationType(name);
                    if (classificationType != null)
                    {
                        // TODO: need to carefully test this case
                        if (identifierPosition > 0)
                        {
                            // NOTE: Set priority of classification next to identifier
                            SetPriority(formatMap, classificationType, identifierPosition);
                        }
                    }
                    else
                    {
                        classificationType = registryService.CreateClassificationType(name, new IClassificationType[0]);
                        var formatting = TextFormattingRunProperties.CreateTextFormattingRunProperties();
                        if (identifierPosition > 0)
                        {
                            // NOTE: Set priority of classification next to identifier
                            var afterIdentifierClassification = formatMap.CurrentPriorityOrder[identifierPosition + 1];
                            formatMap.AddExplicitTextProperties(classificationType, formatting, afterIdentifierClassification);
                        }
                        else
                        {
                            // NOTE: Set the last priority
                            formatMap.AddExplicitTextProperties(classificationType, formatting);
                        }
                    }

                    languageClassifications.Add(classificationType);
                }
                _classifications.Add(language, languageClassifications);
            }

            return _classifications;
        }

        /// <returns>
        /// if <see cref="PredefinedClassificationTypeNames.Identifier"/> classification doesn't exist or
        /// it is the last element returns -1, otherwise returns it position
        /// </returns>
        private static int GetIdentifierPosition(IClassificationTypeRegistryService registryService, IClassificationFormatMap formatMap)
        {
            var identifierClassification = registryService.GetClassificationType(PredefinedClassificationTypeNames.Identifier);
            if (identifierClassification != null)
            {
                var identifierPosition = formatMap.CurrentPriorityOrder.IndexOf(identifierClassification);
                if (identifierPosition >= 0 && identifierPosition < formatMap.CurrentPriorityOrder.Count - 1) return identifierPosition;
            }
            return -1;
        }

        /// <summary>
        /// Swap priority of items from <paramref name="classificationType"/> to identifier classification: <para/>
        /// * [...,cur,a1,a2,id,...] -> [...,a1,a2,id,cur,...]<para/>
        /// * [...,id,a1,a2,cur,...] -> [...,id,cur,a1,a2,...]
        /// </summary>
        private static void SetPriority(
            IClassificationFormatMap formatMap,
            IClassificationType classificationType,
            int identifierPosition)
        {
            var index = formatMap.CurrentPriorityOrder.IndexOf(classificationType);
            if (index < identifierPosition)
            {
                while (index < identifierPosition)
                {
                    formatMap.SwapPriorities(classificationType, formatMap.CurrentPriorityOrder[++index]);
                }
            }
            else
            {
                while (identifierPosition < --index)
                {
                    formatMap.SwapPriorities(formatMap.CurrentPriorityOrder[index], classificationType);
                }
            }
        }
    }
}