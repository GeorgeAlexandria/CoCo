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

        public static IClassificationType DefaultIdentifierClassification =>
            ServicesProvider.Instance.RegistryService.GetClassificationType(PredefinedClassificationTypeNames.Identifier);

        /// <summary>
        /// Try to retreive a default classification for <paramref name="name"/> if it isn't <see cref="PredefinedClassificationTypeNames.Identifier"/>
        /// </summary>
        public static bool TryGetDefaultNonIdentifierClassification(string name, out IClassificationType classification)
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
        public static IReadOnlyDictionary<string, ICollection<IClassificationType>> GetClassifications()
        {
            if (_classifications != null) return _classifications;

            _classifications = new Dictionary<string, ICollection<IClassificationType>>();

            var registryService = ServicesProvider.Instance.RegistryService;
            var formatMapService = ServicesProvider.Instance.FormatMapService;

            var formatMap = formatMapService.GetClassificationFormatMap(category: "text");
            var identifierPosition = GetClassificationPosition(registryService, formatMap, PredefinedClassificationTypeNames.Identifier);

            formatMap.BeginBatchUpdate();
            foreach (var (language, names) in Names.All)
            {
                var languageClassifications = new List<IClassificationType>();
                foreach (var name in names)
                {
                    var classificationPosition = identifierPosition;
                    if (_nonIdentifierClassifications.TryGetValue(name, out var relevantClassification))
                    {
                        classificationPosition = GetClassificationPosition(registryService, formatMap, relevantClassification);
                    }

                    var classificationType = registryService.GetClassificationType(name);
                    if (classificationType != null)
                    {
                        // TODO: need to carefully test this case
                        if (classificationPosition > 0)
                        {
                            // NOTE: Set priority of classification next to the relevant classification
                            SetPriority(formatMap, classificationType, classificationPosition);
                        }
                    }
                    else
                    {
                        classificationType = registryService.CreateClassificationType(name, new IClassificationType[0]);
                        var formatting = TextFormattingRunProperties.CreateTextFormattingRunProperties();
                        if (classificationPosition > 0)
                        {
                            // NOTE: Set priority of classification next to the relevant classification
                            var afterClassification = formatMap.CurrentPriorityOrder[classificationPosition + 1];
                            formatMap.AddExplicitTextProperties(classificationType, formatting, afterClassification);
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
            formatMap.EndBatchUpdate();

            return _classifications;
        }

        /// <returns>
        /// if <paramref name="classificationName"/> classification doesn't exist or
        /// it is the last element returns -1, otherwise returns it position
        /// </returns>
        private static int GetClassificationPosition(
            IClassificationTypeRegistryService registryService, IClassificationFormatMap formatMap, string classificationName)
        {
            var classification = registryService.GetClassificationType(classificationName);
            if (classification != null)
            {
                var classificationPosition = formatMap.CurrentPriorityOrder.IndexOf(classification);
                if (classificationPosition >= 0 && classificationPosition < formatMap.CurrentPriorityOrder.Count - 1)
                {
                    return classificationPosition;
                }
            }
            return -1;
        }

        /// <summary>
        /// Swap priority of items from <paramref name="classificationType"/> to classification: <para/>
        /// * [...,cur,a1,a2,tar,...] -> [...,a1,a2,tar,cur,...]<para/>
        /// * [...,tar,a1,a2,cur,...] -> [...,tar,cur,a1,a2,...]
        /// </summary>
        private static void SetPriority(
            IClassificationFormatMap formatMap,
            IClassificationType classificationType,
            int classificationPosition)
        {
            var index = formatMap.CurrentPriorityOrder.IndexOf(classificationType);
            if (index < classificationPosition)
            {
                while (index < classificationPosition)
                {
                    formatMap.SwapPriorities(classificationType, formatMap.CurrentPriorityOrder[++index]);
                }
            }
            else
            {
                while (classificationPosition < --index)
                {
                    formatMap.SwapPriorities(formatMap.CurrentPriorityOrder[index], classificationType);
                }
            }
        }
    }
}