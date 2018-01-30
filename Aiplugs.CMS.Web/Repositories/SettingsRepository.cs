using System;
using System.Collections.Generic;
using System.Linq;
using Aiplugs.CMS.Web.Models;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Web.Repositories
{
  public class SettingsRepository : ISettingsRepository
  {
    const string COLLECTIONS = "__collections__";
    const string SETTINGS = "__settings__";
    private readonly IItemRepository _items;
    private readonly IUserManageService _users;

    public SettingsRepository(IItemRepository itemRepository, IUserManageService userManageService)
    {
      _items = itemRepository;
      _users = userManageService;
    }
    public void Add(Collection collection)
    {
      var user = _users.GetUserAsync().Result;      
      if (user == null)
        throw new ApplicationException("Cannot resolve user");

      if (collection.Id != 0)
        throw new ArgumentException(nameof(collection));

      _items.Add(COLLECTIONS, JObject.FromObject(collection), user.Id);
    }

    public void Replace(Collection collection)
    {
      var user = _users.GetUserAsync().Result;      
      if (user == null)
        throw new ApplicationException("Cannot resolve user");

      var item = _items.Find(collection.Id);      
      if (item == null)
        throw new ArgumentException(nameof(collection));

      _items.Update(item, JObject.FromObject(collection), user.Id);
    }

    internal Collection convertItemToColleciton(Item item) 
    {
      var collection = item.Data.ToObject<Collection>();
      collection.Id = item.Id;
      return collection;
    }

    public Collection FindCollection(long id)
    {
      var item = _items.Find(id);
      if (item == null)
        return null;
      
      return convertItemToColleciton(item);
    }

    public IEnumerable<Collection> GetCollections()
    {
      return _items.Get(COLLECTIONS).ToArray().Select(convertItemToColleciton);
    }

    public Collection GetCollection(string name)
    {
      return GetCollections().Where(collection => collection.Name == name).FirstOrDefault();
    }
    
    public void Remove(Collection collection)
    {
      var item = _items.Find(collection.Id);      
      if (item == null)
        throw new ArgumentException(nameof(collection));
      
      _items.Remove(item);
    }
  }
}