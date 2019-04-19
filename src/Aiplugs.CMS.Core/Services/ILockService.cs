using System.Threading.Tasks;

namespace Aiplugs.CMS.Core.Services
{
    public interface ILockService
    {
        Task<bool> LockAsync(string name);
        Task UnlockAsync(string name);
    }
}
