using Aiplugs.CMS.Web.ViewModels.StorageViewModels;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Web.Services
{
    public interface IStoragePagingService
    {
        Task<PageViewModel> GetPageAsync(IFolder parent, string contentType, string skipToken = null, int limit = 100, bool dirOnly = false);
    }
}
