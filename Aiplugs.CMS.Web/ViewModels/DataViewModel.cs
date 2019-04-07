using Aiplugs.CMS.Web.Models;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Web.ViewModels
{
    public class DataViewModel
    {
        public string CollectionName { get; set; }
        public IItem Item { get; set; }
    }
    public class EditViewModel
    {
        public string CollectionName { get; set; }
        public string Uri { get; set; }
        public CMSSchema Schema { get; set; }
        public JToken Data { get; set; }
    }
}