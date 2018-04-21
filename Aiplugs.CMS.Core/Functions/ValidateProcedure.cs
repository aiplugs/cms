using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Core.Functions
{
    public class ValidateProcedure : IProcedure
    {
        public async Task Execute(IContext context)
        {
            var collection = context.Parameters.CollectionName;
            await context.Cursor(async item => {
                var isValid = await context.DataService.ValidateAsync(collection, item.Data);

                if (item.IsValid != isValid)
                    await context.DataService.SetStatusAsync(item.Id, isValid);
            });
        }
    }
}