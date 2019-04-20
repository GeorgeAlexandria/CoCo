using System.Windows.Media;

namespace CoCo.UI.ViewModels
{
    /// <summary>
    /// Provides a couple of values when request on reset was received
    /// </summary>
    /// <remarks>
    /// It's used just to show on the UI the non empty values when request was received.
    /// </remarks>
    public interface IResetValuesProvider
    {
        Color GetForeground(string name);

        Color GetBackground(string name);

        int GetFontRenderingSize(string name);
    }
}