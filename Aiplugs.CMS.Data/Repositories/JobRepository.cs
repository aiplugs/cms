using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aiplugs.CMS.Data.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly AiplugsDbContext _db;

        public JobRepository(AiplugsDbContext db)
        {
            _db = db;
        }

        public async Task<Job> AddAsync(Job job)
        {
            job.Id = Helper.CreateKey();
            var entry = await _db.Jobs.AddAsync(job);
            await _db.SaveChangesAsync();
            return entry.Entity;
        }

        public Task<Job> FindAsync(string id)
        {
            return _db.Jobs.FindAsync(id);
        }

        public async Task<IEnumerable<Job>> GetAsync(string name, bool desc = true, string skipToken = null, int limit = 10)
        {
            var query = _db.Jobs.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(job => job.Name == name);

            if (!string.IsNullOrEmpty(skipToken))
            {
                if (desc)
                    query = query.Where(job => string.Compare(job.Id, skipToken) > 0);
                else
                    query = query.Where(job => string.Compare(job.Id, skipToken) < 1);
            }

            if (desc)
                query = query.OrderByDescending(job => job.CreatedAt);
            else
                query = query.OrderBy(job => job.CreatedAt);

            return await query.Take(limit).ToArrayAsync();
        }

        public async Task UpdateAsync(Job job)
        {
            var entry =_db.Entry(job);
            entry.State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
