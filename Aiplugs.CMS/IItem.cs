using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS
{
  public interface IItem
  {
    long Id { get; }
    JObject Data { get; }
  }
}