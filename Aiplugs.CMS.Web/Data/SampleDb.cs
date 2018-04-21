using System.Data;
using Microsoft.Data.Sqlite;

namespace Aiplugs.CMS.Web.Data
{
    public static class SampleDb
    {
        public static IDbConnection Instance { get; }
        static SampleDb()
        {
            Instance = new SqliteConnection("DataSource=:memory:");
            Instance.Open();
        }

        public static void CloseAndDispose()
        {
            Instance.Close();
            Instance.Dispose();
        }
    }
}