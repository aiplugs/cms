using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiplugs.CMS.Data.Entities
{
    public class Item
    {
        [Key]
        public string Id { get; set; }

        public string CollectionName { get; set; }

        public string CurrentId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsValid { get; set; }
        
        public string Data { get; set; }

        public ICollection<ItemRecord> History { get; set; }
    }
}
