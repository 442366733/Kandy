using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        void Publish<T>(string eventName, T message) where T : class, IntegrationEvent;
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

            //死信队列
            var exchange_dlx = $"{_option.Transport.ExchangeName}.dlx";
            var queue_dlx = $"{_option.Transport.QueueName}.error";
            var routekey_dlx = "#";
            var arguments = new Dictionary<string, object>();
            arguments.Add("x-dead-letter-exchange", exchange_dlx);

            channel.QueueDeclare(queue: queue_dlx, durable: true, exclusive: false, autoDelete: false);
            channel.ExchangeDeclare(exchange: exchange_dlx, type: "topic", durable: true);
            channel.QueueBind(queue: queue_dlx, exchange: exchange_dlx, routingKey: routekey_dlx);

            channel.QueueDeclare(queue: _option.Transport.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
            channel.ExchangeDeclare(exchange: _option.Transport.ExchangeName, type: "topic", durable: true);
            foreach (var _ in GetSubscribeAttributeNames())
                channel.QueueBind(queue: _option.Transport.QueueName, exchange: _option.Transport.ExchangeName, routingKey: _);

            channel.BasicQos(prefetchSize: 0, prefetchCount: _option.Transport.PrefetchCount, global: false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var isSubscribe = GetMethodHandleByEventName(ea.RoutingKey) != null;
                    if (isSubscribe)
                    {
                        OnConsumeMessageReceived(model, ea);
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
        /// 执行消费
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        private void OnConsumeMessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var routingKey = string.Empty;
            if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.Any())
            {
                var x_death = ea.BasicProperties.Headers["x-death"] as List<object>;
                var x_death_details = x_death.FirstOrDefault() as Dictionary<string, object>;
                if (x_death_details.ContainsKey("routing-keys"))
                {
                    var routingKey_bytes = (x_death_details["routing-keys"] as List<object>)[0] as byte[];
                    routingKey = Encoding.UTF8.GetString(routingKey_bytes);
                }
            }
            else
            {
                routingKey = ea.RoutingKey;
            }
            var hanlder = GetMethodHandleByEventName(routingKey);
            var obj = ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, hanlder.ImplTypeInfo);
            var methodInfo = hanlder.MethodInfo;

            var parameterType = hanlder.ParameterInfo.ParameterType;
            if (typeof(IntegrationEvent).IsAssignableFrom(parameterType) == false)
                throw new ArgumentException("Parameter needs to be inherited `IntegrationEvent`");

            var parameter = JsonConvert.DeserializeObject(message, parameterType);
            methodInfo.Invoke(obj, new object[] { parameter });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void Publish<T>(string eventName, T message) where T : class, IntegrationEvent
        {
            var channel = GetModel();
            try
            {
                var msg = JsonConvert.SerializeObject(message);
                channel.ExchangeDeclare(exchange: _option.Transport.ExchangeName, type: "topic", durable: true);

                var basicProperties = channel.CreateBasicProperties();
                basicProperties.Persistent = true;

                var body = Encoding.UTF8.GetBytes(msg);
                channel.BasicPublish(exchange: _option.Transport.ExchangeName, routingKey: eventName, basicProperties: basicProperties, body: body);
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
            return _registerEvents.EventData.Select(x => x.Key).ToList();
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
