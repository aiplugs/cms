using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Aiplugs.CMS.Data.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Aiplugs.CMS.Core.Services
{
    public class ValidationService : IValidationService
    {
        private readonly ISettingsRepository _settings;
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _factory;
        public ValidationService(ISettingsRepository settingsRepository, IMemoryCache memoryCache, IHttpClientFactory httpClientFactory)
        {
            _settings = settingsRepository;
            _cache = memoryCache;
            _factory = httpClientFactory;
        }
        public async Task<bool> ValidateAsync(Uri schemaUri, JToken data)
        {
            if (_cache.TryGetValue(schemaUri.ToString(), out string json) == false)
            {
                var client = _factory.CreateClient();
                var response = await client.GetAsync(schemaUri);

                if (response.IsSuccessStatusCode == false || response.StatusCode == HttpStatusCode.NotFound)
                    return false;

                json = await response.Content.ReadAsStringAsync();

                _cache.Set(schemaUri.ToString(), json);
            }

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

        public async Task<bool> ValidateAsync(string collectionName, JToken data)
        {
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentNullException(nameof(collectionName));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (_cache.TryGetValue(collectionName, out string json) == false)
            {
                var collection = await _settings.FindCollectionAsync(collectionName);
                if (collection == null)
                    throw new ArgumentException($"Collection({collectionName}) is not found");

                json = collection.Schema;
                
                _cache.Set(collectionName, json, TimeSpan.FromMinutes(3));
            }

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
    }
}