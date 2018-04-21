using System.Threading.Tasks;

namespace Aiplugs.CMS {
  public interface IUserManageService
  {
      string GetUserId();
      string GetUserName();
      Task<IUser> GetUserAsync();
  }
}