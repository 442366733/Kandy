using Kindy.Logging.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Kindy.Logging.Nlog
{
    public class NLogLogger : ILogger
    {
        private readonly NLog.Logger _log;
        private readonly string _applicationName;
        private readonly string _className;
        private readonly ILoggerDispatcher _loggerDispatcher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string LogDispatcherCategoryNamePrefix = "Kindy";
        public NLogLogger(ILoggerDispatcher loggerDispatcher, IHttpContextAccessor httpContextAccessor, string categoryName, string applicationName)
        {
            _log = NLog.LogManager.GetLogger(categoryName);
            _loggerDispatcher = loggerDispatcher;
            _applicationName = applicationName;
            _className = categoryName;
            _httpContextAccessor = httpContextAccessor;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return _log.IsFatalEnabled;
                case LogLevel.Debug:
                    return _log.IsDebugEnabled;
                case LogLevel.Trace:
                    return _log.IsTraceEnabled;
                case LogLevel.Error:
                    return _log.IsErrorEnabled;
                case LogLevel.Information:
                    return _log.IsInfoEnabled;
                case LogLevel.Warning:
                    return _log.IsWarnEnabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            string message = null;
            if (null != formatter)
            {
                message = formatter(state, exception);
            }
            if (null != exception)
            {
                message = string.Format("【抛出信息】：{0} <br />【异常类型】：{1} <br />【异常信息】：{2} <br />【堆栈调用】：{3}", new object[] { message, exception.GetType().Name, exception.Message, exception.StackTrace });
                message = message.Replace("\r\n", "<br />");
                message = message.Replace("位置", "<strong style=\"color:red\">位置</strong>");
            }

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                if (_className.StartsWith(LogDispatcherCategoryNamePrefix))
                {
                    var localPort = string.Empty;
                    if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
                        localPort = ":" + _httpContextAccessor.HttpContext?.Connection?.LocalPort.ToString();
                    // 日志长度过长,暂时不采用其他解决方案
                    if (message.Length > 5120)
                        message = Substring2(message, 5120);
                    _loggerDispatcher.Dispatch(new LogMessageEntry()
                    {
                        ProjectName = _applicationName,
                        ClassName = _className,
                        LocalIp = GetLanIp() + localPort,
                        AddTime = DateTime.Now,
                        LogMessage = message,
                        LogType = logLevel.ToString(),
                        LogTag = string.Empty,
                        TraceHead = _httpContextAccessor?.HttpContext?.Request?.Headers["skyapm"].FirstOrDefault()
                    });
                }
                switch (logLevel)
                {
                    case LogLevel.Critical:
                        _log.Fatal(message);
                        break;
                    case LogLevel.Debug:
                        _log.Debug(message);
                        break;
                    case LogLevel.Trace:
                        _log.Trace(message);
                        break;
                    case LogLevel.Error:
                        _log.Error(message, exception, null);
                        break;
                    case LogLevel.Information:
                        _log.Info(message);
                        break;
                    case LogLevel.Warning:
                        _log.Warn(message);
                        break;
                    default:
                        _log.Warn($"未知日志级别{logLevel}");
                        _log.Info(message, exception, null);
                        break;
                }
            }
        }

        /// <summary>
        /// 获取局域网IP
        /// </summary>
        private string GetLanIp()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .Select(p => p.GetIPProperties())
                .SelectMany(p => p.UnicastAddresses)
                .Where(p => p.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(p.Address))
                .FirstOrDefault()?.Address.ToString();
        }

        /// <summary>
        /// 部分字符串获取
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxlen"></param>
        /// <returns></returns>
        private string Substring2(string str, int maxlen)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            if (str.Length <= maxlen)
                return str;
            return str.Substring(0, maxlen);
        }
    }
}
