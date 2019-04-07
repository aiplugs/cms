using System.Threading.Tasks;

namespace Aiplugs.CMS.Core.Services
{
    public interface IProcedureService
    {
        IProcedure Resolve(string name);
        Task<string> RegisterAsync(string collectionName, string procedureName, IContextParameters parameters);
    }
}