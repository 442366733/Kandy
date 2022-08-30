using Kindy.EventBusClient.Rabbitmq;
using Kindy.Logging.Logging;

namespace Kindy.Logging.Nlog.Logging
{
    public class LogEventTransport : ILoggerTransport
    {
        private readonly IEventBusClient _eventBus;

        public LogEventTransport(IEventBusClient eventBus)
        {
            _eventBus = eventBus;
        }

        public void Publish(LogMessageEntry logMessageEntry)
        {
            _eventBus.Publish("logstash", logMessageEntry);
        }
    }
}
