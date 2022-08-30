using System.Reflection;

namespace Kindy.EventBusClient.Rabbitmq
{
    /// <summary>
    /// 执行消费描述符
    /// </summary>
    public class ConsumerExecutorDescriptor
    {
        /// <summary>
        /// 类
        /// </summary>
        public TypeInfo ImplTypeInfo { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        public MethodInfo MethodInfo { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public ParameterInfo ParameterInfo { get; set; }
    }

}
