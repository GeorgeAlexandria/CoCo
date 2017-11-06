using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace CoCoTests
{
    internal class TextSnapshot : ITextSnapshot
    {
        private readonly ITextVersion2 _version;

        public IContentType ContentType { get; }

        public ITextBuffer TextBuffer { get; }

        public ITextVersion Version => _version;

        public int Length => TextImage.Length;

        public int LineCount => TextImage.LineCount;

        public char this[int position] => TextImage[position];

        public IEnumerable<ITextSnapshotLine> Lines
        {
            get
            {
                int lineCount = TextImage.LineCount;
                for (int line = 0; line < lineCount; ++line)
                {
                    yield return GetLineFromLineNumber(line);
                }
            }
        }

        public ITextImage TextImage { get; }

        public TextSnapshot(ITextBuffer buffer, ITextVersion2 version, StringOperand source)
        {
            _version = version;
            TextBuffer = buffer;
            TextImage = new TextImage(version.ImageVersion, source);
            ContentType = version.TextBuffer.ContentType;
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

        public void SaveToFile(string filePath, bool replaceFile, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public string GetText(Span span) => TextImage.GetText(span);

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) =>
            TextImage.CopyTo(sourceIndex, destination, destinationIndex, count);

        public char[] ToCharArray(int startIndex, int length) => TextImage.ToCharArray(startIndex, length);

        public ITextSnapshotLine GetLineFromLineNumber(int lineNumber) => new TextSnapshotLine(this, TextImage.GetLineFromLineNumber(lineNumber));

        public ITextSnapshotLine GetLineFromPosition(int position) => GetLineFromLineNumber(TextImage.GetLineNumberFromPosition(position));

        public int GetLineNumberFromPosition(int position) => TextImage.GetLineNumberFromPosition(position);

        public void Write(TextWriter writer)
        {
            TextImage.Write(writer, new Span(0, TextImage.Length));
        }

        public void Write(TextWriter writer, Span span)
        {
            TextImage.Write(writer, span);
        }
    }
}