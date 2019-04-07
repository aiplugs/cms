using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Data.Entities;
using Aiplugs.CMS.Data.QueryBuilders;
using Aiplugs.CMS.Query;
using Microsoft.EntityFrameworkCore;

namespace Aiplugs.CMS.Data.Repositories
{
    public class DataRepository : IDataRepository
    {
        private readonly AiplugsDbContext _db;
        private readonly IQueryBuilder _queryBuilder;
        public DataRepository(AiplugsDbContext db, IQueryBuilder queryBuilder)
        {
            _db = db;
            _queryBuilder = queryBuilder;
        }

        public async Task<IEnumerable<Item>> GetAsync(string collectionName, string keyword, bool desc, string skipToken, int limit)
        {
            var query = _db.Items.AsNoTracking().Where(item => item.CollectionName == collectionName);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(item => EF.Functions.Like(item.Data, $"%{keyword}%"));

            if (!string.IsNullOrEmpty(skipToken))
            {
                if (desc)
                    query = query.Where(item => string.Compare(item.Id, skipToken) > 0);
                else
                    query = query.Where(item => string.Compare(item.Id, skipToken) < 0);
            }

            if (desc)
                query = query.OrderBy(item => item.Id);
            else
                query = query.OrderByDescending(item => item.Id);

            return await query.Take(limit).ToArrayAsync();
        }

        public async Task<IEnumerable<Item>> QueryAsync(string collectionName, IQuery query, bool desc, string skipToken, int limit)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var conditions = _queryBuilder.Build(query);

            var sql =
                @"SELECT
                        i.Id,
                        i.CollectionName,
                        i.CurrentId,
                        i.Data,
                        i.CreatedAt,
                        i.UpdatedAt,
                        i.CreatedBy,
                        i.UpdatedBy,
                        i.IsValid
                    FROM Items AS i
                  WHERE i.CollectionName = @CollectionName AND ( " + conditions + " )";

            if (!string.IsNullOrEmpty(skipToken))
                sql += $" AND i.Id {(desc ? ">" : "<")} @SkipToken";

            sql += $"\nORDER BY i.Id {(desc ? "ASC" : "DESC")}";
            sql += $"\nLIMIT @Limit";
            
