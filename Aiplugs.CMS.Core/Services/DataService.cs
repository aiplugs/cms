using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Data;
using Aiplugs.CMS.Core.Query;
using Aiplugs.Functions.Core;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Core.Services
{
    public class DataService : IDataService
    {
        private readonly IDataRepository _repository;
        private readonly IUserResolver _resolver;
        private readonly IValidationService _validator;
        public DataService(IDataRepository repository, IUserResolver userResolver, IValidationService validationService)
        {
            _repository = repository;
            _resolver = userResolver;
            _validator = validationService;
        }

        public async Task<long> AddAsync(string collectionName, JObject data)
        {
            var userId = _resolver.GetUserId();

            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            return await _repository.AddAsync(collectionName, data, userId);
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(string collectionName, DateTime from, long? skipToken = null, int limit = 100)
        {
            return await _repository.GetEventsAsync(collectionName, from, skipToken, limit);
        }

        public async Task<IEnumerable<IRecord>> GetHistoryAsync(long id, long? skipToken = null, int limit = 100)
        {
            return await _repository.GetHistoryAsync(id, skipToken, limit);
        }

        public async Task<IRecord> GetRecordThenAsync(long id, DateTime then)
        {
            return await _repository.GetRecordThenAsync(id, then);
        }

        public async Task<IItem> LookupAsync(long id)
        {
            return await _repository.LookupAsync(id);
        }

        public Task<IEnumerable<IItem>> SearchAsync(string collectionName, string keyword = null, long? skipToken = null, int limit = 100, bool desc = true)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IItem>> QueryAsync(string collectionName, string query, long? skipToken = null, int limit = 100, bool desc = true)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException(nameof(query));
            
            var result = QParser.Parse(query);
            
            if (result.Success == false)
                throw new ArgumentException(nameof(query), $"Invalid query: {result.ErrorMessage}");
            
            return await _repository.QueryAsync(collectionName, result.Expression, desc, skipToken, limit);
        }

        public async Task UpdateAsync(long id, JObject data)
        {
            var userId = _resolver.GetUserId();

            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            
            await _repository.UpdateAsync(id, data, userId);
        }

        public async Task SetStatusAsync(long id, bool isValid)
        {
            await _repository.SetStatusAsync(id, isValid);
        }

        public async Task<bool> ValidateAsync(string collectionName, JObject data)
        {
            return await _validator.ValidateAsync(collectionName, data);
        }
    }
}