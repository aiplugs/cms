using System;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Models;
using Aiplugs.Functions.Core;

namespace Aiplugs.CMS.Core.Services
{
    public class UserManageService : IUserManageService, IUserResolver
    {
        public Task<IUser> GetUserAsync()
        {
            return Task.FromResult<IUser>(new User());
        }

        public string GetUserId()
        {
            return new Guid().ToString();
        }

        public string GetUserName()
        {
            return "admin";
        }
    }
}