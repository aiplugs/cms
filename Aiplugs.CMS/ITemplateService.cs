using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS
{
    public interface ITemplateService
    {
        Task<IEnumerable<Template>> GetAsync();
        Task<Template> FindAsync(string name);
        Task<Template> LookupAsync(string id);
        Task AddAsync(string name, string template);
        Task UpdateAsync(string id, string currentId, string name, string template);
        Task RemoveAsync(string id, string currentId);
        Task<string> RenderAsync<TModel>(string templateName, TModel token);
    }
}
