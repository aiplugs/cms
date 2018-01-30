using System.Collections.Generic;
using System.Linq;
using Aiplugs.CMS.Web.Models;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Web.Repositories
{
  public interface IItemRepository
  {
    IQueryable<Item> Get(string collection);
    IQueryable<Item> SearchByKeyword(string collection, string keyword);
    IQueryable<Item> SearchByQuery(string collection, string query);
    Item Find(long id);
    long Add(string collection, JObject data, string userId);
    void Update(Item item, JObject data, string userId);
    void Remove(Item item);
    IQueryable<Record> GetHistory(long itemId);
  }
}