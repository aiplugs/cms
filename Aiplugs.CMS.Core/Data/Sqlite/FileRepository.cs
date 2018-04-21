using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Aiplugs.CMS.Core.Data.Sqlite
{
    public class FileRepository : FileRepositoryBase
    {
        public FileRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public override async Task<long> AddAsync(long folderId, string name, string binaryPath, string contentType, long size, string lastModifiedBy, DateTime lastModifiedAt)
        {
            return await _db.TransactionalAsync(async tran => {
                return await _db.ExecuteScalarAsync<long>(
                    @"INSERT INTO 
                        Files (FolderId, Name, BinaryPath, ContentType, Size, LastModifiedBy, LastModifiedAt) 
                        VALUES (@FolderId, @Name, @BinaryPath, @ContentType, @Size, @LastModifiedBy, @LastModifiedAt);
                        SELECT last_insert_rowid();", new {
                            FolderId = folderId,
                            Name = name,
                            BinaryPath = binaryPath,
                            ContentType = contentType,
                            Size = size,
                            LastModifiedBy = lastModifiedBy,
                            LastModifiedAt = lastModifiedAt
                        }, transaction:tran);
            });
        }

        public override async Task<IEnumerable<File>> GetChildrenAsync(long folderId, long? skipToken, int limit)
        {
            var sql = @"SELECT
                            Id, 
                            FolderId,
                            Name,
                            BinaryPath,
                            ContentType,
                            Size,
                            LastModifiedAt,
                            LastModifiedBy
                        FROM Files
                    WHERE FolderId = @FolderId ";
            if (skipToken.HasValue)
                sql += " AND Id > @SkipToken";
            sql += " LIMIT @Limit";
            
            return await _db.NonTransactionalAsync(async() => {
                return await _db.QueryAsync<File>(sql
                    , new { FolderId = folderId, SkipToken = skipToken, Limit = limit });
            });
        }
    }
}