using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Core.Data
{
    public interface ISettingsRepository
    {
        Task<Settings> GetSettingsAsync();
        Task UpdateAsync(Settings settings, string userId);
        Task<IEnumerable<Collection>> GetCollectionsAsync();
        Task<Collection> LookupCollectionAsync(long id);
        Task<Collection> FindCollectionAsync(string collectionName);
        Task<long> AddAsync(Collection collection, string userId);
        Task UpdateAsync(Collection collection, string userId);
        Task RemoveAsync(Collection collection);
    }
}