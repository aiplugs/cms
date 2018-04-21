using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Aiplugs.CMS.Core.Data.Sqlite
{
    public class FolderRepository : FolderRepositoryBase
    {
        public FolderRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public override async Task<long> AddAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (path.StartsWith("/") == false || path.EndsWith("/") == false)
                throw new ArgumentException(nameof(path));

            return await _db.TransactionalAsync(async tran => {
                return await _db.ExecuteScalarAsync<long>(
                    @"INSERT INTO 
                        Folders (Path) 
                        VALUES (@Path);
                        SELECT last_insert_rowid();", new {
                            Path = path
                        }, transaction:tran);
            });
        }

        public override async Task<IEnumerable<Folder>> GetChildrenAsync(string path, string skipToken, int limit)
        {
            var sql = @"SELECT Id, 
                             Path 
                        FROM Folders
                    WHERE Path LIKE @Path1 AND Path NOT LIKE @Path2 ";
            if (skipToken != null)
                sql += $" AND Path > @SkipToken";
            sql += $" LIMIT @Limit";

            return await _db.NonTransactionalAsync(async() => {
                return await _db.QueryAsync<Folder>(sql, 
                    new { Path1 = $"{path}_%", Path2 = $"{path}%/%/", SkipToken = skipToken, Limit = limit });
            });
        }
    }
}