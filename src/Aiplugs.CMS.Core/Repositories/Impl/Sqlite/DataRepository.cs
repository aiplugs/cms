using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Data.QueryBuilders;
using Aiplugs.CMS.Query;
using Dapper;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Core.Data.Sqlite
{
    public class DataRepository : DataRepositoryBase
    {
        public DataRepository(IDbConnection dbConnection, IQueryBuilder queryBuilder) 
            : base(dbConnection, queryBuilder) 
        {}

        public override async Task<long> AddAsync(string collectionName, JObject data, string userId)
        {
            return await _db.TransactionalAsync(async tran => {
                var now = DateTime.UtcNow;
                var itemId = await _db.ExecuteScalarAsync<long>(
                        @"INSERT INTO 
                            Items (CollectionName, CreatedAt, IsValid)
                            VALUES (@CollectionName, @CreatedAt, 1); 
                          SELECT last_insert_rowid();", new { CollectionName = collectionName, CreatedAt = now }, transaction:tran);

                await UpdateAsync(itemId, data, userId, now, tran);
                
                return itemId;
            });
        }

        public override async Task<IEnumerable<IItem>> GetAsync(string collectionName, string keyword, bool desc, long? skipToken, int limit)
        {
            var sql = 
                @"SELECT
                        i.Id AS Id,
                        i.CollectionName AS CollectionName,
                        r.Data AS Data,
                        i.CreatedAt AS CreatedAt,
                        r.CreatedAt AS UpdatedAt,
                        r.CreatedBy AS UpdatedBy,
                        i.IsValid AS IsValid
                    FROM Items AS i
                  INNER JOIN Records AS r 
                    ON i.CurrentId = r.Id
                  WHERE i.CollectionName = @CollectionName ";
            
            if (string.IsNullOrEmpty(keyword) == false)
                sql += " AND Data LIKE @Keyword";

            if (skipToken.HasValue)
                sql += $" AND i.Id {(desc ? "<" : ">")} @SkipToken";

            sql += $"\nORDER BY Id {(desc ? "DESC" : "ASC")}";
            sql += $"\nLIMIT @Limit";

            var items = await _db.NonTransactionalAsync(async () => 
                            await _db.QueryAsync<DbItem>(sql, new { CollectionName = collectionName, Keyword = $"%{keyword}%", SkipToken = skipToken, Limit = limit }));
            
            return items.Select(item => new Item 
            {
                Id = item.Id,
                CollectionName = item.CollectionName,
                Data = JObject.Parse(item.Data),
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                UpdatedBy = item.UpdatedBy,
                IsValid = item.IsValid,
            });
        }

        public override async Task<IEnumerable<IRecord>> GetHistoryAsync(long id, long? skipToken, int limit)
        {
            var sql = 
                @"SELECT Id, ItemId, Data, CreatedAt, CreatedBy
                    FROM Records
                  WHERE ItemId = @ItemId";

            if (skipToken.HasValue)
                sql += $"AND Id < @SkipToken";

            sql += $"\nORDER BY Id DESC";
            sql += $"\nLIMIT @Limit";
            
            var records = await _db.NonTransactionalAsync(async () => 
                            await _db.QueryAsync<DbRecord>(sql, new { ItemId = id, SkipToken = skipToken, Limit = limit }));
            
            return records.Select(record => new Record
            {
                Id = record.Id,
                ItemId = record.ItemId,
                Data = JObject.Parse(record.Data),
                CreatedAt = record.CreatedAt,
                CreatedBy = record.CreatedBy,
            });
        }

        public override async Task<IEnumerable<IItem>> QueryAsync(string collectionName, IQuery query, bool desc, long? skipToken, int limit)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var conditions = _builder.Build(query);

            var sql = 
                @"SELECT
                        i.Id AS Id,
                        i.CollectionName AS CollectionName,
                        r.Data AS Data,
                        i.CreatedAt AS CreatedAt,
                        r.CreatedAt AS UpdatedAt,
                        r.CreatedBy AS UpdatedBy,
                        i.IsValid AS IsValid
                    FROM Items AS i
                  INNER JOIN Records AS r 
                    ON i.CurrentId = r.Id
                  WHERE i.CollectionName = @CollectionName AND " + conditions;
            
            if (skipToken.HasValue)
                sql += $" AND i.Id {(desc ? "<" : ">")} @SkipToken";

            sql += $"\nORDER BY Id {(desc ? "DESC" : "ASC")}";
            sql += $"\nLIMIT @Limit";

            var items = await _db.NonTransactionalAsync(async () => 
                            await _db.QueryAsync<DbItem>(sql, new { CollectionName = collectionName, SkipToken = skipToken, Limit = limit }));
            
            return items.Select(item => new Item 
            {
                Id = item.Id,
                CollectionName = item.CollectionName,
                Data = JObject.Parse(item.Data),
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                UpdatedBy = item.UpdatedBy,
                IsValid = item.IsValid,
            });
        }

        public override async Task<IEnumerable<Event>> GetEventsAsync(string collectionName, DateTime from, long? skipToken, int limit)
        {
            var sql = @"SELECT i.Id AS Id,
                               i.CreatedAt AS CreatedAt,
                               MIN(r.CreatedAt) AS UpdatedAt
                            FROM Records AS r
                        INNER JOIN Items AS i 
                            ON i.Id = r.ItemId
                        WHERE i.CollectionName = @CollectionName AND r.CreatedAt >= @From ";

            if (skipToken.HasValue)
                sql += " AND i.Id > @SkipToken";

            sql += "\nGROUP BY i.Id";
            sql += "\nLIMIT @Limit";

            return await _db.NonTransactionalAsync(async() => {
                var items = await _db.QueryAsync<DbItem>(sql, new { CollectionName = collectionName, From = from, SkipToken = skipToken, Limit = limit });

                return items.Select<DbItem,Event>(item => {
                    if (item.UpdatedAt == item.CreatedAt)
                        return new CreateEvent(item.Id, item.UpdatedAt);

                    return new UpdateEvent(item.Id, item.UpdatedAt);
                });
            });
        }
    }
}