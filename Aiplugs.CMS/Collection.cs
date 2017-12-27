using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Aiplugs.CMS
{
  public class Collection
  {
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public JsonSchema Schema { get; set; }
  }
}