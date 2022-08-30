namespace Kindy.Logging.Logging
{
    public interface ILoggerTransport
    {
        void Publish(LogMessageEntry logMessageEntry);
    }
}
