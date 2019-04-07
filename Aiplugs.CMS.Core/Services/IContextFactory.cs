using Aiplugs.CMS.Data.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace Aiplugs.CMS.Core.Services
{
    public interface IJobInfo
    {
    }
    public interface IContextFactory
    {
        IContext Create(Job job, ILogger logger, CancellationToken token, Action<int> onProgress);
    }
}
