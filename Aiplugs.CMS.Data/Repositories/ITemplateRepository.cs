using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Data.Repositories
{
    public interface ITemplateRepository
    {
        Task<IEnumerable<Template>> GetAsync();
        Task<Template> FindAsync(string templateName);
        Task<Template> LookupAsync(string templateId);
        Task AddAsync(string templateName, string templateText, string userId, DateTimeOffset createAt);
        Task Update(string templateId, string currentId, string templateName, string templateText, string userId, DateTimeOffset updateAt);
        Task RemoveAsync(string templateId, string cuurentId);
    }
}
