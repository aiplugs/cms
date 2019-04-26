using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Models;
using Aiplugs.CMS.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityUser = Aiplugs.CMS.Data.Entities.User;
using IdentityRole = Aiplugs.CMS.Data.Entities.Role;

namespace Aiplugs.CMS.Core.Services
{
    public class UserManageService : IUserManageService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AiplugsDbContext _db;
        public UserManageService(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AiplugsDbContext db
        ) {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public async Task AddRoleAsync(string roleName)
        {
            await _roleManager.CreateAsync(new Data.Entities.Role { Name = roleName });
        }

        public async Task AddToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IRole> FindRoleAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
                return null;

            return new Role
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public async Task<IUser> FindUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            return new User
            {
                Id = user.Id,
                Name = user.UserName,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }

        public async Task<IEnumerable<IRole>> GetRolesAsync()
        {
            var roles = await _db.Roles.ToArrayAsync();
            return roles.Select(role => new Role
            {
                Id = role.Id,
                Name = role.Name
            });
        }

        public async Task<IEnumerable<IUser>> GetUsersAsync()
        {
            var result = new List<User>();
            var users = await _db.Users.ToArrayAsync();

            foreach (var user in users)
            {
                result.Add(new User
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }
            
            return result;
        }

        public async Task RemoveUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new ArgumentOutOfRangeException(nameof(userId));

            using (var tran = await _db.Database.BeginTransactionAsync())
            {
                if ((await _userManager.GetUsersInRoleAsync("admin")).Count() == 1)
                    throw new InvalidOperationException("Cannot delete user: need an admin user at least.");
                
                await _userManager.DeleteAsync(user);
                
                tran.Commit();
            }
        }

        public async Task RemoveRoleAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
                throw new ArgumentOutOfRangeException(nameof(roleId));
            
            using (var tran = await _db.Database.BeginTransactionAsync())
            {
                if ((await _userManager.GetUsersInRoleAsync(role.Name)).Any())
                    throw new InvalidOperationException($"Cannot delete role:'{role.Name}' role has any user.");
                
                await _roleManager.DeleteAsync(role);

                tran.Commit();            
            }
        }
    }
}