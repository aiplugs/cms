using System;
using System.Collections.Generic;
using System.Linq;
using Aiplugs.CMS.Web.Data;
using Aiplugs.CMS.Web.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Web.Repositories
{
  public class ItemRepository : IItemRepository
  {
    private readonly ApplicationDbContext db;
    public ItemRepository(ApplicationDbContext dbContext) {
      db = dbContext;
    }
    public long Add(string collection, JObject data, string userId)
    {
      using (var tran = db.Database.BeginTransaction())
      {
        var item = new Item { CollectionName = collection };
        db.Items.Add(item);
        db.SaveChanges();
      
        var record = new Record { Data = data, CreatedBy = userId, CreatedAt = DateTimeOffset.Now };
        item.History.Add(record);
        db.SaveChanges();
        
        item.Current = record;
        db.SaveChanges();

        tran.Commit();

        return item.Id;
      }
    }

    public Item Find(long id)
    {
      return db.Items.Include(item => item.Current).Where(item =>  item.Id == id).FirstOrDefault();
    }

    public IQueryable<Item> Get(string collection)
    {                 
      return db.Items.Include(item => item.Current).Where(item => item.CollectionName == collection);
    }

    public IQueryable<Record> GetHistory(long itemId)
    {
      return db.Records.Where(r => r.ItemId == itemId).OrderByDescending(r => r.Id);
    }

    public void Remove(Item item)
    {
      using (var tran = db.Database.BeginTransaction())
      {
        item.CurrentId = null;
        db.SaveChanges();

        db.Records.RemoveRange(db.Records.Where(r => r.ItemId == item.Id));
        db.Items.Remove(item);
        db.SaveChanges();

        tran.Commit();
      }
    }

    public IQueryable<Item> SearchByKeyword(string collection, string keyword)
    {
      return db.Items.Include(it => it.Current).Where(it => it.CollectionName == collection && it.Current.JSON.Contains(keyword));
    }

    public IQueryable<Item> SearchByQuery(string collection, string query)
    {
      throw new System.NotImplementedException();
    }

    public void Update(Item item, JObject data, string userId)
    {
      using (var tran = db.Database.BeginTransaction())
      {
        var record = new Record { Data = data, CreatedBy = userId, CreatedAt = DateTimeOffset.Now };
        item.History.Add(record);
        db.SaveChanges();
        
        item.Current = record;
        db.SaveChanges();

        tran.Commit();
      }
    }
  }
}