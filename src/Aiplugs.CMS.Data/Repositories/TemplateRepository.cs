using Aiplugs.CMS.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Data.Repositories
{
    public class TemplateRepository : ITemplateRepository
    {
        private const string COLLECTION_NAME = "__templates__";
        private readonly IDataRepository _data;
        private object _repository;

        public TemplateRepository(IDataRepository data)
        {
            _data = data;
        }

        public async Task<IEnumerable<Template>> GetAsync()
        {
            var items = await _data.GetAsync(COLLECTION_NAME, null, true, null, int.MaxValue);

            return items.Select(item =>
            {
                var data = JsonConvert.DeserializeObject<Entities.Template>(item.Data);
                return new Template
                {
                    Id = item.Id,
                    Name = data.Name,
                    LastModifiedBy = data.LastModifiedBy,
                    LastModifiedAt = data.LastModifiedAt
                };
            });
        }
        public async Task<Template> FindAsync(string templateName)
        {
            var result = QParser.Parse($"$.Name = {templateName}");
            var query = result.Expression;
            var item = (await _data.QueryAsync(COLLECTION_NAME, query, true, null, int.MaxValue)).SingleOrDefault();
            if (item == null)
                return null;
            var data = JsonConvert.DeserializeObject<Entities.Template>(item.Data);
            return new Template
            {
                Id = item.Id,
                CurrentId = item.CurrentId,
                Name = data.Name,
                Text = data.Text,
                LastModifiedBy = data.LastModifiedBy,
                LastModifiedAt = data.LastModifiedAt
            };
        }
        public async Task<Template> LookupAsync(string templateId)
        {
            var item = await _data.LookupAsync(templateId);
            var data = JsonConvert.DeserializeObject<Entities.Template>(item.Data);
            return new Template
            {
                Id = item.Id,
                CurrentId = item.CurrentId,
                Name = data.Name,
                Text = data.Text,
                LastModifiedBy = data.LastModifiedBy,
                LastModifiedAt = data.LastModifiedAt
            };
        }
        public async Task AddAsync(string templateName, string templateText, string userId, DateTimeOffset createAt)
        {
            var data = new Entities.Template
            {
                Name = templateName,
                Text = templateText,
                LastModifiedBy = userId,
                LastModifiedAt = createAt
            };
            var json = JsonConvert.SerializeObject(data);
            await _data.AddAsync(COLLECTION_NAME, json, userId, DateTimeOffset.Now);
        }
        public async Task Update(string templateId, string currentId, string templateName, string templateText, string userId, DateTimeOffset updateAt)
        {
            var data = new Entities.Template
            {
                Name = templateName,
                Text = templateText,
                LastModifiedBy = userId,
                LastModifiedAt = updateAt
            };
            var json = JsonConvert.SerializeObject(data);
            await _data.UpdateDataAsync(templateId, json, currentId, userId, updateAt);
        }
        public async Task RemoveAsync(string templateId, string cuurentId)
        {
            await _data.RemoveAsync(templateId);
        }
    }
}
