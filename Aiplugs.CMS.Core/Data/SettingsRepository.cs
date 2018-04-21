using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Query;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Core.Data {
    public abstract class SettingsRepository : ISettingsRepository
    {
        private const string COLLECTIONS = "__collections__";
        private const string SETTINGS = "__settings__";
        private readonly IDataRepository _repository;
        public SettingsRepository(IDataRepository repository)
        {
            _repository = repository;
        }

        public async Task<Settings> GetSettingsAsync()
        {
            var item = (await _repository.GetAsync(SETTINGS, null, false, null, 1)).FirstOrDefault();

            if (item == null)
                return new Settings();

            return item.Data.ToObject<Settings>();
        }

        public async Task UpdateAsync(Settings settings, string userId)
        {
            var item = (await _repository.GetAsync(SETTINGS, null, false, null, 1)).FirstOrDefault();
            if (item == null)
                await _repository.AddAsync(SETTINGS,  JObject.FromObject(settings), userId);
            else
                await _repository.UpdateAsync(item.Id, JObject.FromObject(settings), userId);
        }

        public async Task<long> AddAsync(Collection collection, string userId)
        {
            return await _repository.AddAsync(COLLECTIONS, JObject.FromObject(collection), userId);
        }

        public async Task<Collection> FindCollectionAsync(string collectionName)
        {
            return ToCollection((await _repository.QueryAsync(COLLECTIONS, QParser.Parse($"$.name = '{collectionName}'").Expression, false, null, 1)).FirstOrDefault());
        }

        public async Task<IEnumerable<Collection>> GetCollectionsAsync()
        {
            return (await _repository.GetAsync(COLLECTIONS, null, false, null, 100)).Select(item => ToCollection(item)).ToArray().OrderBy(c => c.Name);
        }

        public async Task<Collection> LookupCollectionAsync(long id)
        {
            return ToCollection(await _repository.LookupAsync(id));
        }

        public async Task RemoveAsync(Collection collection)
        {
            await _repository.RemoveAsync(collection.Id);
        }

        public async Task UpdateAsync(Collection collection, string userId)
        {
            await _repository.UpdateAsync(collection.Id, JObject.FromObject(collection), userId);
        }

        private Collection ToCollection(IItem item)
        {
            if (item == null)
                return null;

            var collection = item.Data.ToObject<Collection>();
            collection.Id = item.Id;
            return collection;
        }
    }
}