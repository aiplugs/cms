using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Core.Data
{
    public interface IFolderRepository
    {
        Task<Folder> LookupHomeAsync();
        Task<Folder> LookupAsync(long id);
        Task<IEnumerable<Folder>> GetChildrenAsync(string path, string skipToken, int limit);
        Task<Folder> GetAsync(string path);
        Task<long> AddAsync(string path);
        Task UpdateAsync(long id, string path);
        Task RemoveAsync(long id);
    }
}