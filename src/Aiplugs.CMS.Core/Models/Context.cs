using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Aiplugs.CMS.Core.Models
{
    public class Context : IContext
    {
        public ILogger Logger { get; set; }

        public IList<Error> Errors { get; set; } = new List<Error>();

        public CancellationToken CancellationToken { get; set; }

        public IProgress<int> Progress { get; set; }

        public IContextParameters Parameters { get; set; }

        public IDataService DataService { get; set; }

        public IStorageService StorageService { get; set; }

        public ISettingsService SettingsService { get; set; }
    }
}