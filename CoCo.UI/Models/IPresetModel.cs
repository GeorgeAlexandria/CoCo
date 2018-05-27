using System.Collections.Generic;

namespace CoCo.UI.Models
{
    public interface IPresetModel
    {
        string Name { get; }

        ICollection<IClassificationModel> Classifications { get; }
    }
}