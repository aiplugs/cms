using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Aiplugs.CMS
{
    public class Context
    {
        public ILogger Logger { get; set; }
        public IDataService DataService { get; set; }
        public IStorageService StorageService { get; set; }
        public ISettingsService SettingsService { get; set; }

        public IEnumerable<long> Items { get; set; }
        public IList<Error> Errors { get; set; } = new List<Error>();
        public CancellationToken CancellationToken { get; set; }
        public IProgress<int> Progress { get; set; }

    }
}
