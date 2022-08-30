
using System.Threading;
using System.Threading.Tasks;

namespace Kindy.Logging.Logging
{
    public interface ILoggerDispatcher
    {
        bool Dispatch(LogMessageEntry segment);
        Task Flush(ILoggerTransport loggerTransport, CancellationToken token = default);
        void Close();
    }
}
