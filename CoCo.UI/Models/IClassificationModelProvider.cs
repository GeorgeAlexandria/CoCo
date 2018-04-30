using System.Collections.Generic;

namespace CoCo.UI.Models
{
    public interface IClassificationModelProvider
    {
        IEnumerable<IClassificationModel> Get(string language);
    }
}