using System;
using System.Data;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Polly;

namespace Aiplugs.CMS.Core
{
    public static class IDbConnectionExtensions
    {
        public static async Task TransactionalAsync(this IDbConnection db, Func<IDbTransaction, Task> action, IsolationLevel level = IsolationLevel.ReadCommitted, int tryMax = 3)
        {
            await Policy.Handle<Exception>().RetryAsync(tryMax).ExecuteAsync(async () => {
                using(var tran = db.BeginTransaction(level))
                {
                    try {
                        await action(tran);
                        tran.Commit();                   
                    }
                    catch(Exception ex)
                    {
                        tran.Rollback();
                        ExceptionDispatchInfo.Capture(ex).Throw();
                    }
                }
            });
        }
        public static void Transactional(this IDbConnection db, Action<IDbTransaction> action, IsolationLevel level = IsolationLevel.ReadCommitted, int tryMax = 3)
        {
            Policy.Handle<Exception>().Retry(tryMax).Execute(() => {
                using(var tran = db.BeginTransaction(level))
                {
                    try {
                        action(tran);
                        tran.Commit();
                    }
                    catch(Exception ex)
                    {
                        tran.Rollback();
                        ExceptionDispatchInfo.Capture(ex).Throw();
                    }
                }
            });
        }
        public static async Task<T> TransactionalAsync<T>(this IDbConnection db, Func<IDbTransaction, Task<T>> action, IsolationLevel level = IsolationLevel.ReadCommitted, int tryMax = 3)
        {
            T result = default(T);
            await Policy.Handle<Exception>().RetryAsync(tryMax).ExecuteAsync(async () => {
                using(var tran = db.BeginTransaction(level))
                {
                    try {
                        result = await action(tran);
                        tran.Commit();                   
                    }
                    catch(Exception ex)
                    {
                        tran.Rollback();
                        ExceptionDispatchInfo.Capture(ex).Throw();
                    }
                }
            });
            return result;
        }
        public static T Transactional<T>(this IDbConnection db, Func<IDbTransaction, T> action, IsolationLevel level = IsolationLevel.ReadCommitted, int tryMax = 3)
        {
            T result = default(T);
            Policy.Handle<Exception>().Retry(tryMax).Execute(() => {
                using(var tran = db.BeginTransaction(level))
                {
                    try {
                        result = action(tran);
                        tran.Commit();
                        return;                
                    }
                    catch(Exception ex)
                    {
                        tran.Rollback();
                        ExceptionDispatchInfo.Capture(ex).Throw();
                    }
                }
            });
            return result;
        }
        public static async Task<T> NonTransactionalAsync<T>(this IDbConnection db, Func<Task<T>> action, int tryMax = 3)
        {
            T result = default(T);
            await Policy.Handle<Exception>().RetryAsync(tryMax).ExecuteAsync(async () => {
                result = await action();
            });
            return result;
        }
        public static T NonTransactional<T>(this IDbConnection db, Func<T> action, int tryMax = 3)
        {
            T result = default(T);
            Policy.Handle<Exception>().Retry(tryMax).Execute(() => {
                result = action();
            });
            return result;
        }
    }
}