using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS 
{
  public interface IDataService
  {
    IEnumerable<IItem> GetItems(string collection);
    IEnumerable<IItem> SearchItems(string collection);
    JObject Find(long id);
    bool Validate(string collection, JObject item);
    void Add(string collection, JObject item);
    void Update(long id, JObject item);
    IEnumerable<JObject> GetHistory(long id);
  }
}