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
        public SubscribeAttribute(string name, bool isdelay = false)
        {
            Name = name;
            IsDelayConsumeMessage = isdelay;
        }

        /// <summary>
        /// route key name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// delay processing message
        /// </summary>
        public bool IsDelayConsumeMessage { get; set; }

        /// <summary>
        /// message ttl
        /// </summary>
        public int MessageTTL => IsDelayConsumeMessage ? 0 : -1;
    }
}
