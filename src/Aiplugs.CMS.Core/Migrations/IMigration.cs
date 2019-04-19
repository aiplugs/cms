using System.Data;

namespace Aiplugs.CMS.Core
{
    public interface IMigration
    {
        bool NeedMigrate(IDbTransaction tran = null);
        void Migrate(IDbTransaction tran = null);
    }
}