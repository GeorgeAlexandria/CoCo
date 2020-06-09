using FSharp.Compiler;
using Microsoft.VisualStudio.Text;

namespace CoCo.Analyser
{
    internal static class FSharpCompilerExtensions
    {
        public static Range.range ToRange(this SnapshotSpan span)
        {
            Range.pos ToPosition(SnapshotPoint point)
            {
                var snapshotLine = span.Snapshot.GetLineFromPosition(point.Position);
                var column = point.Position - snapshotLine.Start.Position;
                return Range.mkPos(snapshotLine.LineNumber, column);
            }

            var start = ToPosition(span.Start);
            var end = ToPosition(span.End);
            return Range.mkFileIndexRange(1, start, end);
        }

        public static Span ToSpan(this Range.range range, ITextSnapshot textSnapshot)
        {
            var startLine = textSnapshot.GetLineFromLineNumber(range.StartLine - 1);
            var startPosition = startLine.Start.Position + range.StartColumn;

            var endLine = textSnapshot.GetLineFromLineNumber(range.EndLine - 1);
            var endPosition = endLine.Start.Position + range.EndColumn;

            return new Span(startPosition, endPosition - startPosition);
        }
    }
}