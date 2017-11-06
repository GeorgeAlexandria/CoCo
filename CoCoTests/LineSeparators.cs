using System;
using System.Collections.Generic;

namespace CoCoTests
{
    internal class LineSeparators : ILineSeparators
    {
        // The little 31 bits determine the start position of a line' break.
        // The 32 bit determines how many characters are represented in the line' break.
        // If the 32 bit is set it means that line' break contains 2 character otherwise it contains only 1.
        private readonly List<uint> _lineBreaks = new List<uint>();

        private const uint lengthOfLineBreakMask = (uint)int.MaxValue + 1;

        private const uint positionOfLineBreakMask = int.MaxValue;

        private LineSeparators()
        {
        }

        public int Length => _lineBreaks.Count;

        public int StartOfLineBreak(int index) => (int)(_lineBreaks[index] & positionOfLineBreakMask);

        public int EndOfLineBreak(int index) => StartOfLineBreak(index) + LengthOfLineBreak(index);

        public static LineSeparators CreateLineBreaks(string source)
        {
            LineSeparators lineBreaks = new LineSeparators();
            int i = 0;
            while (i < source.Length)
            {
                int num = LengthOfLineBreak(source, i);
                if (num == 0)
                {
                    i++;
                    continue;
                }
                lineBreaks.Add(i, num);
                i += num;
            }
            return lineBreaks;
        }

        private int LengthOfLineBreak(int index) => (_lineBreaks[index] & lengthOfLineBreakMask) == lengthOfLineBreakMask ? 2 : 1;

        private void Add(int start, int length)
        {
            if (start < 0 || start > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(start));
            if (length < 1 || length > 2) throw new ArgumentOutOfRangeException(nameof(length));

            _lineBreaks.Add((uint)start | (length == 2 ? lengthOfLineBreakMask : 0));
        }

        private static int LengthOfLineBreak(string str, int start)
        {
            char curChar = str[start];

            //TODO: Did I handle all of cases?
            return curChar == '\r'
                ? ++start >= str.Length || str[start] != '\n' ? 1 : 2
                : curChar == '\n' ? 1 : 0;
        }
    }
}