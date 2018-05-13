using System;
using Microsoft.VisualStudio.Text;

namespace CoCo.Test.Common
{
    internal class TextVersion : ITextVersion
    {
        public TextVersion(ITextBuffer textBuffer, int length)
        {
            TextBuffer = textBuffer ?? throw new ArgumentNullException(nameof(textBuffer));
            Length = length;
        }

        public int VersionNumber => 0;

        public int ReiteratedVersionNumber => 0;

        public ITextVersion Next { get; }

        public INormalizedTextChangeCollection Changes { get; }

        public int Length { get; }

        public ITextBuffer TextBuffer { get; }

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