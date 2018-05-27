using System.Collections.Generic;

namespace CoCo.UI.Models
{

    public interface ILanguageModel
    {
        string Name { get; }

        ICollection<IPresetModel> Presets { get; }

        ICollection<IClassificationModel> Classifications { get; }
    }
}