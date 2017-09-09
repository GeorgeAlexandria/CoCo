using Microsoft.VisualStudio.Text;

namespace CoCoTests
{
    internal class TextSnapshotLine : ITextSnapshotLine
    {
        public ITextSnapshot Snapshot => Extent.Snapshot;

        public int LineNumber { get; }

        public SnapshotPoint Start => Extent.Start;

        public int Length => Extent.Length;

        public int LengthIncludingLineBreak => Extent.Length + LineBreakLength;

        public int LineBreakLength { get; }

        public SnapshotPoint End => Extent.End;

        public SnapshotPoint EndIncludingLineBreak
        {
            get
            {
                SnapshotSpan extent = Extent;
                int position = extent.Span.End + LineBreakLength;
                return new SnapshotPoint(extent.Snapshot, position);
            }
        }

        public SnapshotSpan Extent { get; }

        public SnapshotSpan ExtentIncludingLineBreak => new SnapshotSpan(Extent.Start, LengthIncludingLineBreak);

        public TextSnapshotLine(ITextSnapshot snapshot, int lineNumber, Span extent, int lineBreakLength)
        {
            Extent = new SnapshotSpan(snapshot, extent);
            LineNumber = lineNumber;
            LineBreakLength = lineBreakLength;
        }

        public TextSnapshotLine(ITextSnapshot snapshot, TextImageLine lineSpan) :
            this(snapshot, lineSpan.LineNumber, lineSpan.Extent, lineSpan.LineBreakLength)
        {
        }

        public string GetText() => Extent.GetText();

        public string GetTextIncludingLineBreak() => ExtentIncludingLineBreak.GetText();

        public string GetLineBreakText()
        {
            SnapshotSpan extent = Extent;
            Span span = new Span(extent.Span.End, LineBreakLength);
            return extent.Snapshot.GetText(span);
        }
    }
}
