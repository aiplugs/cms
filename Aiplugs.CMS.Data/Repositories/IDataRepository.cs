using Aiplugs.CMS.Data.Entities;
using Aiplugs.CMS.Query;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Data.Repositories
{
    public interface IDataRepository
    {
        Task<Item> LookupAsync(string id);

        Task<IEnumerable<Item>> GetAsync(string collectionName, string keyword, bool desc, string skipToken, int limit);

        Task<IEnumerable<Item>> QueryAsync(string collectionName, IQuery query, bool desc, string skipToken, int limit);

        Task<IEnumerable<ItemRecord>> GetHistoryAsync(string id, string skipToken, int limit);

        Task<ItemRecord> GetRecordThenAsync(string id, DateTimeOffset then);

        Task<IEnumerable<Event>> GetEventsAsync(string collectionName, DateTimeOffset from, string skipToken, int limit);

        Task<Item> AddAsync(string collectionName, string data, string userId, DateTimeOffset datetime);

        Task RemoveAsync(string id);

        Task UpdateDataAsync(string id, string data, string currentId, string userId, DateTimeOffset datetime);

        Task UpdateStatusAsync(string id, bool isValid, string currentId);

    }
}
