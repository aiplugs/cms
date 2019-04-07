using System;

namespace Aiplugs.CMS
{
    public class CreateEvent : Event
    {
        public CreateEvent(string id, DateTimeOffset trackAt) 
            : base("Create", id, trackAt)
        {}
    }
}