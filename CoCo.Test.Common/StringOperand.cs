using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Text;

namespace CoCo.Test.Common
{
    internal class StringOperand
    {
        private static readonly StringOperand _empty = new StringOperand(string.Empty);

        private readonly int _textSpanStart;

        private readonly int _lineBreakSpanStart;

        private readonly string _source;

        private readonly LineSeparators _lineBreaks;

        public StringOperand(string source)
        {
            _lineBreaks = LineSeparators.CreateLineBreaks(source);
            _source = source;
            _textSpanStart = 0;
            _lineBreakSpanStart = 0;
            LineBreakCount = _lineBreaks.Count;
            Length = _source.Length;
        }

        private StringOperand(string source, LineSeparators lineBreaks, int start, int length, int lineBreakSpanStart, int lineBreakCount)
        {
            _source = source;
            _lineBreaks = lineBreaks;
            _textSpanStart = start;
            _lineBreakSpanStart = lineBreakSpanStart;
            Length = length;
            LineBreakCount = lineBreakCount;
        }

        public int Length { get; }

        public int LineBreakCount { get; }

        public char this[int index]
        {
            get
            {
                if (index < 0 || index >= Length) throw new ArgumentOutOfRangeException(nameof(index));
                return _source[_textSpanStart + index];
            }
        }

        public int GetLineNumberFromPosition(int position)
        {
            if (position < 0 || position > Length) throw new ArgumentOutOfRangeException(nameof(position));
            if (position == Length) return LineBreakCount;

            position += _textSpanStart;

            int firstLineByPosition = _lineBreakSpanStart;
            int lastLineByPosition = LineBreakSpanEnd;
            while (firstLineByPosition < lastLineByPosition)
            {
                int temp = (firstLineByPosition + lastLineByPosition) / 2;
                if (position < _lineBreaks.EndOfLineBreak(temp))
                {
                    lastLineByPosition = temp;
                    continue;
                }
                firstLineByPosition = temp;
            }
            return firstLineByPosition - _lineBreakSpanStart;
        }

        public void GetLineFromLineNumber(int lineNumber, out Span extent, out int lineBreakLength)
        {
            if (lineNumber < 0 || lineNumber > LineBreakCount) throw new ArgumentOutOfRangeException(nameof(lineNumber));

            int spanFromLine = _lineBreakSpanStart + lineNumber;
            int startLine = (lineNumber == 0) ? 0 : (_lineBreaks.EndOfLineBreak(spanFromLine - 1) - _textSpanStart);
            if (lineNumber == LineBreakCount)
            {
                lineBreakLength = 0;
                extent = Span.FromBounds(startLine, Length);
                return;
            }

            int startLineBreakPos = _lineBreaks.StartOfLineBreak(spanFromLine);
            lineBreakLength = _lineBreaks.EndOfLineBreak(spanFromLine) - startLineBreakPos;
            Debug.Assert(lineBreakLength < 3);

            int endLine = startLineBreakPos - _textSpanStart;
            extent = Span.FromBounds(startLine, endLine);
        }

        public string GetText(Span span) => _source.Substring(_textSpanStart + span.Start, span.Length);

        public StringOperand GetSubText(Span span)
        {
            if (span.End > Length) throw new ArgumentOutOfRangeException(nameof(span));
            if (span.Length == 0) return _empty;
            if (span.Length == Length) return this;

            FindFirstAndLastLines(span, out int firstLine, out int lastLine);

            int start = _textSpanStart + span.Start;
            int end = start + span.Length - 1;

            return lastLine - firstLine == 0
                ? new StringOperand(_source, _lineBreaks, start, span.Length, 0, 0)
                : new StringOperand(_source, _lineBreaks, start, span.Length, firstLine, lastLine - firstLine);
        }

        private int TextSpanEnd => _textSpanStart + Length;

        private int LineBreakSpanEnd => _lineBreakSpanStart + LineBreakCount;

        private void FindFirstAndLastLines(Span span, out int firstLineNumber, out int lastLineNumber)
        {
            firstLineNumber = _lineBreakSpanStart + GetLineNumberFromPosition(span.Start);
            lastLineNumber = _lineBreakSpanStart + GetLineNumberFromPosition(span.End);
        }
    }
}