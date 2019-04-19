using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Aiplugs.CMS
{
  public class Collection
  {
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]    
    public string Name { get; set; }
    
    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

    [JsonProperty("displayOrder")]
    public int DisplayOrder { get; set; } 

    [JsonProperty("titlePath")]
    public string TitlePath { get; set; }

    [JsonProperty("previewTemplate")]
    public string PreviewTemplate { get; set; }

    [JsonProperty("schema")]
    public string Schema { get; set; }

    public IEnumerable<ProcedureInfo> Procedures { get; set; }

    public string GetDisplayName()
    {
      return DisplayName ?? Name;
    }
  }
}