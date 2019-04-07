using Aiplugs.CMS.Data.Entities;
using Microsoft.Extensions.Logging;
using System;

namespace Aiplugs.CMS.Core.Logging
{
    public class JobLogger : ILogger
    {
        private readonly ILogger _logger;
        private readonly Job _job;

        public JobLogger(ILogger logger, Job job)
        {
            _logger = logger;
            _job = job;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                _job.Log += $"{DateTimeOffset.UtcNow:o} {logLevel.ToString()}: {message}\n" + (exception != null ? exception.ToString() : string.Empty);
            }
        }
        
    }
}
