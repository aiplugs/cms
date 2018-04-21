using System;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Core.Data
{
  public class Item : IItem
  {
    public long Id { get; set; }
    public string CollectionName { get; set; }
    public JObject Data { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsValid { get; set; }
  }
}