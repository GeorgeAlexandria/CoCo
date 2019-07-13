using System;
using Microsoft.CodeAnalysis.Text;
using FSharpISourceText = FSharp.Compiler.Text.ISourceText;

namespace CoCo.Analyser.Classifications.FSharp
{
    internal sealed class SourceTextWrapper : FSharpISourceText
    {
        private readonly SourceText sourceText;

        public SourceTextWrapper(SourceText sourceText)
        {
            this.sourceText = sourceText;
        }

        public char this[int value] => sourceText[value];

        public int Length => sourceText.Length;

        public bool ContentEquals(FSharpISourceText sourceText) =>
            sourceText is SourceTextWrapper wrapper && wrapper.sourceText.ContentEquals(this.sourceText);

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) =>
            sourceText.CopyTo(sourceIndex, destination, destinationIndex, count);

        public Tuple<int, int> GetLastCharacterPosition() => (sourceText.Lines.Count > 0
            ? (sourceText.Lines.Count, sourceText.Lines[sourceText.Lines.Count - 1].Span.Length)
            : (0, 0)).ToTuple();

        public int GetLineCount() => sourceText.Lines.Count;

        public string GetLineString(int lineIndex) => sourceText.Lines[lineIndex].ToString();

        public string GetSubTextString(int start, int length) =>
            sourceText.GetSubText(new TextSpan(start, length)).ToString();

        public bool SubTextEquals(string target, int startIndex)
        {
            if (startIndex < 0 || startIndex >= sourceText.Length ||
                string.IsNullOrEmpty(target) ||
                target.Length + startIndex > sourceText.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < target.Length; ++i)
            {
                if (sourceText[startIndex + i] != target[i]) return false;
            }
            return true;
        }
    }
}