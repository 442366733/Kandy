using System;
using System.Collections.Generic;
using System.Text;

namespace Kindy.EventBusClient.Rabbitmq
{
    /// <summary>
    /// 订阅注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class SubscribeAttribute : Attribute
    {
        public SubscribeAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// route key name.
        /// </summary>
        public string Name { get; }
    }
}
