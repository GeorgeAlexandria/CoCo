using System.Collections.Generic;

namespace CoCo.UI.Models
{
    public class PresetModel
    {
        public PresetModel(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public ICollection<ClassificationModel> Classifications { get; } = new List<ClassificationModel>();
    }
}