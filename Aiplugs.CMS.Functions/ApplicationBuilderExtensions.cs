using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aiplugs.CMS.Functions
{
  public static class ApplicationBuilderExtensions
  {
    public static void UseFunctions(this IApplicationBuilder app)
    {
      var services = app.ApplicationServices;
      var lifetime = services.GetRequiredService<IApplicationLifetime>();
      var jobService = services.GetRequiredService<IJobService>();
      var contextFactory = services.GetRequiredService<IContextFactory>();
      var loggerFactory = services.GetRequiredService<ILoggerFactory>();
      var logger = loggerFactory.CreateLogger("Aiplugs.CMS.Functions");

      var runner = new JobRunner(jobService, contextFactory, logger);

      lifetime.ApplicationStopping.Register(() => runner.NotifyStop());
      lifetime.ApplicationStopped.Register(() => runner.Dispose());

      runner.Start();
    }
  }
}