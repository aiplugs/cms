using System;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Core.Data
{
  public class Record : IRecord
  {
    public long Id { get; set; }
    public long ItemId { get; set; }
    public JObject Data { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
  }
}