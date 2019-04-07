using Aiplugs.CMS.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Data.Repositories
{
    public interface IFolderRepository
    {
        Task<Folder> LookupHomeAsync();

        Task<Folder> LookupAsync(string id);

        Task<IEnumerable<Folder>> GetChildrenAsync(string path, string skipToken, int limit);

        Task<Folder> GetAsync(string path);

        Task<Folder> AddAsync(string path);

        Task UpdateAsync(string id, string path);

        Task RemoveAsync(string id);
    }
}
