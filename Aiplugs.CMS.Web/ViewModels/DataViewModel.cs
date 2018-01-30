using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Web.ViewModels
{
  public class DataViewModel
  {
    public string Uri { get; set; }
    public JObject Data { get; set; }
  }
}