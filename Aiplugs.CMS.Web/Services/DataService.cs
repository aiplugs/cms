using System;
using System.Collections.Generic;
using System.Linq;
using Aiplugs.CMS.Web.Repositories;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Web.Services
{
  public class DataService : IDataService
  {
    private readonly IItemRepository items;
    private readonly IUserManageService userResolver;
    private readonly IDataValidateService dataValidator;
    public DataService(IItemRepository repository, IUserManageService resolver/*, IDataValidateService validator*/) 
    {
      items = repository;
      userResolver = resolver;
      // dataValidator = validator;
    }
    public long Add(string collection, JObject data)
    {
      var user = userResolver.GetUserAsync().Result;
      if (user == null)
        throw new ApplicationException("Cannot resolve username");

      return items.Add(collection, data, user.Id);
    }

    public JObject Find(long id)
    {
      return items.Find(id)?.Data;
    }

    public IEnumerable<JObject> GetHistory(long id)
    {
      return items.GetHistory(id).Select(r => r.Data);
    }

    public IQueryable<IItem> GetItems(string collection)
    {
      return items.Get(collection);
    }
    public IEnumerable<IItem> GetItems(string collection, long? skipToken, int limit, bool desc = true)
    {
      var query = GetItems(collection);
      
      if (desc)
        query = query.OrderByDescending(item => item.Id);      
      else
        query = query.OrderBy(item => item.Id);
      
      if (skipToken.HasValue) 
        query = query.Where(item => item.Id > skipToken);

      return items.Get(collection).Take(limit).ToArray();
    }
    public IQueryable<IItem> SearchItems(string collection)
    {
      throw new System.NotImplementedException();
    }
    public IEnumerable<IItem> SearchItems(string collection, int page, int limit)
    {
      throw new System.NotImplementedException();
    }

    public void Update(long id, JObject data)
    {
      var item = items.Find(id);
      if (item == null) 
        throw new ArgumentException($"Item({id}) is not found. ");

      var user = userResolver.GetUserAsync().Result;
      if (user == null)
        throw new ApplicationException("Cannot resolve username");
      
      items.Update(item, data, user.Id);
    }

    public bool Validate(string collection, JObject data)
    {
      // return dataValidator.Validate(collection, data);
      return true;
    }
  }
}