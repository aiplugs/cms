using System;

namespace Aiplugs.CMS.Core.Models
{
    public class File : IFile
    {
        public long Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string BinaryPath { get;  set; }
        public long FolderId { get; set; }
    }
}