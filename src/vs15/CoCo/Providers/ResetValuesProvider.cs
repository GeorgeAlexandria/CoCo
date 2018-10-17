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

        public Color Foreground => FormattingService.GetDefaultFormatting().ForegroundBrush.GetColor();

        public Color Background => FormattingService.GetDefaultFormatting().BackgroundBrush.GetColor();

        public int FontRenderingSize => (int)FormattingService.GetDefaultFormatting().FontRenderingEmSize;
    }
}