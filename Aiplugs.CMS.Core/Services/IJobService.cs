using Aiplugs.CMS.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Core.Services
{
    public interface IJobService
    {
        Task<string> ExclusiveCreateAsync(string name, IContextParameters parameters);
        Task<IEnumerable<Job>> GetAsync(string name, bool desc = true, string skipToken = null, int limit = 10);
        Task<Job> FindAsync(string id);
        void Cancel(string id);
        void CancelAll();
        Task<Job> DequeueAsync();
        Task SaveAsync(Job job);
        void RegisterCanceller(string id, Action canceller);
        void UnregisterCanceller(string id);
    }
}
