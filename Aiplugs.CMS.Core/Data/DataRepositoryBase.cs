using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Query;
using Dapper;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Core.Data
{
    public abstract class DataRepositoryBase : IDataRepository
    {
        protected readonly IDbConnection _db;
        protected readonly IQueryBuilder _builder;
        public DataRepositoryBase(IDbConnection dbConnection, IQueryBuilder queryBuilder)
        {
            _db = dbConnection;
            _builder = queryBuilder;
        }

        protected class DbItem
        {
            public long Id { get; set; }
            public string CollectionName { get; set; }
            public string Data { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public string UpdatedBy { get; set; }
            public bool IsValid { get; set; }
        }

        protected class DbRecord
        {
            public long Id { get; set; }
            public long ItemId { get; set; }
            public string Data { get; set; }
            public DateTime CreatedAt { get; set; }
            public string CreatedBy { get; set; }
        }

        public abstract Task<long> AddAsync(string collectionName, JObject data, string userId);
        public abstract Task<IEnumerable<IItem>> GetAsync(string collectionName, string keyword, bool desc, long? skipToken, int limit);
        public abstract Task<IEnumerable<IItem>> QueryAsync(string collectionName, IQuery query, bool desc, long? skipToken, int limit);
        public abstract Task<IEnumerable<IRecord>> GetHistoryAsync(long id, long? skipToken, int limit);

        public abstract Task<IEnumerable<Event>> GetEventsAsync(string collectionName, DateTime from, int limit);

        public async Task<IItem> LookupAsync(long id)
        {
            return await _db.NonTransactionalAsync(async() => {
                var item = await _db.QuerySingleOrDefaultAsync<DbItem>(
                    @"SELECT i.Id AS Id,
                             i.CollectionName AS CollectionName,
                             r.Data AS Data,
                             i.CreatedAt AS CreatedAt,
                             r.CreatedAt AS UpdatedAt,
                             r.CreatedBy AS UpdatedBy,
                             i.IsValid AS IsValid
                        FROM Items AS i
                    INNER JOIN Records AS r 
                        ON i.CurrentId = r.Id
                    WHERE i.Id = @Id", new { Id = id });

                if (item == null)
                    return null;
                
                return new Item 
                {
                    Id = item.Id,
                    CollectionName = item.CollectionName,
                    Data = JObject.Parse(item.Data),
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                    UpdatedBy = item.UpdatedBy,
                    IsValid = item.IsValid,
                };
            });
        }

        public async Task RemoveAsync(long id)
        {
            await _db.TransactionalAsync(async tran => {
                await _db.ExecuteAsync(
                        @"DELETE FROM Records
                          WHERE ItemId = @Id", new { Id = id }, transaction:tran);

                await _db.ExecuteAsync(
                        @"DELETE FROM Items
                          WHERE Id = @Id", new { Id = id }, transaction:tran);
            });
        }

        public async Task UpdateAsync(long id, JObject data, string userId, DateTime updatedAt, IDbTransaction tran)
        {
            var recordId = await _db.ExecuteScalarAsync<long>(
                    @"INSERT INTO 
                        Records (ItemId, Data, CreatedBy, CreatedAt) 
                        VALUES (@ItemId, @Data, @CreatedBy, @CreatedAt); 
                        SELECT last_insert_rowid();",new { ItemId = id, Data = data.ToString(), CreatedBy = userId, CreatedAt = updatedAt }, transaction:tran);
            
            await _db.ExecuteAsync(
                    @"UPDATE Items
                        SET CurrentId = @CurrentId, IsValid = 1
                        WHERE Id = @Id", new { Id = id, CurrentId = recordId }, transaction:tran);
        }
        public async Task UpdateAsync(long id, JObject data, string userId)
        {
            await _db.TransactionalAsync(async tran => {
                await UpdateAsync(id, data, userId, DateTime.UtcNow, tran);
            });
        }

        public async Task SetStatusAsync(long id, bool isValid)
        {
            await _db.TransactionalAsync(async tran => {
                await _db.ExecuteAsync(
                        @"UPDATE Items
                            SET IsValid = @IsValid
                          WHERE Id = @Id", new { Id = id, IsValid = isValid }, transaction:tran);
            });
        }
    }
}