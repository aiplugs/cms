using System;
using System.Collections.Generic;

namespace Aiplugs.CMS.Web.ViewModels.StorageViewModels
{
    public class FileUploadViewModel
    {
        public int Count { get; set; }
        public long Size { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}