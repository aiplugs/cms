using System;

namespace Aiplugs.CMS.Core.Data
{
    public class File
    {
        public long Id { get; set; }
        public long FolderId { get; set; }
        public string Name { get; set; }
        public string BinaryPath { get; set;}
        public string ContentType { get; set; }
        public long Size { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }
    }
}