using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace Aiplugs.CMS.Core.Data.Sqlite
{
    public class Migration : MigrationBase
    {
        public Migration(IDbConnection dbConnection, bool force = false) 
            : base(dbConnection, force)
        {
            Migrations = new IMigration[] 
            {
                new InitialMigration(dbConnection),
            };
        }
        #region Migrations
        internal class InitialMigration : IMigration
        {
            private readonly IDbConnection _db;
            public InitialMigration(IDbConnection dbConnection) 
            {
                _db = dbConnection;
            }
            public void Migrate(IDbTransaction tran)
            {
                _db.Execute(@"CREATE TABLE IF NOT EXISTS 
                            Items (
                                Id               INTEGER PRIMARY KEY,
                                CollectionName   VARCHAR(255) NOT NULL,
                                CreatedAt        DATETIME NOT NULL,
                                CurrentId        INTEGER NULL,
                                IsValid          BOOLEAN NOT NULL
                            )", tran);
                _db.Execute(@"CREATE TABLE IF NOT EXISTS 
                            Records (
                                Id        INTEGER PRIMARY KEY,
                                ItemId    INTEGER NOT NULL,
                                Data      JSON NOT NULL,
                                CreatedAt DATETIME NOT NULL,
                                CreatedBy VARCHAR(64) NOT NULL,
                                FOREIGN KEY(ItemId) REFERENCES Items(Id)
                            )", tran);
                _db.Execute(@"CREATE TABLE IF NOT EXISTS 
                            Folders (
                                Id   INTEGER PRIMARY KEY,
                                Path TEXT UNIQUE
                            )", tran);
                _db.Execute(@"CREATE TABLE IF NOT EXISTS 
                            Files (
                                Id          INTEGER PRIMARY KEY,
                                FolderId    INTEGER NOT NULL,
                                Name        VARCHAR(255) NOT NULL,
                                BinaryPath  TEXT NOT NULL,
                                ContentType VARCHAR(255) NULL,
                                Size        INTEGER NOT NULL,
                                LastModifiedAt   DATETIME NOT NULL,
                                LastModifiedBy   VARCHAR(64) NOT NULL,
                                FOREIGN KEY(FolderId) REFERENCES Folders(Id),
                                UNIQUE (FolderId, Name)
                            )", tran);
                _db.Execute(@"INSERT INTO Folders (Path) VALUES (@Path)", new { Path = "/" });
            }

            public bool NeedMigrate(IDbTransaction tran)
            {
                return true;
            }
        }
        #endregion
    }
}