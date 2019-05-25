using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Aiplugs.CMS
{
    public interface IContext
    {
        ILogger Logger { get; }
        IList<Error> Errors { get; }
        CancellationToken CancellationToken { get; }
        IProgress<int> Progress { get; }
        IContextParameters Parameters { get; }
        IDataService DataService { get; }
        IStorageService StorageService { get; }
        ISettingsService SettingsService { get; }
        IUserManageService UserManageService { get; }
    }
}