            var result = new List<Item>();
            using (var connection = _db.Database.GetDbConnection())
            {
                connection.Open();
                using (var dbcmd = connection.CreateCommand())
                {
                    dbcmd.CommandText = sql;
                    {
                        var p = dbcmd.CreateParameter();
                        p.ParameterName = "CollectionName";
                        p.Value = collectionName;
                        dbcmd.Parameters.Add(p);
                    }
                    {
                        var p = dbcmd.CreateParameter();
                        p.ParameterName = "SkipToken";
                        p.Value = skipToken;
                        dbcmd.Parameters.Add(p);
                    }
                    {
                        var p = dbcmd.CreateParameter();
                        p.ParameterName = "Limit";
                        p.Value = limit;
                        dbcmd.Parameters.Add(p);
                    }

                    using (var reader = await dbcmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new Item
                            {
                                Id = reader.GetString(0),
                                CollectionName = reader.GetString(1),
                                CurrentId = reader.GetString(2),
                                Data = reader.GetString(3),
                                CreatedAt = reader.GetFieldValue<DateTimeOffset>(4),
                                UpdatedAt = reader.GetFieldValue<DateTimeOffset>(5),
                                CreatedBy = reader.GetString(6),
                                UpdatedBy = reader.GetString(7),
                                IsValid = reader.GetBoolean(8),
                            });
                        }
                    }
                }
            }
            return result;
        }
        
        public async Task<IEnumerable<Event>> GetEventsAsync(string collectionName, DateTimeOffset from, string skipToken, int limit)
        {
            var sql = @"SELECT i.Id AS Id,
                               i.CreatedAt AS CreatedAt,
                               MIN(r.CreatedAt) AS UpdatedAt
                            FROM Records AS r
                        INNER JOIN Items AS i 
                            ON i.Id = r.ItemId
                        WHERE i.CollectionName = @CollectionName AND r.CreatedAt >= @From ";
            
            if (!string.IsNullOrEmpty(skipToken))
                sql += " AND i.Id > @SkipToken";

            sql += "\nGROUP BY i.Id";
            sql += "\nLIMIT @Limit";

            var result = new List<Event>();

            using (var dbcmd = _db.Database.GetDbConnection().CreateCommand())
            {
                dbcmd.CommandText = sql;
                {
                    var p = dbcmd.CreateParameter();
                    p.ParameterName = "CollectionName";
                    p.Value = collectionName;
                    dbcmd.Parameters.Add(p);
                }
                {
                    var p = dbcmd.CreateParameter();
                    p.ParameterName = "From";
                    p.Value = from;
                    dbcmd.Parameters.Add(p);
                }
                {
                    var p = dbcmd.CreateParameter();
                    p.ParameterName = "SkipToken";
                    p.Value = skipToken;
                    dbcmd.Parameters.Add(p);
                }
                {
                    var p = dbcmd.CreateParameter();
                    p.ParameterName = "Limit";
                    p.Value = limit;
                    dbcmd.Parameters.Add(p);
                }

                using (var reader = await dbcmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var id = reader.GetString(0);
                        var createdAt = reader.GetFieldValue<DateTimeOffset>(1);
                        var updatedAt = reader.GetFieldValue<DateTimeOffset>(2);

                        if (updatedAt == createdAt)
                            result.Add(new CreateEvent(id, createdAt));

                        else
                            result.Add(new UpdateEvent(id, updatedAt));
                    }
                }
            }

            return result;
        }
        
        public async Task<IEnumerable<ItemRecord>> GetHistoryAsync(string id, string skipToken, int limit)
        {
            var query = _db.Records.AsNoTracking().Where(record => record.ItemId == id);

            if (!string.IsNullOrEmpty(skipToken))
                query = query.Where(record => string.Compare(record.Id, skipToken) > 0);

            return await query.OrderBy(record => record.Id).Take(limit).ToArrayAsync();
        }

        public Task<ItemRecord> GetRecordThenAsync(string id, DateTimeOffset then)
        {
            return _db.Records.AsNoTracking().Where(record => record.ItemId == id && record.CreatedAt <= then).OrderBy(record => record.Id).FirstOrDefaultAsync();
        }

        public Task<Item> LookupAsync(string id)
        {
            return _db.Items.AsNoTracking().Where(item => item.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Item> AddAsync(string collectionName, string data, string userId, DateTimeOffset datetime)
        {
            using (var tran = await _db.Database.BeginTransactionAsync())
            {
                var item = new Item
                {
                    Id = Helper.CreateKey(),
                    CollectionName = collectionName,
                    CreatedAt = datetime,
                    UpdatedAt = datetime,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    IsValid = true,
                };

                await _db.AddAsync(item);

                await _db.SaveChangesAsync();

                var record = new ItemRecord
                {
                    Id = Helper.CreateKey(),
                    ItemId = item.Id,
                    Data = data,
                    CreatedAt = datetime,
                    CreatedBy = userId,
                };

                item.Data = data;
                item.CurrentId = record.Id;

                await _db.AddAsync(record);

                await _db.SaveChangesAsync();

                tran.Commit();

                _db.Entry(item).State = EntityState.Detached;

                return item;
            }


        }

        public async Task RemoveAsync(string id)
        {
            var entry = _db.Items.Attach(new Item { Id = id });

            entry.State = EntityState.Deleted;

            await _db.SaveChangesAsync();
        }

        public async Task UpdateDataAsync(string id, string data, string currentId, string userId, DateTimeOffset datetime)
        {
            using (var tran = await _db.Database.BeginTransactionAsync())
            {
                var actual = await _db.Items.Where(o => o.Id == id).Select(o => o.CurrentId).FirstOrDefaultAsync();

                if (actual != currentId)
                    throw new DbUpdateConcurrencyException("Cannot update item", null);

                var record = new ItemRecord
                {
                    Id = Helper.CreateKey(),
                    ItemId = id,
                    Data = data,
                    CreatedAt = datetime,
                    CreatedBy = userId,
                };

                await _db.Records.AddAsync(record);

                var item = new Item
                {
                    Id = id,
                    Data = data,
                    UpdatedAt = record.CreatedAt,
                    UpdatedBy = record.CreatedBy,
                    CurrentId = record.Id
                };
                var entry = _db.Items.Attach(item);
                entry.Property(o => o.Data).IsModified = true;
                entry.Property(o => o.UpdatedAt).IsModified = true;
                entry.Property(o => o.UpdatedBy).IsModified = true;
                entry.Property(o => o.CurrentId).IsModified = true;

                await _db.SaveChangesAsync();

                tran.Commit();

                _db.Entry(item).State = EntityState.Detached;
            }
        }

        public async Task UpdateStatusAsync(string id, bool isValid, string currentId)
        {
            using (var tran = await _db.Database.BeginTransactionAsync())
            {
                var actual = await _db.Items.Where(o => o.Id == id).Select(o => o.CurrentId).FirstOrDefaultAsync();

                if (actual != currentId)
                    throw new DbUpdateConcurrencyException("Cannot update item", null);

                var item = new Item
                {
                    Id = id,
                    IsValid = isValid
                };
                var entry = _db.Items.Attach(item);
                entry.Property(o => o.IsValid).IsModified = true;

                await _db.SaveChangesAsync();

                tran.Commit();

                _db.Entry(item).State = EntityState.Detached;
            }
        }
    }
}
