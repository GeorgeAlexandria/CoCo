using System.IO;

namespace CoCoTests
{
    internal static class TextHelper
    {
        public static TextBuffer CreateTextBuffer(this string contentType, string path)
        {
            path = TestHelper.GetPathRelativeToTest(path);
            var code = File.ReadAllText(path);
            var buffer = new TextBuffer(new ContentType(contentType), new StringOperand(code));
            return buffer;
        }
    }
}