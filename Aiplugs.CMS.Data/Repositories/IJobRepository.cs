using Aiplugs.CMS.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Data.Repositories
{
    public interface IJobRepository
    {
        Task<Job> FindAsync(string id);

        Task<IEnumerable<Job>> GetAsync(string name, bool desc = true, string skipToken = null, int limit = 10);

        Task<Job> AddAsync(Job job);

        Task UpdateAsync(Job job);
    }
}
