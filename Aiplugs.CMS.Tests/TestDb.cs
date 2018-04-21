using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Data;
using Aiplugs.Functions.Core;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Aiplugs.CMS.Tests
{
    public class TestDb : IDisposable
    {
        public TestDbBase[] DBs { get; }

        public TestDb(bool migration = true)
        {
            var dbs = new List<TestDbBase>{ new SQLiteTestDb(migration) };
            // var sqlsvr = Environment.GetEnvironmentVariable("AIPLUGS_CMS_TESTS_TESTDB");
            // if (string.IsNullOrEmpty(sqlsvr) == false)
            //     dbs.Add(new SqlServerTestDb(sqlsvr, migration));

            DBs = dbs.ToArray();
        }
        
        public void Dispose()
        {
            foreach(var db in DBs)
            {
                db.Dispose();
            }
        }
    }
    public abstract class TestDbBase : IDisposable
    {
        IDbConnection Instance { get; }
        public virtual IDataRepository DataRepository { get; }
        public virtual IFileRepository FileRepository { get; }
        public virtual IFolderRepository FolderRepository { get; }

        public TRepository Resolve<TRepository>()
        {
            if (typeof(TRepository) == typeof(IDataRepository))
                return (TRepository)DataRepository;
            
            if (typeof(TRepository) == typeof(IFileRepository))
                return (TRepository)FileRepository;
            
            if (typeof(TRepository) == typeof(IFolderRepository))
                return (TRepository)FolderRepository;
            
            throw new NotSupportedException();
        }
        public abstract void Dispose();
    }
    public class SQLiteTestDb : TestDbBase
    {
        public IDbConnection Instance { get; }
        public override IDataRepository DataRepository { get { return new Core.Data.Sqlite.DataRepository(Instance, new Core.Data.Sqlite.QueryBuilder()); } }
        public override IFileRepository FileRepository { get { return new Core.Data.Sqlite.FileRepository(Instance); } }
        public override IFolderRepository FolderRepository { get { return new Core.Data.Sqlite.FolderRepository(Instance); } }
        public SQLiteTestDb(bool migration = true)
        {
            Instance = new SqliteConnection("DataSource=:memory:");

            Instance.Open();

            if (migration)
                new Core.Data.Sqlite.Migration(Instance, true).Migrate();
        }
        public override void Dispose()
        {
            Instance.Close();
            Instance.Dispose();
        }
    }
    public class SqlServerTestDb : TestDbBase
    {
        public IDbConnection Instance { get; }

        public IDataRepository DataRepository { get { return new Core.Data.Sqlite.DataRepository(Instance, new Core.Data.Sqlite.QueryBuilder()); } }
        public IFileRepository FileRepository { get { return new Core.Data.Sqlite.FileRepository(Instance); } }
        public IFolderRepository FolderRepository { get { return new Core.Data.Sqlite.FolderRepository(Instance); } }

        public SqlServerTestDb(string connectionString, bool migration = true)
        {
            Instance = new SqlConnection(connectionString);

            Instance.Open();
            
            if (migration) 
            {
                Cleanup();
                //new Core.Data.SqlServer.Migration(Instance, false).Migrate();
            }
        }

        public override void Dispose()
        {
            Instance.Close();
            Instance.Dispose();
        }

        public void Cleanup()
        {
            Instance.Execute(@"DROP TABLE IF EXISTS Records;");
            Instance.Execute(@"DROP TABLE IF EXISTS Items;");
            Instance.Execute(@"DROP TABLE IF EXISTS Files;");
            Instance.Execute(@"DROP TABLE IF EXISTS Folders;");
        }
    }
}