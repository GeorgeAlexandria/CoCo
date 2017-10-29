using System;
using Microsoft.VisualStudio.Text;

namespace CoCoTests
{
    internal class TextImageVersion : ITextImageVersion
    {
        internal TextImageVersion(int versionNumber, int length, object identifier)
        {
            VersionNumber = versionNumber;
            Identifier = identifier;
            Length = length;
        }

        public ITextImageVersion Next { get; }

        public int Length { get; }

        public INormalizedTextChangeCollection Changes { get; }

        public int VersionNumber { get; }

        public object Identifier { get; }

        public int ReiteratedVersionNumber => throw new NotImplementedException();

        public int TrackTo(VersionedPosition other, PointTrackingMode mode)
        {
            if (other.Version == null) throw new ArgumentException(nameof(other));
            if (other.Version.VersionNumber == VersionNumber) return other.Position;
            if (other.Version.VersionNumber > VersionNumber) return Tracking.TrackPositionForwardInTime(mode, other.Position, this, other.Version);
            return Tracking.TrackPositionBackwardInTime(mode, other.Position, this, other.Version);
        }

        public Span TrackTo(VersionedSpan span, SpanTrackingMode mode)
        {
            if (span.Version == null) throw new ArgumentException(nameof(span));
            if (span.Version.VersionNumber == VersionNumber) return span.Span;
            if (span.Version.VersionNumber > VersionNumber) return Tracking.TrackSpanForwardInTime(mode, span.Span, this, span.Version);
            return Tracking.TrackSpanBackwardInTime(mode, span.Span, this, span.Version);
        }
    }
}