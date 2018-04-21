using System;

namespace Aiplugs.CMS {
    public abstract class Event {
        public string EventType { get; private set; }
        public long Id { get; private set; }
        public DateTime TrackAt { get; private set; }
        public Event(string type, long id, DateTime trackAt)
        {
            Id = id;
            EventType = type;
            TrackAt = trackAt;
        }
    }
}