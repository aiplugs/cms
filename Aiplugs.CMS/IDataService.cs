using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS
{
    public interface IDataService
    {
        Task<IEnumerable<IItem>> SearchAsync(string collectionName, string keyword = null, long? skipToken = null, int limit = 100, bool desc = true);
        Task<IEnumerable<IItem>> QueryAsync(string collectionName, string query, long? skipToken = null, int limit = 100, bool desc = true);
        Task<IItem> LookupAsync(long id);
        Task<long> AddAsync(string collectionName, JObject data);
        Task UpdateAsync(long id, JObject data);
        Task<IEnumerable<IRecord>> GetHistoryAsync(long id, long? skipToken = null, int limit = 100);
        Task<IEnumerable<Event>> GetEventsAsync(string collectionName, DateTime from, int limit = 100);
        Task<bool> ValidateAsync(string collectionName, JObject data);
        Task SetStatusAsync(long id, bool isValid);
    }
}