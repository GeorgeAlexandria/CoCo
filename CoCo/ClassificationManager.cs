using System.Collections.Generic;
using System.ComponentModel.Composition;
using CoCo.Analyser;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;

namespace CoCo
{
    public sealed class ClassificationManager
    {
        private static Dictionary<string, List<IClassificationType>> _classifications;

        [Import]
        private IClassificationTypeRegistryService _registryService;

        [Import]
        private IClassificationFormatMapService _formatMapService;

        private ClassificationManager()
        {
        }

        private static ClassificationManager _instance;

        public static ClassificationManager Instance
        {
            get
            {
                if (_instance != null) return _instance;

                if (ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) is IComponentModel componentModel)
                {
                    _instance = new ClassificationManager();
                    componentModel.DefaultCompositionService.SatisfyImportsOnce(_instance);
                    return _instance;
                }

                // TODO: throw some kind of exception
                return null;
            }
        }

        public IClassificationFormatMapService FormatMapService => _formatMapService;

        public IClassificationTypeRegistryService RegistryService => _registryService;

        public IClassificationType DefaultClassification =>
            RegistryService.GetClassificationType(PredefinedClassificationTypeNames.Identifier);

        /// <returns>
        /// Classifications are grouped by language
        /// </returns>
        public Dictionary<string, List<IClassificationType>> GetClassifications()
        {
            if (_classifications != null) return _classifications;

            _classifications = new Dictionary<string, List<IClassificationType>>();

            var formatMap = _formatMapService.GetClassificationFormatMap(category: "text");
            var identifierPosition = GetIdentifierPosition(_registryService, formatMap);

            var languageClassifications = new List<IClassificationType>();
            foreach (var name in Names.All)
            {
                var classificationType = _registryService.GetClassificationType(name);
                if (classificationType != null)
                {
                    if (identifierPosition > 0)
                    {
                        SetPriority(formatMap, classificationType, identifierPosition);
                    }
                }
                else
                {
                    classificationType = _registryService.CreateClassificationType(name, new IClassificationType[0]);
                    var formatting = TextFormattingRunProperties.CreateTextFormattingRunProperties();
                    if (identifierPosition > 0)
                    {
                        var afterIdentifierClassification = formatMap.CurrentPriorityOrder[identifierPosition + 1];
                        formatMap.AddExplicitTextProperties(classificationType, formatting, afterIdentifierClassification);
                    }
                    else
                    {
                        formatMap.AddExplicitTextProperties(classificationType, formatting);
                    }
                }

                languageClassifications.Add(classificationType);
            }

            _classifications.Add("CSharp", languageClassifications);
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