using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Aiplugs.CMS.Core.Data
{
    public abstract class FileRepositoryBase : IFileRepository
    {
        protected readonly IDbConnection _db;
        public FileRepositoryBase(IDbConnection dbConnection)
        {
            _db = dbConnection;
        }

        public abstract Task<long> AddAsync(long folderId, string name, string binaryPath, string  contentType, long size, string lastModifiedBy, DateTime lastModifiedAt);

        public async Task<File> LookupAsync(long id)
        {
            return await _db.NonTransactionalAsync(async() => {
                return await _db.QuerySingleOrDefaultAsync<File>(
                    @"SELECT
                            Id, 
                            FolderId,
                            Name,
                            BinaryPath,
                            ContentType,
                            Size,
                            LastModifiedAt,
                            LastModifiedBy
                        FROM Files
                    WHERE Id = @Id", new { Id = id });
            });
        }

        public abstract Task<IEnumerable<File>> GetChildrenAsync(long folderId, long? skipToken, int limit);

        public async Task<File> FindChildAsync(long folderId, string name)
        {
            return await _db.NonTransactionalAsync(async() => {
                return await _db.QueryFirstOrDefaultAsync<File>(
                    @"SELECT
                            Id, 
                            FolderId,
                            Name,
                            BinaryPath,
                            ContentType,
                            Size,
                            LastModifiedAt,
                            LastModifiedBy
                        FROM Files
                    WHERE FolderId = @FolderId AND Name = @Name", new { FolderId = folderId, Name = name });
            });
        }

        public async Task RemoveAsync(long id)
        {
            await _db.TransactionalAsync(async tran => {
                await _db.ExecuteAsync(
                    @"DELETE 
                        FROM Files 
                      WHERE Id = @Id", new { Id = id }, transaction:tran);
            });
        }

        public async Task UpdateAsync(long id, long folderId, string name, string  contentType, long size, string lastModifiedBy, DateTime lastModifiedAt)
        {
            await _db.TransactionalAsync(async tran => {
                await _db.ExecuteAsync(
                    @"UPDATE Files
                        SET FolderId = @FolderId,
                            Name = @Name,
                            ContentType = @ContentType,
                            Size = @Size,
                            LastModifiedAt = @LastModifiedAt,
                            LastModifiedBy = @LastModifiedBy
                      WHERE Id = @Id", new {
                            Id = id,
                            FolderId = folderId,
                            Name = name,
                            ContentType = contentType,
                            Size = size,
                            LastModifiedBy = lastModifiedBy,
                            LastModifiedAt = lastModifiedAt
                      }, transaction:tran);
            });
        }
    }
}