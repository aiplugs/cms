using Aiplugs.CMS.Data.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Core.Services
{
    public class JobRegistory : IJobRegistory
    {
        private IDictionary<string, Job> _jobs = new ConcurrentDictionary<string, Job>();
        private IDictionary<string, Action> _cancellers = new ConcurrentDictionary<string, Action>();
        private ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();

        public Job FindJob(string id)
        {
            if (ExistJob(id) == false)
                return null;

            return _jobs[id];
        }

        public void AddJob(Job job)
        {
            if (ExistJob(job.Id) == false)
                _jobs.Add(job.Id, job);
        }

        public void RemoveJob(string id)
        {
            if (ExistJob(id))
                _jobs.Remove(id);
        }

        public bool ExistJob(string id)
        {
            return _jobs.ContainsKey(id);
        }

        public Action FindCanceller(string id)
        {
            if (ExistCanceller(id) == false)
                return null;

            return _cancellers[id];
        }

        public void AddCanceller(string id, Action canceller)
        {
            if (ExistCanceller(id) == false)
                _cancellers.Add(id, canceller);
        }

        public void RemoveCanceller(string id)
        {
            if (ExistCanceller(id))
                _cancellers.Remove(id);
        }

        public bool ExistCanceller(string id)
        {
            return _cancellers.ContainsKey(id);
        }

        public IEnumerable<string> AllCancellers
        {
            get
            {
                return _cancellers.Keys;
            }
        }

        public void Enqueue(string id)
        {
            _queue.Enqueue(id);
        }

        public async Task<Job> DequeueAsync()
        {
            while (true)
            {
                if (_queue.IsEmpty)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    continue;
                }
                string id;
                if (_queue.TryDequeue(out id))
                    return FindJob(id);
            }
        }
    }
}
