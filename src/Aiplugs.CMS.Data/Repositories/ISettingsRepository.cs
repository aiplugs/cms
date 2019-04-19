using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Data.Repositories
{
    public interface ISettingsRepository
    {
        Task<string> AddAsync(Collection collection, string userId, DateTimeOffset datetime);
        Task<Collection> LookupCollectionAsync(string id);
        Task<Collection> FindCollectionAsync(string collectionName);
        Task<IEnumerable<Collection>> GetCollectionsAsync();
        Task<Settings> GetSettingsAsync();
        Task UpdateAsync(Collection collection, string userId, DateTimeOffset datetime);
        Task UpdateAsync(Settings settings, string userId, DateTimeOffset datetime);
    }
}
