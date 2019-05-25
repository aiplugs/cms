using System;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Web.ViewModels
{
    public class DataPreviewViewModel
    {
        public IItem Item { get; set; }
        public Collection Collection { get; set; }
        public Func<string, Task<string>> ResolveUserNameAsync { get; set; }
    }
}