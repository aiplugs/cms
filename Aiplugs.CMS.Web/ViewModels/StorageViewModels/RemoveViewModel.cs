using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Web.ViewModels.StorageViewModels
{
    public class RemoveViewModel
    {
        public IEnumerable<string> Files { get; set; }

        public IEnumerable<string> Folders { get; set; }
    }
}
