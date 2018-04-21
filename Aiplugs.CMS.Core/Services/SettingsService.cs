using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Data;
using Aiplugs.CMS.Core.Models;
using Aiplugs.Functions.Core;
using Microsoft.Extensions.Configuration;
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
        public async Task<long> AddAsync(Collection collection)
        {
            var userId = _resolver.GetUserId();
            
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            return await _repository.AddAsync(collection, userId);
        }

        public async Task<Collection> LookupCollectionAsync(long id)
        {
            return await _repository.LookupCollectionAsync(id);
        }

        public async Task<Collection> FindCollectionAsync(string collectionName)
        {
            return await _repository.FindCollectionAsync(collectionName);
        }

        public Task<IEnumerable<Collection>> GetCollectionHistoryAsync(long id)
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
                throw new ArgumentNullException(nameof(userId));

            await _repository.UpdateAsync(settings, userId);
        }

        public async Task UpdateAsync(Collection collection)
        {
            var userId = _resolver.GetUserId();
            
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            await _repository.UpdateAsync(collection, userId);
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