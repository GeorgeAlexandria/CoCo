using System.Collections.Generic;
using CoCo.Analyser;
using CoCo.UI.Data;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Services
{
    public sealed class AnalyzingService : IAnalyzingService
    {
        public static readonly AnalyzingService Instance = new AnalyzingService();

        private AnalyzingService()
        {
        }

        public event ClassificationChangedEventHandler ClassificationChanged;

        public static void SetAnalyzingOptions(Option option)
        {
            var classificationTypes = new Dictionary<string, IClassificationType>(Names.All.Count);
            foreach (var languageClassifications in ClassificationManager.Instance.GetClassifications().Values)
            {
                foreach (var classification in languageClassifications)
                {
                    classificationTypes.Add(classification.Classification, classification);
                }
            }

            var classifications = new Dictionary<IClassificationType, ClassificationInfo>(classificationTypes.Count);
            foreach (var language in option.Languages)
            {
                foreach (var classification in language.Classifications)
                {
                    if (classificationTypes.TryGetValue(classification.Name, out var type))
                    {
                        classifications.Add(type, new ClassificationInfo(type, classification.IsClassified, classification.ClassifyInXml));
                    }
                }
            }
            Instance.ClassificationChanged?.Invoke(new ClassificationsChangedEventArgs(classifications));
        }
    }
}