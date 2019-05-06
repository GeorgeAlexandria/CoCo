using System;
using Microsoft.VisualStudio.Text;

namespace CoCo.Test.Common
{
    internal class TextVersion : ITextVersion2
    {
        private readonly TextImageVersion _textImageVersion;

        public TextVersion(ITextBuffer textBuffer, TextImageVersion imageVersion)
        {
            TextBuffer = textBuffer ?? throw new ArgumentNullException(nameof(textBuffer));
            _textImageVersion = imageVersion ?? throw new ArgumentNullException(nameof(imageVersion));
        }

        public int VersionNumber => ImageVersion.VersionNumber;

        public int ReiteratedVersionNumber => _textImageVersion.ReiteratedVersionNumber;

        public ITextVersion Next { get; }

        public INormalizedTextChangeCollection Changes => ImageVersion.Changes;

        public int Length => ImageVersion.Length;

        public ITextBuffer TextBuffer { get; }

        public ITextImageVersion ImageVersion => _textImageVersion;

        // TODO: isn't completed
        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode) =>
            CreateTrackingPoint(position, trackingMode, TrackingFidelityMode.UndoRedo);

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode) =>
            CreateTrackingSpan(new Span(start, length), trackingMode);

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity) =>
            CreateTrackingSpan(new Span(start, length), trackingMode, trackingFidelity);

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateCustomTrackingSpan(Span span, TrackingFidelityMode trackingFidelity, object customState, CustomTrackToVersion behavior)
        {
            if (behavior == null) throw new ArgumentNullException(nameof(behavior));
            throw new NotImplementedException();
        }
    }
}