using System;

namespace Aiplugs.CMS.Data.Entities
{
    public class Template
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTimeOffset LastModifiedAt { get; set; }
    }
}
