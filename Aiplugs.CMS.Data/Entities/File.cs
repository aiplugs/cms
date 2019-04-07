using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiplugs.CMS.Data.Entities
{
    public class File
    {
        [Key]
        public string Id { get; set; }

        public string FolderId { get; set; }

        public string Name { get; set; }
        
        public string BinaryPath { get; set; }

        public string ContentType { get; set; }

        public long Size { get; set; }

        public DateTimeOffset LastModifiedAt { get; set; }

        public string LastModifiedBy { get; set; }
    }
}
