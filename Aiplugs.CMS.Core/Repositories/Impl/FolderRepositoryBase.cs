using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Aiplugs.CMS.Core.Data
{
    public abstract class FolderRepositoryBase : IFolderRepository
    {
        protected readonly IDbConnection _db;
        public FolderRepositoryBase(IDbConnection dbConnection)
        {
            _db = dbConnection;
        }

        public abstract Task<long> AddAsync(string path);

        public abstract Task<IEnumerable<Folder>> GetChildrenAsync(string path, string skipToken, int limit);

        public async Task<Folder> GetAsync(string path)
        {
            return await _db.NonTransactionalAsync(async() => {
                return await _db.QueryFirstOrDefaultAsync<Folder>(
                    @"SELECT Id, 
                             Path 
                        FROM Folders
                    WHERE Path = @Path", new { Path = path });
            });
        }

        public async Task<Folder> LookupAsync(long id)
        {
            return await _db.NonTransactionalAsync(async() => {
                return await _db.QuerySingleOrDefaultAsync<Folder>(
                    @"SELECT Id, 
                             Path 
                        FROM Folders
                    WHERE Id = @Id", new { Id = id });
            });
        }

        public async Task<Folder> LookupHomeAsync()
        {
            return await GetAsync("/");
        }

        public async Task RemoveAsync(long id)
        {
            await _db.TransactionalAsync(async tran => {
                await _db.ExecuteAsync(
                    @"DELETE 
                        FROM Folders
                    WHERE Id = @Id", new { Id = id }, transaction:tran);
            });
        }

        public async Task UpdateAsync(long id, string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (path.StartsWith("/") == false || path.EndsWith("/") == false)
                throw new ArgumentException(nameof(path));
                
            await _db.TransactionalAsync(async tran => {
                await _db.ExecuteAsync(
                    @"UPDATE Folders
                        SET Path = @Path
                    WHERE Id = @Id", new { Id = id, Path = path }, transaction:tran);
            });
        }
    }
}