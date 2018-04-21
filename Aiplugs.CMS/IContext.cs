using System;
using System.Collections.Generic;
using System.Threading;
using Aiplugs.Functions;

namespace Aiplugs.CMS
{
    public interface IContext : IContext<IContextParameters>
    {
        IDataService DataService { get; }
        IStorageService StorageService { get; }
        ISettingsService SettingsService { get; }
    }
}
