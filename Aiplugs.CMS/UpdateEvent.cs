using System;

namespace Aiplugs.CMS
{
    public class UpdateEvent : Event
    {
        public UpdateEvent(long id, DateTime trackAt) 
            : base("Update", id, trackAt)
        {}
    }
}