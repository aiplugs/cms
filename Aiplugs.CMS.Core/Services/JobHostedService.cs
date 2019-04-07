using Aiplugs.CMS.Core.Logging;
using Aiplugs.CMS.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Core.Services
{
    public class JobHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private const string ERR_EXCEPTION_THROWN = "ExceptionThrown";

        public JobHostedService(IServiceScopeFactory scopeFactory, ILogger<JobHostedService> logger)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(JobHostedService)} is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var registory = scope.ServiceProvider.GetRequiredService<IJobRegistory>();
                    try
                    {
                        var job = await registory.DequeueAsync();
                        if (job != null)
                            await Run(job, scope);

                        else
                            await Task.Delay(TimeSpan.FromSeconds(3000));
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation($"{nameof(JobHostedService)} is notified cancel event.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error has occured in job");
                    }
                }
            }
            _logger.LogInformation($"{nameof(JobHostedService)} is stopping.");
        }

        protected async Task Run(Job job, IServiceScope scope)
        {
            var jobService = scope.ServiceProvider.GetRequiredService<IJobService>();
            var procedureService = scope.ServiceProvider.GetRequiredService<IProcedureService>();
            var contextFactory = scope.ServiceProvider.GetRequiredService<IContextFactory>();

            var log = new JobLogger(_logger, job);
            var cts = new CancellationTokenSource();
            var context = contextFactory.Create(job, log, cts.Token, p => job.Progress = p);

            jobService.RegisterCanceller(job.Id, () => cts.Cancel(true));

            await Task.Factory.StartNew(() =>
            {
                job.StartAt = DateTime.UtcNow;
                job.Status = JobStatus.Running;
                try
                {
                    var procedure = procedureService.Resolve(job.Name);
                    procedure.ExecuteAsync(context).Wait(context.CancellationToken);
                }
                catch (Exception ex)
                {
                    var baseEx = ex.GetBaseException();
                    if (baseEx.GetType() == typeof(OperationCanceledException))
                    {
                        log.LogInformation("<< The job has been canceled. >>");
                        ExceptionDispatchInfo.Capture(baseEx).Throw();
                    }
                    else
                    {
                        log.LogError(ex, ex.Message);
                        context.Errors.Add(new Error(ERR_EXCEPTION_THROWN));
                    }
                }
            }, context.CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default)
            .ContinueWith(t =>
            {
                foreach (var error in context.Errors)
                {
                    log.LogError(error.ToString());
                }
                job.FinishAt = DateTime.UtcNow;

                if (t.IsCanceled)
                {
                    job.Status = JobStatus.Canceled;
                }
                else
                {
                    job.Progress = 100;
                    job.Status = context.Errors.Count == 0 ? JobStatus.Success : JobStatus.Faild;
                }
                jobService.SaveAsync(job).Wait(context.CancellationToken);
            });
        }
    }
}
