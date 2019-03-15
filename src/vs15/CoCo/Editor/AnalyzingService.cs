using System.Collections.Generic;
using CoCo.Analyser;
using CoCo.UI.Data;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Editor
{
    public sealed class AnalyzingService : IAnalyzingService
    {
        public static readonly AnalyzingService Instance = new AnalyzingService();

        private AnalyzingService()
        {
        }

        public event ClassificationChangedEventHandler ClassificationChanged;

        public static void SetAnalyzingOptions(EditorOption option)
        {
            var classificationTypes = new Dictionary<string, IClassificationType>(Names.All.Count);
            foreach (var languageClassifications in ClassificationManager.GetClassifications().Values)
            {
                foreach (var classification in languageClassifications)
                {
                    classificationTypes.Add(classification.Classification, classification);
                }
            }

            var classifications = new Dictionary<IClassificationType, ClassificationOption>(classificationTypes.Count);
            foreach (var language in option.Languages)
            {
                foreach (var classification in language.Classifications)
                {
                    if (classificationTypes.TryGetValue(classification.Name, out var type))
                    {
                        classifications.Add(type, new ClassificationOption(classification.IsDisabled, classification.IsDisabledInXml));
                    }
                }
            }
            Instance.ClassificationChanged?.Invoke(new ClassificationsChangedEventArgs(classifications));
        }
    }
}