using Aiplugs.Functions.Core;
using Microsoft.AspNetCore.Builder;

namespace Aiplugs.CMS.Core
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAiplugsCMS(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;
            
            app.UseAiplugsFunctions<IContextParameters>();

            return app;
        }
    }
}