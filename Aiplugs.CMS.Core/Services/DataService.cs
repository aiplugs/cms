using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Data.Entities;
using Aiplugs.CMS.Data.Repositories;
using Aiplugs.CMS.Query;
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

        public async Task<string> AddAsync(string collectionName, JToken data)
        {
            var userId = _resolver.GetUserId();

            if (userId == null)
                throw new UnauthorizedAccessException();
            
            var item = await _repository.AddAsync(collectionName, data.ToString(), userId, DateTimeOffset.UtcNow);

            return item.Id;
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(string collectionName, DateTimeOffset from, string skipToken = null, int limit = 100)
        {
            return await _repository.GetEventsAsync(collectionName, from, skipToken, limit);
        }
        private DatumRecord Map(ItemRecord record) => new DatumRecord
        {
            Id = record.Id,
            Data = JToken.Parse(record.Data),
            CreatedAt = record.CreatedAt,
            CreatedBy = record.CreatedBy
        };

        public async Task<IEnumerable<IRecord>> GetHistoryAsync(string id, string skipToken = null, int limit = 100)
        {
            return (await _repository.GetHistoryAsync(id, skipToken, limit)).Select(Map).ToArray();
        }

        public async Task<IRecord> GetRecordThenAsync(string id, DateTime then)
        {
            var record = await _repository.GetRecordThenAsync(id, then);
            return Map(record);
        }

        private Datum Map(Item item) => new Datum
        {
            Id = item.Id,
            Data = JToken.Parse(item.Data),
            IsValid = item.IsValid,
            CreatedAt = item.CreatedAt,
            CreatedBy = item.CreatedBy,
            UpdatedAt = item.UpdatedAt,
            UpdatedBy = item.UpdatedBy,
            CurrentId = item.CurrentId
        };
        public async Task<IItem> LookupAsync(string id)
        {
            var item = await _repository.LookupAsync(id);
            return Map(item);
        }

        public async Task<IEnumerable<IItem>> SearchAsync(string collectionName, string keyword = null, string skipToken = null, int limit = 100, bool desc = true)
        {
            return (await _repository.GetAsync(collectionName, keyword, desc, skipToken, limit))
                .Select(Map)
                .ToArray();
        }

        public async Task<IEnumerable<IItem>> QueryAsync(string collectionName, string query, string skipToken = null, int limit = 100, bool desc = true)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException(nameof(query));
            
            var result = QParser.Parse(query);
            
            if (result.Success == false)
                throw new ArgumentException(nameof(query), $"Invalid query: {result.ErrorMessage}");
            
            return (await _repository.QueryAsync(collectionName, result.Expression, desc, skipToken, limit))
                .Select(Map)
                .ToArray();
        }

        public async Task UpdateAsync(string id, JToken data, string currentId)
        {
            var userId = _resolver.GetUserId();

            if (userId == null)
                throw new UnauthorizedAccessException();

            await _repository.UpdateDataAsync(id, data.ToString(), currentId, userId, DateTimeOffset.UtcNow);
        }

        public async Task SetStatusAsync(string id, bool isValid, string currentId)
        {
            await _repository.UpdateStatusAsync(id, isValid, currentId);
        }

        public async Task<bool> ValidateAsync(string collectionName, JToken data)
        {
            return await _validator.ValidateAsync(collectionName, data);
        }
    }

    public class DatumRecord : IRecord
    {
        public string Id { get; set; }

        public JToken Data { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string CreatedBy { get; set; }
    }

    public class Datum : IItem
    {
        public string Id { get; set; }
        public JToken Data { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsValid { get; set; }
        public string CurrentId { get; set; }
    }
}