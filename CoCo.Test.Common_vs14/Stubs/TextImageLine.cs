using Microsoft.VisualStudio.Text;

namespace CoCo.Test.Common
{
    internal struct TextImageLine
    {
        public TextImageLine(int lineNumber, Span extent, int lineBreakLength)
        {
            Extent = extent;
            LineNumber = lineNumber;
            LineBreakLength = lineBreakLength;
        }

        public Span Extent { get; }

        public int LineNumber { get; }

        public int LineBreakLength { get; }
    }
}