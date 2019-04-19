using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Data.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private const string COLLECTION_NAME__SETTINGS = "__settings__";
        private const string COLLECTION_NAME__COLECTIONS = "__collections__";
        private readonly IDataRepository _data;

        public SettingsRepository(IDataRepository data)
        {
            _data = data;
        }

        public async Task<string> AddAsync(Collection collection, string userId, DateTimeOffset datetime)
        {
            var item = await _data.AddAsync(COLLECTION_NAME__COLECTIONS, JsonConvert.SerializeObject(collection), userId, datetime);

            return item.Id;
        }

        public async Task<Collection> FindCollectionAsync(string collectionName)
        {
            var collections = await GetCollectionsAsync();

            return collections.Where(c => c.Name == collectionName).FirstOrDefault();
        }

        public async Task<IEnumerable<Collection>> GetCollectionsAsync()
        {
            return (await _data.GetAsync(COLLECTION_NAME__COLECTIONS, null, false, null, int.MaxValue))
                .Select(item => JsonConvert.DeserializeObject<Collection>(item.Data))
                .ToArray();
        }

        public async Task<Settings> GetSettingsAsync()
        {
            var item = (await _data.GetAsync(COLLECTION_NAME__SETTINGS, null, false, null, 1)).FirstOrDefault();

            if (item == null)
                return null;

            return JsonConvert.DeserializeObject<Settings>(item.Data);
        }

        public async Task<Collection> LookupCollectionAsync(string id)
        {
            var item = await _data.LookupAsync(id);

            if (item == null)
                return null;

            return JsonConvert.DeserializeObject<Collection>(item.Data);
        }

        public async Task UpdateAsync(Collection collection, string userId, DateTimeOffset datetime)
        {
            var item = (await _data.GetAsync(COLLECTION_NAME__COLECTIONS, null, false, null, int.MaxValue))
                .Where(d => JsonConvert.DeserializeObject<Collection>(d.Data)?.Name == collection.Name)
                .FirstOrDefault();

            if (item == null)
                throw new InvalidOperationException();

            await _data.UpdateDataAsync(item.Id, JsonConvert.SerializeObject(collection), item.CurrentId, userId, datetime);
        }

        public Task UpdateAsync(Settings settings, string userId, DateTimeOffset datetime)
        {
            throw new NotImplementedException();
        }
    }
}
