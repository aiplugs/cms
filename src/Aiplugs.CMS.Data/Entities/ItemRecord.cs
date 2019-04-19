using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiplugs.CMS.Data.Entities
{
    public class ItemRecord
    {
        [Key]
        public string Id { get; set; }

        public string ItemId { get; set; }
        
        public string Data { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
