using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS
{
    public interface ISettingsService
    {
        Task<Settings> GetSettingsAsync();
        Task UpdateAsync(Settings settings);
        Task<IEnumerable<Settings>> GetSettingsHistoryAsync();
        Task<IEnumerable<Collection>> GetCollectionsAsync();
        Task<Collection> LookupCollectionAsync(string id);
        Task<Collection> FindCollectionAsync(string collectionName);
        Task<string> AddAsync(Collection collection);
        Task UpdateAsync(Collection collection);
        Task<IEnumerable<Collection>> GetCollectionHistoryAsync(string id);
        Task<(bool,IEnumerable<string>)> ValidateCollectionAsync(string collectionName, JToken data);
    }
}