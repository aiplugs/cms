using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Aiplugs.CMS.Core
{
    public abstract class MigrationBase : IMigration
    {
        private readonly IDbConnection _db;
        private readonly bool _force;
        protected IMigration[] Migrations =  new IMigration[0];
        public MigrationBase(IDbConnection dbConnection, bool force = false) 
        {
            _db = dbConnection;
            _force = force;
        }
        public void Migrate(IDbTransaction _ = null)
        {
            _db.Transactional(tran => {
                for(var i = 0; i < Migrations.Length; i++)
                {
                    if (Migrations[i].NeedMigrate(tran) || _force) 
                    {
                        Migrations[i].Migrate(tran);
                    }
                }
            }, IsolationLevel.Serializable);
        }

        public bool NeedMigrate(IDbTransaction _ = null)
        {
            return _db.Transactional(tran => {
                return Migrations.Any(m => m.NeedMigrate(tran));
            }, IsolationLevel.Serializable);
        }
    }
}