using System.Threading.Tasks;

namespace Aiplugs.CMS {
  public interface IUserManageService
  {
      string GetUserName();
      Task<IUser> GetUserAsync();
  }
}