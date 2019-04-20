using System;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS;
using Microsoft.Extensions.Logging;

namespace Sample.Procedures
{
    public class LongRunning : IProcedure
    {
        public Task ExecuteAsync(IContext context)
        {
            context.Logger.LogInformation("Start!");

            for (var i = 0; i < 10; i++)
            {
                context.Progress.Report(10 * i);

                context.Logger.LogInformation(string.Join("", Enumerable.Range(0, i).Select(_ => "=")) + ">");
                
                Task.Delay(TimeSpan.FromSeconds(3)).Wait(context.CancellationToken);
            }
            
            context.Logger.LogInformation("Finish!");

            return Task.FromResult(0);
        }
    }
}
