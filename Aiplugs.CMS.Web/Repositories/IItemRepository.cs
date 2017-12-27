using System.Collections.Generic;
using System.Linq;
using Aiplugs.CMS.Web.Models;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Web.Repositories
{
  public interface IItemRepository
  {
    IEnumerable<Item> Get(string collection);
    IEnumerable<Item> SearchByKeyword(string collection, string keyword);
    IEnumerable<Item> SearchByQuery(string collection, string query);
    Item Find(long id);
    void Add(string collection, JObject data, string userId);
    void Update(Item item, JObject data, string userId);
    void Remove(Item item);
    IQueryable<Record> GetHistory(long itemId);
  }
}