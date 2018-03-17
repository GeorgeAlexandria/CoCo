using System;
using Microsoft.VisualStudio.Text;

namespace CoCo.Test.Common
{
    internal class TrackingPoint : ITrackingPoint
    {
        private ITextVersion _textVersion;
        private int _position;

        internal TrackingPoint(ITextVersion version, int position, PointTrackingMode trackingMode)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (position < 0 || position > version.Length) throw new ArgumentOutOfRangeException(nameof(position));
            if (trackingMode < PointTrackingMode.Positive || trackingMode > PointTrackingMode.Negative) throw new ArgumentOutOfRangeException(nameof(trackingMode));

            _textVersion = version;
            _position = position;
            TextBuffer = version.TextBuffer;
            TrackingMode = trackingMode;
        }

        public ITextBuffer TextBuffer { get; }

        public PointTrackingMode TrackingMode { get; }

        public TrackingFidelityMode TrackingFidelity { get; }

        public int GetPosition(ITextVersion version)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (version.TextBuffer != TextBuffer) throw new ArgumentException(nameof(version.TextBuffer));
            return TrackPosition(version);
        }

        public int GetPosition(ITextSnapshot snapshot)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            if (snapshot.TextBuffer != TextBuffer) throw new ArgumentException(nameof(snapshot.TextBuffer));
            return TrackPosition(snapshot.Version);
        }

        public SnapshotPoint GetPoint(ITextSnapshot snapshot) => new SnapshotPoint(snapshot, GetPosition(snapshot));

        public char GetCharacter(ITextSnapshot snapshot) => GetPoint(snapshot).GetChar();

        private int TrackPosition(ITextVersion targetVersion)
        {
            if (_textVersion == targetVersion) return _position;

            _position = Tracking.TrackPositionForwardInTime(TrackingMode, _position, _textVersion, targetVersion);
            if (targetVersion.VersionNumber > _textVersion.VersionNumber)
            {
                _textVersion = targetVersion;
            }
            return _position;
        }
    }
}
