using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Models;
using Aiplugs.CMS.Data.Repositories;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IAppConfiguration _config;
        private readonly ISettingsRepository _repository;
        private readonly IUserResolver _resolver;
        private readonly IValidationService _validator;
        public SettingsService(IAppConfiguration config, ISettingsRepository repository, IUserResolver userResolver, IValidationService validationService)
        {
            _config = config;
            _repository = repository;
            _resolver  = userResolver;
            _validator = validationService;
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
        }

        public async Task<bool> ValidateAsync(Settings settings)
        {
            return await _validator.ValidateAsync(_config.SettingsSchemaUri, JObject.FromObject(settings));
        }

        public async Task<bool> ValidateAsync(Collection collection)
        {
            return await _validator.ValidateAsync(_config.CollectionSchemaUri, JObject.FromObject(collection));
        }
    }
}