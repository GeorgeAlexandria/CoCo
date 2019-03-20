using Microsoft.VisualStudio.Text.Adornments;

namespace CoCo.QuickInfo
{
    internal sealed class QuickInfoWrapper
    {
        public QuickInfoWrapper(ContainerElement element)
        {
            Element = element;
        }

        public ContainerElement Element { get; }
    }
}