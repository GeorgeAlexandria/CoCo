using System.Collections.Generic;

namespace CoCo.UI.Models
{
    public class LanguageModel
    {
        public LanguageModel(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public ICollection<PresetModel> Presets { get; } = new List<PresetModel>();

        public ICollection<ClassificationModel> Classifications { get; } = new List<ClassificationModel>();
    }
}