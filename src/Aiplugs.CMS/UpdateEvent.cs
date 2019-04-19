using System;

namespace Aiplugs.CMS
{
    public class UpdateEvent : Event
    {
        public UpdateEvent(string id, DateTimeOffset trackAt) 
            : base("Update", id, trackAt)
        {}
    }
}