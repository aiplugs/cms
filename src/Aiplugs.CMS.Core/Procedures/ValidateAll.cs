using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Core.Functions
{
    public class ValidateAll : IProcedure
    {
        public async Task ExecuteAsync(IContext context)
        {
            context.Logger.LogInformation("Start validation.");
            var collection = context.Parameters.CollectionName;
            await context.Cursor(async item => {
                var isValid = await context.DataService.ValidateAsync(collection, item.Data);

                if (item.IsValid != isValid)
                {
                    await context.DataService.SetStatusAsync(item.Id, isValid, item.CurrentId);
                    context.Logger.LogInformation($"{item.Id} has changed.");
                }
            });
            context.Logger.LogInformation("Finish validation.");
        }
    }
}