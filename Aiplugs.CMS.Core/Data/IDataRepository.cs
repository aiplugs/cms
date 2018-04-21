using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Query;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Core.Data
{
    public interface IDataRepository
    {
        Task<IItem> LookupAsync(long id);
        Task<IEnumerable<IItem>> GetAsync(string collectionName, string keyword, bool desc, long? skipToken, int limit);
        Task<IEnumerable<IItem>> QueryAsync(string collectionName, IQuery query, bool desc, long? skipToken, int limit);
        Task<IEnumerable<IRecord>> GetHistoryAsync(long id, long? skipToken, int limit);
        Task<IEnumerable<Event>> GetEventsAsync(string collectionName, DateTime from, int limit);
        Task<long> AddAsync(string collectionName, JObject data, string userId);
        Task UpdateAsync(long id, JObject data, string userId);
        Task SetStatusAsync(long id, bool isValid);
        Task RemoveAsync(long id);
    }
}