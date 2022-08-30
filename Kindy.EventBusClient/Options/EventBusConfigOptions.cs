namespace Kindy.EventBusClient.Rabbitmq
{
    /// <summary>
    /// EventBus配置项
    /// </summary>
    public class EventBusConfigOptions
    {
        /// <summary>
        /// 是否订阅
        /// </summary>
        public bool IsStartSubribe { get; set; }

        /// <summary>
        /// 传输介质
        /// </summary>
        public RabbitmqClientOptions Transport { get; set; }
    }

    /// <summary>
    /// RabbitMq配置项
    /// </summary>
    public class RabbitmqClientOptions
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string HostName { get; set; } = "localhost";

        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; } = "guest";

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = "guest";

        /// <summary>
        /// 虚拟主机
        /// </summary>
        public string VirtualHost { get; set; } = "/";

        /// <summary>
        /// 交互机
        /// </summary>
        public string ExchangeName { get; set; } = "rb.exchange";

        /// <summary>
        /// 队列
        /// </summary>
        public string QueueName { get; set; } = "rb.queue";

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; } = 5672;

        /// <summary>
        /// 消费个数
        /// </summary>
        public ushort PrefetchCount { get; set; } = 1;
    }
}
