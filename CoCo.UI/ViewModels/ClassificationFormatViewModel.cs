using System.Collections.Generic;

namespace CoCo.UI.ViewModels
{
    public static class ClassificationFormatProvider
    {
        public static IEnumerable<ClassificationFormatViewModel> Get(string language)
        {
            return new[] { new ClassificationFormatViewModel(), new ClassificationFormatViewModel() };
        }
    }

    public class ClassificationFormatViewModel : BaseViewModel
    {
        public bool IsCheked { get; set; } = true;

        public string Name { get; set; } = "Sample";
    }
}