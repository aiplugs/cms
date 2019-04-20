using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS
{
    public interface IDataService
    {
        Task<IEnumerable<IItem>> SearchAsync(string collectionName, string keyword = null, string skipToken = null, int limit = 100, bool desc = true);
        Task<IEnumerable<IItem>> QueryAsync(string collectionName, string query, string skipToken = null, int limit = 100, bool desc = true);
        Task<IItem> LookupAsync(string id);
        Task<string> AddAsync(string collectionName, JToken data);
        Task UpdateAsync(string id, JToken data, string currentId, bool? isValid = null);
        Task<IEnumerable<IRecord>> GetHistoryAsync(string id, string skipToken = null, int limit = 100);
        Task<IRecord> GetRecordThenAsync(string id, DateTime then);
        Task<IEnumerable<Event>> GetEventsAsync(string collectionName, DateTimeOffset from, string skipToken = null, int limit = 100);
        Task<(bool, IEnumerable<string>)> ValidateAsync(string collectionName, JToken data);
        Task SetStatusAsync(string id, bool isValid, string currentId);
    }
}