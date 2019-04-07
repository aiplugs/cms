using Aiplugs.CMS.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Core.Services
{
    public interface IJobRegistory
    {
        Job FindJob(string id);
        void AddJob(Job job);
        void RemoveJob(string id);
        bool ExistJob(string id);


        Action FindCanceller(string id);
        void AddCanceller(string id, Action canceller);
        void RemoveCanceller(string id);
        bool ExistCanceller(string id);
        IEnumerable<string> AllCancellers { get; }

        void Enqueue(string id);
        Task<Job> DequeueAsync();
    }
}