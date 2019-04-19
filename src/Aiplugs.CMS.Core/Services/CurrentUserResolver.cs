using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Aiplugs.CMS.Core.Services
{
    public class CurrentUserResolver : IUserResolver
    {
        private readonly IHttpContextAccessor _context;
        public CurrentUserResolver(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetUserId()
        {
            return _context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}