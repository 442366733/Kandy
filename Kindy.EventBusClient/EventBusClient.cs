using Kindy.EventBusClient.Rabbitmq.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Kindy.EventBusClient.Rabbitmq
{
    /// <summary>
    /// IEventBusClient
    /// </summary>
    public interface IEventBusClient
    {
        /// <summary>
        /// 注册事件源
        /// </summary>
        ConcurrentDictionary<string, ConsumerExecutorDescriptor> RegisterEventData { get; }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <param name="message_ttl"></param>
        void Publish<T>(string eventName, T message, int message_ttl = -1) where T : class, IntegrationEvent;

        /// <summary>
        /// 简单推送队列信息
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="routingkey"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="ttl"></param>
        void SimplePublish(string exchangeName, string routingkey, string message, string type = "topic", string? message_ttl = null);
    }

    /// <summary>
    /// EventBusClient
    /// </summary>
    public class EventBusClient : IEventBusClient, IDisposable
    {
        private bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;
        private IConnection _connection;
        private int _defaultPoolSize;
        private readonly ConcurrentQueue<IModel> _pool;
        private int _poolCount;
        private static bool _disposed = false;
        private static object _objLock = new object();

        private readonly EventBusConfigOptions _option;
        private readonly IRegisterEvent _registerEvents;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="option"></param>
        /// <param name="registerEvent"></param>
        /// <param name="serviceProvider"></param>
        public EventBusClient(
            IOptions<EventBusConfigOptions> option,
            IRegisterEvent registerEvent,
            IServiceProvider serviceProvider)
        {
            _option = option.Value;
            _registerEvents = registerEvent;
            _defaultPoolSize = 5;
            _serviceProvider = serviceProvider;
            _pool = new ConcurrentQueue<IModel>();
            if (_option.IsStartSubribe)
                StartConsume();
        }

        /// <summary>
        /// 启动消费
        /// </summary>
        public EventBusClient StartConsume()
        {
            Consume();
            DelayConsume();
            return this;
        }

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        private IConnection TryConnect()
        {
            if (IsConnected) return _connection;
            var factory = new ConnectionFactory()
            {
                HostName = _option.Transport.HostName,
                UserName = _option.Transport.UserName,
                Password = _option.Transport.Password,
                Port = _option.Transport.Port,
                VirtualHost = _option.Transport.VirtualHost
            };
            _connection = factory.CreateConnection();
            return _connection;
        }

        /// <summary>
        /// 获取通道
        /// </summary>
        /// <returns></returns>
        private IModel GetModel()
        {
            TryConnect();

            lock (_objLock)
            {
                IModel model = null;
                if (_poolCount >= _defaultPoolSize)
                {
                    _pool.TryDequeue(out model);
                    _pool.Enqueue(model);
                    return model;
                }
                else
                {
                    Interlocked.Increment(ref _poolCount);
                    model = _connection.CreateModel();
                    _pool.Enqueue(model);
                }
                return model;
            }
        }

        /// <summary>
        /// 消费消息
        /// </summary>
        private void Consume()
        {
            TryConnect();
            var channel = _connection.CreateModel();
            channel.ExchangeDeclare(exchange: _option.Transport.ExchangeName, type: "topic", durable: true);
            channel.QueueDeclare(queue: _option.Transport.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            foreach (var _ in GetSubscribeAttributeNames())
                channel.QueueBind(queue: _option.Transport.QueueName, exchange: _option.Transport.ExchangeName, routingKey: _);
            channel.BasicQos(prefetchSize: 0, prefetchCount: _option.Transport.PrefetchCount, global: false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var routingKey = GetRoutingKey(ea);
                    var isSubscribe = GetMethodHandleByEventName(routingKey) != null;
                    if (isSubscribe)
                    {
                        OnConsumeMessageReceived(model, routingKey, ea);
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else
                    {
                        channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                }
                catch (Exception)
                {
                    channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };
            channel.BasicConsume(queue: _option.Transport.QueueName, autoAck: false, consumer: consumer);
            Console.WriteLine("start consume exchange [{0}],queque [{1}],routingKey [{2}]...",
                _option.Transport.ExchangeName, _option.Transport.QueueName, string.Join(",", GetSubscribeAttributeNames()));
        }

        /// <summary>
        /// 延迟消费消息
        /// </summary>
        private void DelayConsume()
        {
            if (GetDelaySubscribeAttributeNames().Any() == false) return;

            TryConnect();
            var channel = _connection.CreateModel();

            //死信队列
            var delay_exchange = $"delay.{_option.Transport.ExchangeName}";
            var delay_queue = $"delay.{_option.Transport.QueueName}";
            var dlx_exchange = $"dlx.{_option.Transport.ExchangeName}";
            var dlx_queue = $"dlx.{_option.Transport.QueueName}";

            channel.ExchangeDeclare(exchange: dlx_exchange, type: "topic", durable: true);
            channel.QueueDeclare(queue: dlx_queue, durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind(queue: dlx_queue, exchange: dlx_exchange, routingKey: dlx_queue);

            var arguments = new Dictionary<string, object>();
            arguments.Add("x-dead-letter-exchange", dlx_exchange);
            arguments.Add("x-dead-letter-routing-key", dlx_queue);
            //arguments.Add("x-message-ttl", 10000);
            channel.ExchangeDeclare(exchange: delay_exchange, type: "topic", durable: true);
            channel.QueueDeclare(queue: delay_queue, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
            foreach (var _ in GetDelaySubscribeAttributeNames())
                channel.QueueBind(queue: delay_queue, exchange: delay_exchange, routingKey: _);

            channel.BasicQos(prefetchSize: 0, prefetchCount: _option.Transport.PrefetchCount, global: false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var routingKey = GetRoutingKey(ea);
                    var isSubscribe = GetMethodHandleByEventName(routingKey) != null;
                    if (isSubscribe)
                    {
                        OnConsumeMessageReceived(model, routingKey, ea);
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else
                    {
                        channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                }
                catch (Exception)
                {
                    channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };
            channel.BasicConsume(queue: dlx_queue, autoAck: false, consumer: consumer);
            Console.WriteLine("start consume exchange [{0}],queque [{1}],routingKey [{2}]...",
                _option.Transport.ExchangeName, _option.Transport.QueueName, string.Join(",", GetDelaySubscribeAttributeNames()));
        }

        /// <summary>
        /// 执行消费
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routingKey"></param>
        /// <param name="ea"></param>
        private void OnConsumeMessageReceived(object sender, string routingKey, BasicDeliverEventArgs ea)
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var hanlder = GetMethodHandleByEventName(routingKey);
            var obj = ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, hanlder.ImplTypeInfo);
            var methodInfo = hanlder.MethodInfo;

            var parameterType = hanlder.ParameterInfo.ParameterType;
            if (typeof(IntegrationEvent).IsAssignableFrom(parameterType) == false)
                throw new ArgumentException("Parameter needs to be inherited `IntegrationEvent`");

            var parameter = JsonConvert.DeserializeObject(message, parameterType);
            methodInfo.Invoke(obj, new object[] { parameter });
            Console.WriteLine($"received date {DateTime.Now.ToString()},received msg:{message},");
        }

        /// <summary>
        /// 获取RoutingKey
        /// </summary>
        /// <param name="ea"></param>
        /// <returns></returns>
        private string GetRoutingKey(BasicDeliverEventArgs ea)
        {
            var routingKey = string.Empty;
            if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.Any())
            {
                var x_death = ea.BasicProperties.Headers["x-death"] as List<object>;
                var x_death_details = x_death?.FirstOrDefault() as Dictionary<string, object>;
                if (x_death_details != null && x_death_details.ContainsKey("routing-keys"))
                {
                    var routingKey_bytes = (x_death_details.GetValueOrDefault("routing-keys") as List<object>).FirstOrDefault() as byte[];
                    routingKey = Encoding.UTF8.GetString(routingKey_bytes);
                }
                else
                {
                    routingKey = ea.RoutingKey;
                }
            }
            else
            {
                routingKey = ea.RoutingKey;
            }
            return routingKey;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void Publish<T>(string eventName, T message, int message_ttl = -1) where T : class, IntegrationEvent
        {
            var channel = GetModel();
            try
            {
                var exchangeName = message_ttl == -1 ? _option.Transport.ExchangeName : $"delay.{_option.Transport.ExchangeName}";
                var msg = JsonConvert.SerializeObject(message);
                channel.ExchangeDeclare(exchange: exchangeName, type: "topic", durable: true);

                var basicProperties = channel.CreateBasicProperties();
                basicProperties.Persistent = true;
                if (message_ttl != -1)
                    basicProperties.Expiration = message_ttl.ToString();

                var body = Encoding.UTF8.GetBytes(msg);
                channel.BasicPublish(exchange: exchangeName, routingKey: eventName, basicProperties: basicProperties, body: body);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 简单推送队列信息
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="routingkey"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="ttl"></param>
        public void SimplePublish(string exchangeName, string routingkey, string message, string type = "topic", string? message_ttl = null)
        {
            var channel = GetModel();
            try
            {
                channel.ExchangeDeclare(exchange: exchangeName, type: type, durable: true);

                var basicProperties = channel.CreateBasicProperties();
                basicProperties.Persistent = true;
                basicProperties.Expiration = message_ttl;
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: exchangeName, routingKey: routingkey, basicProperties: basicProperties, body: body);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 注册事件源
        /// </summary>
        public ConcurrentDictionary<string, ConsumerExecutorDescriptor> RegisterEventData => _registerEvents.EventData;

        /// <summary>
        /// 获取事件执行方法
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        private ConsumerExecutorDescriptor GetMethodHandleByEventName(string eventName)
        {
            _registerEvents.EventData.TryGetValue(eventName, out var handler);
            return handler;
        }

        /// <summary>
        /// 获取订阅的RoutingKey集合
        /// </summary>
        /// <returns></returns>
        private IList<string> GetSubscribeAttributeNames()
        {
            return _registerEvents.EventData.Where(x => x.Value.MessageTTL == -1).Select(x => x.Key).ToList();
        }

        /// <summary>
        /// 获取延迟订阅的RoutingKey集合
        /// </summary>
        /// <returns></returns>
        private IList<string> GetDelaySubscribeAttributeNames()
        {
            return _registerEvents.EventData.Where(x => x.Value.MessageTTL != -1).Select(x => x.Key).ToList();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            while (_pool.TryDequeue(out var _channel))
                _channel.Dispose();

            if (_connection != null)
                _connection.Dispose();

            _registerEvents.EventData = null;
            _disposed = true;
        }
    }
}
