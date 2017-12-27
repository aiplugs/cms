using System.Threading.Tasks;
using Aiplugs.CMS.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Aiplugs.CMS.Web.Services {
  public class UserManageService : IUserManageService
  {
    private readonly IHttpContextAccessor ctx;
    private readonly UserManager<ApplicationUser> userManager;
    public UserManageService(IHttpContextAccessor context,  UserManager<ApplicationUser> manager)
    {
        ctx = context;
        userManager = manager;
    }
    public string GetUserName()
    {
      return ctx.HttpContext.User?.Identity?.Name;
    }
    public async Task<IUser> GetUserAsync()
    {
      return await userManager.FindByNameAsync(GetUserName());
    }
  }
}