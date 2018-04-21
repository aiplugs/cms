using Aiplugs.Functions.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Aiplugs.CMS.Core
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAiplugsCMS(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;
            
            var migration = services.GetRequiredService<IMigration>();

            if (migration.NeedMigrate())
                migration.Migrate();

            app.UseAiplugsFunctions<IContextParameters>();

            return app;
        }
    }
}