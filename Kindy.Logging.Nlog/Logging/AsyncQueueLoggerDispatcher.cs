using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Kindy.Logging.Logging
{
    public class AsyncQueueLoggerDispatcher : ILoggerDispatcher
    {
        private readonly BlockingCollection<LogMessageEntry> _messageQueue;
        private readonly CancellationTokenSource _cancellation;

        public AsyncQueueLoggerDispatcher(ILoggerTransport loggerTransport)
        {
            _messageQueue = new BlockingCollection<LogMessageEntry>();
            _cancellation = new CancellationTokenSource();
            Task.Factory.StartNew(() => { Flush(loggerTransport); }, TaskCreationOptions.LongRunning);
        }
        public bool Dispatch(LogMessageEntry logMessage)
        {
            if (_cancellation.IsCancellationRequested)
                return false;
            _messageQueue.TryAdd(logMessage);
            return true;
        }
        public Task Flush(ILoggerTransport loggerTransport, CancellationToken token = default)
        {
            while (_messageQueue.TryTake(out var message, -1))
            {
                loggerTransport.Publish(message);
            }
            return Task.CompletedTask;
        }
        public void Close()
        {
            _cancellation.Cancel();
        }
    }
}
