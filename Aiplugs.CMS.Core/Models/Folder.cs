using System;
using System.Collections.Generic;

namespace Aiplugs.CMS.Core.Models
{
    public class Folder : IFolder
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}