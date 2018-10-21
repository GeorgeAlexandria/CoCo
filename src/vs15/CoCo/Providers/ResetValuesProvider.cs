using System.Windows.Media;
using CoCo.Services;
using CoCo.UI.ViewModels;

namespace CoCo.Providers
{
    internal sealed class ResetValuesProvider : IResetValuesProvider
    {
        private ResetValuesProvider()
        {
        }

        private static ResetValuesProvider _instance;

        public static ResetValuesProvider Instance => _instance ?? (_instance = new ResetValuesProvider());

        public Color GetForeground(string name) => FormattingService.GetDefaultFormatting(name).ForegroundBrush.GetColor();

        public Color GetBackground(string name) => FormattingService.GetDefaultFormatting(name).BackgroundBrush.GetColor();

        public int GetFontRenderingSize(string name) => (int)FormattingService.GetDefaultFormatting(name).FontRenderingEmSize;
    }
}