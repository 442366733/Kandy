namespace Kindy.DDDTemplate.API.Model.Config
{
    /// <summary>
    /// EventBus配置
    /// </summary>
    public class EventBusOptions
    {
        public RabbitMQ RabbitMQ { get; set; }
        public string ProviderName { get; set; }
        public string DbConnectionString { get; set; }
    }

    public class RabbitMQ
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string ExchangeName { get; set; }
    }
}
