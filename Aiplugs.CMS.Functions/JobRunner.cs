using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Aiplugs.CMS.Functions
{
  public class JobRunner : IDisposable
  {
    private readonly IJobService _jobService;
    private readonly IContextFactory _contextFactory;
    private readonly ILogger _logger;
    
    CancellationTokenSource CancellationTokenSource;
    public JobRunner(IJobService jobService, IContextFactory contextFactory, ILogger logger)
    {
      _jobService = jobService;
      _contextFactory = contextFactory;
      _logger = logger;
    }

    public Task Start()
    {
      CancellationTokenSource = new CancellationTokenSource();
      return Task.Factory.StartNew(() => {
        while (true)
        {
          try
          {
            var job = _jobService.Dequeue();
            if (job != null)
            {
              Run(job);
            }
          }
          catch(Exception ex)
          {
            _logger.LogError(ex, "An error has occured in job");
          }
        }
      }, CancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void NotifyStop()
    {
      _jobService.CancelAll();
      CancellationTokenSource?.Cancel(true);
    }

    public void Dispose()
    {
      CancellationTokenSource?.Dispose();
      CancellationTokenSource = null;
    }

    public void Run(Job job)
    {
      using(var log = new JobLogger(_logger, job.LogWriter))
      using(var cts = new CancellationTokenSource())
      {
        var context = _contextFactory.Create(log, cts.Token, p => job.Progress = p);

        _jobService.RegisterCanceller(job, () => cts.Cancel(true));
        
        job.Run(context).ContinueWith(t => {
          _jobService.Update(job);
          _jobService.UnregisterCanceller(job);
          job.Dispose();
        });
      }
    }
  }
}