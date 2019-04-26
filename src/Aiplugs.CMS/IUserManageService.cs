using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS {
  public interface IUserManageService
  {
      Task<IEnumerable<IUser>> GetUsersAsync();
      Task<IUser> FindUserAsync(string userId);
      Task RemoveUserAsync(string roleId);
      Task<IEnumerable<IRole>> GetRolesAsync();
      Task<IRole> FindRoleAsync(string roleId);
      Task AddRoleAsync(string roleName);
      Task RemoveRoleAsync(string roleId);
      Task AddToRoleAsync(string userId, string roleName);
  }
}