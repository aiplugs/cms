using System.Threading.Tasks;
using Aiplugs.Functions.Core;

namespace Aiplugs.CMS.Core.Services
{
    public interface IProcedureService : IProcedureResolver
    {
        Task<long?> RegisterBuiltinProcedureAsync(string collectionName, string procedureName, IContextParameters parameters);  
        Task<long?> RegisterCustomProcedureAsync(string collectionName, ProcedureInfo procedure, IContextParameters parameters);
    }
}