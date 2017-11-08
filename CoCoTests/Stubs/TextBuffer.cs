using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace CoCoTests
{
    internal class TextBuffer : ITextBuffer
    {
        public TextBuffer(IContentType contentType, StringOperand source)
        {
            ContentType = contentType;
            CurrentSnapshot = new TextSnapshot(this, new TextVersion(this, new TextImageVersion(source.Length)), source);
        }

        public IContentType ContentType { get; }

        public ITextSnapshot CurrentSnapshot { get; }

        public bool EditInProgress => throw new NotImplementedException();

        public PropertyCollection Properties { get; } = new PropertyCollection();

        public event EventHandler<SnapshotSpanEventArgs> ReadOnlyRegionsChanged;

        public event EventHandler<TextContentChangedEventArgs> Changed;

        public event EventHandler<TextContentChangedEventArgs> ChangedLowPriority;

        public event EventHandler<TextContentChangedEventArgs> ChangedHighPriority;

        public event EventHandler<TextContentChangingEventArgs> Changing;

        public event EventHandler PostChanged;

        public event EventHandler<ContentTypeChangedEventArgs> ContentTypeChanged;

        public void ChangeContentType(IContentType newContentType, object editTag)
        {
            throw new NotImplementedException();
        }

        public bool CheckEditAccess()
        {
            throw new NotImplementedException();
        }

        public ITextEdit CreateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag)
        {
            throw new NotImplementedException();
        }

        public ITextEdit CreateEdit()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyRegionEdit CreateReadOnlyRegionEdit()
        {
            throw new NotImplementedException();
        }

        public ITextSnapshot Delete(Span deleteSpan)
        {
            throw new NotImplementedException();
        }

        public NormalizedSpanCollection GetReadOnlyExtents(Span span)
        {
            throw new NotImplementedException();
        }

        public ITextSnapshot Insert(int position, string text)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly(int position) => true;

        public bool IsReadOnly(int position, bool isEdit) => true;

        public bool IsReadOnly(Span span) => true;

        public bool IsReadOnly(Span span, bool isEdit) => true;

        public ITextSnapshot Replace(Span replaceSpan, string replaceWith) => throw new NotImplementedException();

        public void TakeThreadOwnership()
        {
            throw new NotImplementedException();
        }
    }
}
