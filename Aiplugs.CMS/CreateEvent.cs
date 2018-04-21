using System;

namespace Aiplugs.CMS
{
    public class CreateEvent : Event
    {
        public CreateEvent(long id, DateTime trackAt) 
            : base("Create", id, trackAt)
        {}
    }
}