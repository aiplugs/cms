using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Aiplugs.CMS
{
  public interface IContextFactory
  {
    Context Create(ILogger logger, CancellationToken token, Action<int> onProgress);
  }
}