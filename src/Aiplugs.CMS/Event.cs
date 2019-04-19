using System;

namespace Aiplugs.CMS {
    public abstract class Event {
        public string EventType { get; private set; }
        public string Id { get; private set; }
        public DateTimeOffset TrackAt { get; private set; }
        public Event(string type, string id, DateTimeOffset trackAt)
        {
            Id = id;
            EventType = type;
            TrackAt = trackAt;
        }
    }
}