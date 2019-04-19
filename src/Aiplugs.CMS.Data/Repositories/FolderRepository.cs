using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aiplugs.CMS.Data.Repositories
{
    public class FolderRepository : IFolderRepository
    {
        private readonly AiplugsDbContext _db;

        public FolderRepository(AiplugsDbContext db)
        {
            _db = db;
        }

        public async Task<Folder> AddAsync(string path)
        {
            var folder = new Folder
            {
                Id = Helper.CreateKey(),
                Path = path
            };

            var entry = await _db.Folders.AddAsync(folder);

            await _db.SaveChangesAsync();

            entry.State = EntityState.Detached;

            return folder;
        }

        public Task<Folder> GetAsync(string path)
        {
            return _db.Folders.AsNoTracking().Where(folder => folder.Path == path).FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<Folder>> GetChildrenAsync(string path, string skipToken, int limit)
        {
            var query = _db.Folders.AsNoTracking().Where(folder => EF.Functions.Like(folder.Path, $"{path}_%") && !EF.Functions.Like(folder.Path, $"{path}%/%/"));

            if (!string.IsNullOrEmpty(skipToken))
                query = query.Where(folder => string.Compare(folder.Id, skipToken) > 0);

            query = query.OrderBy(folder => folder.Path);

            query = query.Take(limit);

            return await query.ToArrayAsync();
        }

        public Task<Folder> LookupAsync(string id)
        {
            return _db.Folders.AsNoTracking().Where(folder => folder.Id == id).FirstOrDefaultAsync();
        }

        public Task<Folder> LookupHomeAsync() => GetAsync("/");

        public async Task RemoveAsync(Folder folder)
        {
            _db.Folders.Remove(folder);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveAsync(string id)
        {
            var entry = _db.Folders.Attach(new Folder { Id = id });

            entry.State = EntityState.Deleted;

            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(string id, string path)
        {
            var folder = new Folder
            {
                Id = id,
                Path = path,
            };

            var entry = _db.Folders.Attach(folder);
            entry.Property(f => f.Path).IsModified = true;

            await _db.SaveChangesAsync();

            entry.State = EntityState.Detached;
        }
    }
}
