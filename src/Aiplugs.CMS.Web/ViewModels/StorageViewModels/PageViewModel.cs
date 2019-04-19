using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Web.ViewModels.StorageViewModels
{
    public class PageViewModel
    {
        public string Path { get; set; }
        public IEnumerable<FolderViewModel> Folders { get; set; }
        public IEnumerable<FileViewModel> Files { get; set; }
        public string SkipToken { get; set; }
    }
}
