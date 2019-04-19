using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Models;
using Aiplugs.CMS.Data.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Aiplugs.CMS.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IAppConfiguration _config;
        private readonly ISettingsRepository _repository;
        private readonly IUserResolver _resolver;
        private readonly IDistributedCache _cache;
        public SettingsService(IAppConfiguration config, ISettingsRepository repository, IUserResolver userResolver, IDistributedCache cache)
        {
            _config = config;
            _repository = repository;
            _resolver  = userResolver;
            _cache = cache;
        }
        public async Task<string> AddAsync(Collection collection)
        {
            var userId = _resolver.GetUserId();
            
            if (userId == null)
                throw new UnauthorizedAccessException();

            return await _repository.AddAsync(collection, userId, DateTimeOffset.UtcNow);
        }

        public async Task<Collection> LookupCollectionAsync(string id)
        {
            return await _repository.LookupCollectionAsync(id);
        }

        public async Task<Collection> FindCollectionAsync(string collectionName)
        {
            return await _repository.FindCollectionAsync(collectionName);
        }

        public Task<IEnumerable<Collection>> GetCollectionHistoryAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<Collection>> GetCollectionsAsync()
        {
            return await _repository.GetCollectionsAsync();
        }

        public async Task<Settings> GetSettingsAsync()
        {
            return await _repository.GetSettingsAsync();
        }

        public Task<IEnumerable<Settings>> GetSettingsHistoryAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task UpdateAsync(Settings settings)
        {
            var userId = _resolver.GetUserId();
            
            if (userId == null)
                throw new UnauthorizedAccessException();

            await _repository.UpdateAsync(settings, userId, DateTimeOffset.UtcNow);
        }

        public async Task UpdateAsync(Collection collection)
        {
            var userId = _resolver.GetUserId();
            
            if (userId == null)
                throw new UnauthorizedAccessException();

            await _repository.UpdateAsync(collection, userId, DateTimeOffset.UtcNow);
            await SetSchema(collection.Name, collection.Schema);
        }

        public async Task<bool> ValidateCollectionAsync(string collectionName, JToken data)
        {
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentNullException(nameof(collectionName));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var json = await GetSchema(collectionName);

            try
            {
                var schema = JSchema.Parse(json);
                return data.IsValid(schema);
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> GetSchema(string collectionName)
        {
            var json = await _cache.GetStringAsync(collectionName);

            if (json == null)
            {
                var collection = await FindCollectionAsync(collectionName);

                if (collection == null)
                    throw new ArgumentException($"Collection({collectionName}) is not found");

                json = collection.Schema;

                await SetSchema(collectionName, json);
            }

            return json;
        }

        private async Task SetSchema(string collectionName, string schema)
        {
            await _cache.SetStringAsync(collectionName, schema);
        }
    }
}