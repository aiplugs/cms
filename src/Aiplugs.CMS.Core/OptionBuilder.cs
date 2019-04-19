using System;

namespace Aiplugs.CMS.Core
{
    public class OptionsBuilder
    {
        private bool _forceMigration = false;
        //public Func<IDbConnection, IMigration> MigrationFactory { get; private set; }
        //public Func<IDbConnection, IDataRepository> DataRepositoryFactory { get; private set; }
        //public Func<IDbConnection, IFileRepository> FileRepositoryFactory { get; private set; }
        //public Func<IDbConnection, IFolderRepository> FolderRepositoryFactory { get; private set; }
        public OptionsBuilder()
        {
            UseSqlite();
        }
        public OptionsBuilder UseSqlite()
        {
            //MigrationFactory = db => new Data.Sqlite.Migration(db, _forceMigration);
            //DataRepositoryFactory = db => new Data.Sqlite.DataRepository(db, new SqliteQueryBuilder());
            //FileRepositoryFactory = db => new Data.Sqlite.FileRepository(db);
            //FolderRepositoryFactory = db => new Data.Sqlite.FolderRepository(db);
            

            return this;
        }
        public OptionsBuilder UseSqlServer()
        {
            throw new NotImplementedException();

            // Dapper.SqlMapper.AddTypeMap(typeof(System.DateTime), System.Data.DbType.DateTime2);
            // Dapper.SqlMapper.AddTypeMap(typeof(System.DateTime?), System.Data.DbType.DateTime2);
            // MigrationFactory = db => new Data.Sqlite.Migration(db, _forceMigration);
            // DataRepositoryFactory = db => new Data.Sqlite.DataRepository(db, new Data.Sqlite.QueryBuilder());
            // FileRepositoryFactory = db => new Data.Sqlite.FileRepository(db);
            // FolderRepositoryFactory = db => new Data.Sqlite.FolderRepository(db);

            // FunctionOptionsBuilder.UseSqlServer();

            // return this;
        }
        public OptionsBuilder ForceMigration()
        {
            _forceMigration = true;

            return this;
        }
    }
}