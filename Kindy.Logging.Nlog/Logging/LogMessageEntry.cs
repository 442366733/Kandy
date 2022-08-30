using Kindy.EventBusClient.Rabbitmq;
using System;

namespace Kindy.Logging.Logging
{
    public class LogMessageEntry: IntegrationEvent
    {
        public string ProjectName { get; set; }
        public string ClassName { get; set; }
        public string LogTag { get; set; }
        public string LogType { get; set; }
        public string LogMessage { get; set; }
        public string LocalIp { get; set; }
        public DateTime AddTime { get; set; }
        public string TraceHead { get; set; }
    }
}
