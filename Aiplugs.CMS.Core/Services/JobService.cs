using Aiplugs.CMS.Data.Entities;
using Aiplugs.CMS.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Core.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IJobRegistory _jobRegistory;
        private readonly ILockService _lockService;
        private readonly IUserResolver _userResolver;

        public JobService(IJobRepository jobRepository, IJobRegistory jobRegistory, ILockService lockService, IUserResolver userResolver)
        {
            _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
            _jobRegistory = jobRegistory ?? throw new ArgumentNullException(nameof(jobRegistory));
            _lockService = lockService ?? throw new ArgumentNullException(nameof(lockService));
            _userResolver = userResolver ?? throw new ArgumentNullException(nameof(userResolver));
        }

        public void RegisterCanceller(string id, Action canceller)
        {
            _jobRegistory.AddCanceller(id, canceller);
        }
        public void UnregisterCanceller(string id)
        {
            _jobRegistory.RemoveCanceller(id);
        }
        public void Cancel(string id)
        {
            var canceller = _jobRegistory.FindCanceller(id);
            if (canceller != null)
            {
                canceller();
                _jobRegistory.RemoveCanceller(id);
            }
        }
        public void CancelAll()
        {
            foreach (var id in _jobRegistory.AllCancellers)
            {
                _jobRegistory.FindCanceller(id)();
                _jobRegistory.RemoveCanceller(id);
            }
        }
        public async Task<Job> DequeueAsync()
        {
            return await _jobRegistory.DequeueAsync();
        }

        public async Task<string> ExclusiveCreateAsync(string name, IContextParameters @params)
        {
            var userId = _userResolver.GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            if (await _lockService.LockAsync(name) == false)
            {
                await _lockService.UnlockAsync(name);
                //return null;
            }

            var job = await _jobRepository.AddAsync(new Job(name, userId, @params));
            
            _jobRegistory.AddJob(job);
            _jobRegistory.Enqueue(job.Id);

            return job.Id;
        }

        public async Task<IEnumerable<Job>> GetAsync(string name, bool desc = true, string skipToken = null, int limit = 10)
        {
            return await _jobRepository.GetAsync(name, desc, skipToken, limit);
        }
        public async Task<Job> FindAsync(string id)
        {
            var job = _jobRegistory.FindJob(id);
            if (job != null)
                return job;

            return await _jobRepository.FindAsync(id);
        }

        public async Task SaveAsync(Job job)
        {
            await _jobRepository.UpdateAsync(job);
            if (job.FinishAt.HasValue)
            {
                _jobRegistory.RemoveJob(job.Id);
                UnregisterCanceller(job.Id);
                await _lockService.UnlockAsync(job.Name);
            }
        }
    }
}
