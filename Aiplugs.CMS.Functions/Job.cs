using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Aiplugs.CMS.Functions
{
  public class Job : IDisposable
  {
    public Job()
    {
      LogWriter = new StringWriter(new StringBuilder()); 
    }
    public Job(IProcedure procedure) : this()
    {
      Procedure = procedure;
    }
    public IProcedure Procedure { get; set; } 
    public JobStatus Status { get; set; } = JobStatus.Ready;
    public int Progress { get; set; }
    public DateTimeOffset? Start { get; set; }
    public DateTimeOffset? Finish { get; set; }
    public string Log 
    {
      get {
        return LogWriter != null ? LogWriter.ToString() : _log;
      }
      set {
        LogWriter.Dispose();
        LogWriter = new StringWriter(new StringBuilder(value));
      }
    }
    private string _log = string.Empty;
    public TextWriter LogWriter {get; private set;}
    public Task Run(Context context)
    {
      var method = Procedure.CreateMethod(context);

      return Task.Factory
        .StartNew(() =>
          {
            
            Start = DateTimeOffset.Now;
            Status = JobStatus.Running;
            try
            {
              method.Invoke(null, new []{ context });
            }
            catch(Exception ex)
            {
              context.Logger.LogError(ex, ex.Message);
            }
          }, context.CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default)
        .ContinueWith(t =>
          {
            Finish = DateTimeOffset.Now;
            Progress = 100;

            if (t.IsCanceled)
              Status = JobStatus.Cenceled;

            else
              Status = context.Errors.Count == 0 ? JobStatus.Success : JobStatus.Faild;
          });
    }

    public void Dispose()
    {
      if (LogWriter != null)
      {
        LogWriter.Flush();

        _log = LogWriter.ToString();
        
        LogWriter.Dispose();
        LogWriter = null;
      }
    }
  }
}