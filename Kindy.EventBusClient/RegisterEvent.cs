using System.Collections.Concurrent;

namespace Kindy.EventBusClient.Rabbitmq
{
    /// <summary>
    /// 注册事件源
    /// </summary>
    public class RegisterEvent : IRegisterEvent
    {
        public ConcurrentDictionary<string, ConsumerExecutorDescriptor> EventData { get; set; }
    }

    public interface IRegisterEvent
    {
        ConcurrentDictionary<string, ConsumerExecutorDescriptor> EventData { get; set; }
    }
}
