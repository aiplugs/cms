using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aiplugs.CMS.Data.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly AiplugsDbContext _db;

        public FileRepository(AiplugsDbContext db)
        {
            _db = db;
        }

        public Task<File> LookupAsync(string id)
        {
            return _db.Files.AsNoTracking().Where(file => file.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<File>> GetChildrenAsync(string folderId, string skipToken, int limit)
        {
            var query = _db.Files.AsNoTracking().Where(file => file.FolderId == folderId);

            if (!string.IsNullOrEmpty(skipToken))
                query = query.Where(file => string.Compare(file.Id, skipToken) < 0);

            query = query.OrderBy(file => file.Name);

            query = query.Take(limit);

            return await query.ToArrayAsync();
        }

        public Task<File> FindChildAsync(string folderId, string name)
        {
            return _db.Files.AsNoTracking().Where(file => file.FolderId == folderId && file.Name == name).FirstOrDefaultAsync();
        }

        public async Task<File> AddAsync(string folderId, string name, string binaryPath, string contentType, long size, string userId, DateTimeOffset datetime)
        {
            var file = new File
            {
                Id = Helper.CreateKey(),
                FolderId = folderId,
                Name = name,
                BinaryPath = binaryPath,
                ContentType = contentType,
                Size = size,
                LastModifiedAt = datetime,
                LastModifiedBy = userId
            };

            await _db.Files.AddAsync(file);
            await _db.SaveChangesAsync();

            _db.Entry(file).State = EntityState.Detached;

            return file;
        }

        public async Task UpdateMetaAsync(string id, string contentType, long size, string userId, DateTimeOffset datetime)
        {
            var file = new File
            {
                Id = id,
                ContentType = contentType,
                Size = size,
                LastModifiedAt = datetime,
                LastModifiedBy = userId
            };

            var entry = _db.Files.Attach(file);
            entry.Property(f => f.ContentType).IsModified = true;
            entry.Property(f => f.Size).IsModified = true;
            entry.Property(f => f.LastModifiedAt).IsModified = true;
            entry.Property(f => f.LastModifiedBy).IsModified = true;

            await _db.SaveChangesAsync();

            entry.State = EntityState.Detached;
        }

        public async Task UpdatePathAsync(string id, string folderId, string name, string userId, DateTimeOffset datetime)
        {
            var file = new File
            {
                Id = id,
                FolderId = folderId,
                Name = name,
                LastModifiedAt = datetime,
                LastModifiedBy = userId
            };

            var entry = _db.Files.Attach(file);
            entry.Property(f => f.FolderId).IsModified = true;
            entry.Property(f => f.Name).IsModified = true;
            entry.Property(f => f.LastModifiedAt).IsModified = true;
            entry.Property(f => f.LastModifiedBy).IsModified = true;

            await _db.SaveChangesAsync();

            entry.State = EntityState.Detached;
        }

        public async Task RemoveAsync(string id)
        {
            var entry = _db.Files.Attach(new File { Id = id });

            entry.State = EntityState.Deleted;

            await _db.SaveChangesAsync();
        }
    }
}
