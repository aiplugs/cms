using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Aiplugs.CMS.Core
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAiplugsCMS(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            var scope = scopeFactory.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            
            var roles = new [] { "admin" };

            // var tasks = roles.Select(async role => {
            //     if (await roleManager.RoleExistsAsync(role) == false)
            //         await roleManager.CreateAsync(new Role { Name = role });
            // });

            // Task.WhenAll(tasks).Wait();

            return app;
        }
    }
}