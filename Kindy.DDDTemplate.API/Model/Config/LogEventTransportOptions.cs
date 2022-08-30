namespace Kindy.DDDTemplate.API.Model.Config
{
    /// <summary>
    /// 日志传输服务配置
    /// </summary>
    public class LogEventTransportOptions
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } 

        /// <summary>
        /// 虚拟主机
        /// </summary>
        public string VirtualHost { get; set; }

        /// <summary>
        /// 交互机
        /// </summary>
        public string ExchangeName { get; set; } 

        /// <summary>
        /// 队列
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
    }
}
