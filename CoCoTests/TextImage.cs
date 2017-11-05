﻿using System;
using System.IO;
using Microsoft.VisualStudio.Text;

namespace CoCoTests
{
    internal class StringOperand
    {
        public StringOperand()
        {
        }

        public int GetLineNumberFromPosition(int position) => throw new NotImplementedException();

        public void GetLineFromLineNumber(int lineNumber, out Span extent, out int lineBreakLength) => throw new NotImplementedException();

        public int Length;

        public int LineBreakCount;

        public string GetText(Span span) => throw new NotImplementedException();

        public StringOperand GetSubText(Span span) => throw new NotImplementedException();
    }

    internal class TextImage : ITextImage
    {
        private readonly StringOperand _sourceOperand;

        public TextImage(ITextImageVersion version, StringOperand sourceOperand)
        {
            Version = version;
            _sourceOperand = sourceOperand;
        }

        public char this[int position] => throw new NotImplementedException();

        public ITextImageVersion Version { get; }

        public int Length => _sourceOperand.Length;

        public int LineCount => _sourceOperand.LineBreakCount;

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) => throw new NotImplementedException();

        public TextImageLine GetLineFromLineNumber(int lineNumber)
        {
            _sourceOperand.GetLineFromLineNumber(lineNumber, out var span, out var lineBreakLength);
            return new TextImageLine(this, lineNumber, span, lineBreakLength);
        }

        public TextImageLine GetLineFromPosition(int position) => GetLineFromLineNumber(_sourceOperand.GetLineNumberFromPosition(position));

        public int GetLineNumberFromPosition(int position) => _sourceOperand.GetLineNumberFromPosition(position);

        public ITextImage GetSubText(Span span) => new TextImage(Version, _sourceOperand.GetSubText(span));

        public string GetText(Span span) => _sourceOperand.GetText(span);

        public char[] ToCharArray(int startIndex, int length) => throw new NotImplementedException();

        public void Write(TextWriter writer, Span span) => throw new NotImplementedException();
    }
}