using System;

namespace Aiplugs.CMS
{
    public class Template
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CurrentId { get; set; }
        public string Text { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTimeOffset LastModifiedAt { get; set; }
    }
}
