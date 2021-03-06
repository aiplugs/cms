using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonDiffPatchDotNet;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS
{
    public static class IContextExtensions
    {
        public static async Task Cursor(this IContext context, Func<IItem, Task> action, int batchSize = 100)
        {
            if (context.Parameters.Items.Length > 0)
                await CursorSelections(context.DataService, context.Parameters.Items, action, batchSize);

            else if (context.Parameters.SearchMethod == SearchMethod.Simple)
                await CursorKeyword(context.DataService, context.Parameters.CollectionName, context.Parameters.SearchQuery, action, batchSize);
            
            else if (context.Parameters.SearchMethod == SearchMethod.Advanced)
                await CursorQuery(context.DataService, context.Parameters.CollectionName, context.Parameters.SearchQuery, action, batchSize);
        }
        public static async Task CursorSelections(this IDataService service, string[] selections, Func<IItem, Task> action, int batchSize = 100)
        {
            foreach(var id in selections)
            {
                var item = await service.LookupAsync(id);
                await action(item);
            }
        }
        public static async Task Cursor(this IDataService service, string collection, Func<IItem, Task> action, int batchSize = 100)
        {
            await CursorKeyword(service, collection, null, action, batchSize);
        }
        public static async Task CursorKeyword(this IDataService service, string collection, string keyword, Func<IItem, Task> action, int batchSize = 100)
        {
            IEnumerable<IItem> data = null;
            string skipToken = null;
            do {
                skipToken = data?.LastOrDefault()?.Id;
                data = await service.SearchAsync(collection, keyword, skipToken, batchSize);
                
                foreach(var datum in data)
                    await action(datum);

            } while(data.Count() == batchSize);
        }
        public static async Task CursorQuery(this IDataService service, string collection, string query, Func<IItem, Task> action, int batchSize = 100)
        {
            IEnumerable<IItem> data = null;
            string skipToken = null;
            do {
                skipToken = data?.LastOrDefault()?.Id;
                data = await service.QueryAsync(collection, query, skipToken, batchSize);
                
                foreach(var datum in data)
                    await action(datum);

            } while(data.Count() == batchSize);
        }

        public static async Task<DateTimeOffset> CursorEvents(this IDataService service, string collection, DateTimeOffset from, Func<Event, Task> action, int batchSize = 1000)
        {
            IEnumerable<Event> events = null;
            string skipToken = null;
            DateTimeOffset? end = null;
            DateTimeOffset start = DateTimeOffset.UtcNow;          
            do {
                skipToken = events?.LastOrDefault()?.Id;
                events = await service.GetEventsAsync(collection, from, skipToken, batchSize);
                
                foreach(var ev in events)
                    await action(ev);

                end = events.LastOrDefault()?.TrackAt;
            } while(events.Count() == batchSize);
            return new DateTimeOffset(Math.Max(start.Ticks, end?.ToUniversalTime().Ticks ?? 0), TimeSpan.Zero);
        }

        public static JToken Diff(this IItem item, IRecord record)
        {
            var differ = new JsonDiffPatch();

            return differ.Diff(record.Data, item.Data);
        }
    }
}