using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Aiplugs.CMS.Data;
using Aiplugs.CMS.Data.QueryBuilders;
using Aiplugs.CMS.Data.Repositories;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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
        public IDataRepository DataRepository { get; protected set; }
        public IFileRepository FileRepository { get; protected set; }
        public IFolderRepository FolderRepository { get; protected set; }

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
        DbConnection Connection { get; }
        AiplugsDbContext Context { get; }
        public SQLiteTestDb(bool migration = true)
        {
            Connection = new SqliteConnection("DataSource=:memory:");
            Connection.Open();

            var options = new DbContextOptionsBuilder<AiplugsDbContext>()
                    .UseSqlite(Connection)
                    .Options;
            Context = new AiplugsDbContext(options);
            Context.Database.EnsureCreated();

            DataRepository = new DataRepository(Context, new SqliteQueryBuilder());
            FileRepository = new FileRepository(Context);
            FolderRepository = new FolderRepository(Context);
        }
        public override void Dispose()
        {
            Context.Dispose();
            Connection.Close();
            Connection.Dispose();
        }
    }
    public class SqlServerTestDb : TestDbBase
    {
        IDbConnection Instance { get; }

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