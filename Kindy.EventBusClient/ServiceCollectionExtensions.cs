using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Kindy.EventBusClient.Rabbitmq
{
    /// <summary>
    /// 服务扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加事件总线
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventBus(this IServiceCollection services, Action<EventBusConfigOptions> options)
        {
            var option = new EventBusConfigOptions();
            options?.Invoke(option);
            services.Configure(options);
            if (option.IsStartSubribe)
                services.AddEventBusService();
            else
                services.TryAddSingleton<IRegisterEvent, RegisterEvent>();
            services.AddSingleton<IEventBusClient, EventBusClient>();
            return services;
        }

        /// <summary>
        /// 注册EventBus事件服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="scanModules"></param>
        /// <returns></returns>
        private static IServiceCollection AddEventBusService(this IServiceCollection services, Assembly[]? scanModules = null)
        {
            var registerEvents = new ConcurrentDictionary<string, ConsumerExecutorDescriptor>();
            if (scanModules == null) scanModules = new Assembly[] { Assembly.GetExecutingAssembly() };
            foreach (var assembly in scanModules)
            {
                var allTypes = assembly.GetTypes();
                var ieventBusService = allTypes.Where(t =>
                 t.IsInterface && t.Name == typeof(IEventBusService).Name)
                    .ToList();
                foreach (var item in ieventBusService)
                {
                    var impls = allTypes.Where(t => t.IsClass && t.GetInterfaces().Contains(item)).ToList();
                    foreach (var impl in impls)
                    {
                        services.TryAddEnumerable(ServiceDescriptor.Scoped(item, impl));
                        foreach (var method in impl.GetTypeInfo().DeclaredMethods)
                        {
                            var topicAttr = method.GetCustomAttribute<SubscribeAttribute>(true);
                            if (topicAttr == null)
                                continue;

                            var parameter = method.GetParameters()?.FirstOrDefault();
                            if (method.GetParameters().Count() != 1)
                                throw new ArgumentException("error method parameter count of [SubscribeAttribute]");
                            registerEvents.TryAdd(topicAttr.Name, new ConsumerExecutorDescriptor { MessageTTL = topicAttr.MessageTTL, ImplTypeInfo = impl.GetTypeInfo(), MethodInfo = method, ParameterInfo = parameter });
                        }
                    }
                }
            }
            services.TryAddSingleton<IRegisterEvent>(sp => { return new RegisterEvent { EventData = registerEvents }; });
            return services;
        }
    }
}
