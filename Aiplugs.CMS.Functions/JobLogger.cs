using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Aiplugs.CMS.Functions
{
  public class JobLogger : ILogger, IDisposable
  {
    private readonly ILogger _logger;
    private readonly TextWriter _writer;

    public JobLogger(ILogger logger, TextWriter writer)
    {
      _logger = logger;
      _writer = writer;
    }
    public IDisposable BeginScope<TState>(TState state)
    {
      return _logger.BeginScope<TState>(state);
    }

    public void Dispose()
    {
      if (_writer != null)
      {
        _writer.Dispose();
      }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
      return _logger.IsEnabled(logLevel);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
      _logger.Log(logLevel, eventId, state, exception, formatter);
      _writer.WriteLine($"[{logLevel}] {formatter(state, exception)}");
    }

    public void Flush()
    {
      _writer.Flush();
    }
  }
}