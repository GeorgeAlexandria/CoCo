using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace CoCo.Test.Common
{
    internal class TextSnapshot : ITextSnapshot
    {
        private readonly TextVersion _version;
        private readonly StringOperand _source;

        public TextSnapshot(ITextBuffer buffer, TextVersion version, StringOperand source)
        {
            _version = version;
            _source = source;
            TextBuffer = buffer;
            ContentType = version.TextBuffer.ContentType;
        }

        public IContentType ContentType { get; }

        public ITextBuffer TextBuffer { get; }

        public ITextVersion Version => _version;

        public int Length => _source.Length;

        public int LineCount => _source.LineBreakCount;

        public char this[int position] => _source[position];

        public IEnumerable<ITextSnapshotLine> Lines
        {
            get
            {
                int lineCount = _source.LineBreakCount;
                for (int line = 0; line < lineCount; ++line)
                {
                    yield return GetLineFromLineNumber(line);
                }
            }
        }

        public string GetText(int startIndex, int length) => GetText(new Span(startIndex, length));

        public string GetText() => GetText(new Span(0, Length));

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode) =>
            _version.CreateTrackingPoint(position, trackingMode);

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode, TrackingFidelityMode trackingFidelity) =>
            _version.CreateTrackingPoint(position, trackingMode, trackingFidelity);

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode) =>
            _version.CreateTrackingSpan(start, length, trackingMode);

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity) =>
            _version.CreateTrackingSpan(start, length, trackingMode, trackingFidelity);

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode) =>
            _version.CreateTrackingSpan(span, trackingMode, TrackingFidelityMode.Forward);

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity) =>
            _version.CreateTrackingSpan(span, trackingMode, trackingFidelity);

        public string GetText(Span span) => _source.GetText(span);

        public ITextSnapshotLine GetLineFromLineNumber(int lineNumber)
        {
            _source.GetLineFromLineNumber(lineNumber, out var span, out var lineBreakLength);
            var textImageLine = new TextImageLine(lineNumber, span, lineBreakLength);
            return new TextSnapshotLine(this, textImageLine);
        }

        public ITextSnapshotLine GetLineFromPosition(int position) => GetLineFromLineNumber(GetLineNumberFromPosition(position));

        public int GetLineNumberFromPosition(int position) => _source.GetLineNumberFromPosition(position);

        public void SaveToFile(string filePath, bool replaceFile, Encoding encoding) => throw new NotImplementedException();

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) => throw new NotImplementedException();

        public char[] ToCharArray(int startIndex, int length) => throw new NotImplementedException();

        public void Write(TextWriter writer) => throw new NotImplementedException();

        public void Write(TextWriter writer, Span span) => throw new NotImplementedException();
    }
}