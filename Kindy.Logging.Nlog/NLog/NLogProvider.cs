using Kindy.Logging.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Kindy.Logging.Nlog
{
    public class NLogProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, NLogLogger> _loggers = new ConcurrentDictionary<string, NLogLogger>();
        private readonly ILoggerDispatcher _loggerDispatcher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _applicationName;

        public NLogProvider(ILoggerDispatcher loggerDispatcher, IHttpContextAccessor httpContextAccessor, string applicationName)
        {
            _loggerDispatcher = loggerDispatcher;
            _httpContextAccessor = httpContextAccessor;
            _applicationName = applicationName;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, new NLogLogger(_loggerDispatcher, _httpContextAccessor, categoryName, _applicationName));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
