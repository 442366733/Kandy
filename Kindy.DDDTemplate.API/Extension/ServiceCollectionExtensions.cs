using AutoMapper;
using FluentValidation.AspNetCore;
using Kindy.DatabaseAccessor.SqlSugar.Repositories;
using Kindy.DDDTemplate.API.Extension.ControllerJson;
using Kindy.DDDTemplate.API.Model.Config;
using Kindy.DDDTemplate.Application.IntegrationEvents.Order;
using Kindy.DDDTemplate.Infrastructure.Context.CRM;
using Kindy.DDDTemplate.Infrastructure.Context.Master;
using Kindy.EventBusClient.Rabbitmq;
using Kindy.Infrastructure.Core.Repository;
using Kindy.Logging.Logging;
using Kindy.Logging.Nlog;
using Kindy.Logging.Nlog.Logging;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SqlSugar;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Kindy.DDDTemplate.API.Extension
{
    /// <summary>
    /// 服务扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        #region EFContext

        /// <summary>
        /// 添加EFContext
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddEFDBContext(this IServiceCollection services, IConfiguration configuration)
        {
            //添加事务对应CAP eventbus处理服务
            services.AddTransient<ISubscriberService, SubscriberService>();

            var config = configuration.GetSection("DbConfig").Get<DbConfigOptions>();
            var eventBusconfig = configuration.GetSection("EventBusConfig").Get<EventBusOptions>();

            //添加多个DB上下文
            services.AddCRMDBContext(config);
            services.AddMasterDBContext(config);
            return services.AddEventBus(eventBusconfig);
        }

        private static IServiceCollection AddEventBus(this IServiceCollection services, EventBusOptions config)
        {
            //加入eventbus
            //https://github.com/dotnetcore/CAP
            services.AddCap(options =>
            {
                options.DefaultGroup = "kindy";
                options.UseEntityFramework<MasterDBContext>();
                options.UseRabbitMQ(options =>
                {
                    options.HostName = config.RabbitMQ.HostName;
                    options.UserName = config.RabbitMQ.UserName;
                    options.Password = config.RabbitMQ.Password;
                    options.Port = config.RabbitMQ.Port;
                    options.VirtualHost = config.RabbitMQ.VirtualHost;
                });
                options.FailedRetryCount = 0;
                //cap面板。暂时不用需要引入capDashboard组件
                //options.UseDashboard();
            });
            return services;
        }
        private static IServiceCollection AddMasterDBContext(this IServiceCollection services, DbConfigOptions config)
        {
            //事务处理服务  MediatR 管道处理，只针对MediatR
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MasterTransactionBehavior<,>));
            //ef上下文服务
            var dbconfig = config.DbConfigs.FirstOrDefault(x => x.DbNumber == "1");
            services.AddDbContext<MasterDBContext>(builder =>
            {
                builder.UseMySql(dbconfig.DbString);
            });
            return services;
        }

        private static IServiceCollection AddCRMDBContext(this IServiceCollection services, DbConfigOptions config)
        {
            //事务处理服务  MediatR 管道处理，只针对MediatR
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MasterTransactionBehavior<,>));
            //ef上下文服务
            var dbconfig = config.DbConfigs.FirstOrDefault(x => x.DbNumber == "2");
            services.AddDbContext<CRMDBContext>(builder =>
            {
                builder.UseMySql(dbconfig.DbString);
            });
            return services;
        }
        #endregion

        #region MediatRServices
        /// <summary>
        /// 添加  MediatRServices
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.Load("Kindy.DDDTemplate.Application"));
            return services.AddMediatR(Assembly.GetExecutingAssembly());
        }
        #endregion

        #region 注入仓库服务
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            var assembly = Assembly.Load("Kindy.DDDTemplate.Infrastructure");
            var allTypes = assembly.GetTypes();
            var irepository = assembly.GetTypes().Where(t =>
             t.IsInterface && t.GetInterfaces().Where(t => t.Name == typeof(IRepository<>).Name).Any())
                .ToList();
            foreach (var item in irepository)
            {
                var impls = allTypes.Where(t => t.IsClass && t.GetInterfaces().Contains(item)).ToList();
                foreach (var impl in impls)
                {
                    services.TryAddEnumerable(ServiceDescriptor.Scoped(item, impl));
                }
            }
            return services;
        }
        #endregion

        #region 添加Swagge服务
        /// <summary>
        /// 添加swagger使用服务
        /// </summary>
        /// <param name="services"></param>
        /// <see cref="https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-5.0"/>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "模板 API",
                    Description = "A simple example ASP.NET Core Web API",
                    //TermsOfService = new Uri("http://www.shjinjia.com.cn/"),
                    //Contact = new OpenApiContact
                    //{
                    //    Name = "模板 API",
                    //    Email = string.Empty,
                    //    Url = new Uri("模板 API"),
                    //},
                    //License = new OpenApiLicense
                    //{
                    //    Name = "Use under LICX",
                    //    Url = new Uri("http://www.shjinjia.com.cn/"),
                    //}
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                var openApiSecurity = new OpenApiSecurityScheme
                {
                    Description = "用户登录Token",
                    Name = "token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                };
                c.AddSecurityDefinition("Token认证", openApiSecurity);
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                c.OperationFilter<SecurityRequirementsOperationFilter>(true, "Token认证");
            });

            return services;
        }
        #endregion

        #region 自定义控制器  json序列化   模型验证
        public static IServiceCollection AddCustomerControllers(this IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new ExtendedCamelCaseContractResolver();
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.Formatting = Formatting.Indented;
            });

            //https://github.com/FluentValidation/FluentValidation
            services.AddControllersWithViews()
                    .AddFluentValidation(config =>//添加FluentValidation验证
                    {
                        //程序集方式添加验证
                        config.RegisterValidatorsFromAssemblies(new Assembly[] { Assembly.Load("Kindy.DDDTemplate.Application") });
                        //config.RegisterValidatorsFromAssemblyContaining(typeof(验证的类));
                        //是否与MvcValidation共存，设置为false后将不再执行特性方式的验证
                        config.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    });
            return services;
        }
        #endregion

        #region 模型映射
        /// <summary>
        /// 添加模型映射
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddModelMapper(this IServiceCollection services)
        {
            var assembies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            services.AddAutoMapper(assembies);
            return services;
        }
        #endregion

        #region Options模型映射
        /// <summary>
        /// 添加模型映射
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddOptionsExt(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DbConfigOptions>(configuration.GetSection("DbConfig"));
            services.Configure<EventBusOptions>(configuration.GetSection("EventBus"));
            return services;
        }
        #endregion

        #region SqlsugarScope的配置
        /// <summary>
        /// SqlsugarScope的配置
        /// Scope必须用单例注入
        /// 不可以用Action委托注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddSqlSugarScopeMutilDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var dbList = configuration.GetSection("DbConfig").Get<DbConfigOptions>().DbConfigs;
            var connectConfigList = new List<ConnectionConfig>();

            foreach (var item in dbList)
            {
                //防止数据库重复，导致的事务异常
                if (connectConfigList.Any(a => a.ConfigId == (dynamic)item.DbNumber || a.ConnectionString == item.DbString))
                {
                    continue;
                }
                connectConfigList.Add(new ConnectionConfig()
                {
                    ConnectionString = item.DbString,
                    DbType = (DbType)Convert.ToInt32(Enum.Parse(typeof(DbType), item.DbType)),
                    IsAutoCloseConnection = true,
                    ConfigId = item.DbNumber,
                    InitKeyType = InitKeyType.Attribute,
                    MoreSettings = new ConnMoreSettings()
                    {
                        IsAutoRemoveDataCache = true//自动清理缓存
                    },
                    ConfigureExternalServices = new ConfigureExternalServices()
                    {
                        //DataInfoCacheService = new SqlSugarCache(),
                        EntityNameService = (type, entity) =>
                        {
                            var attributes = type.GetCustomAttributes(true);
                            if (attributes.Any(it => it is TableAttribute))
                            {
                                entity.DbTableName = (attributes.First(it => it is TableAttribute) as TableAttribute).Name;
                            }
                        },
                        EntityService = (type, column) =>
                        {
                            var attributes = type.GetCustomAttributes(true);
                            if (attributes.Any(it => it is KeyAttribute))
                            {
                                column.IsPrimarykey = true;
                            }
                            if (attributes.Any(it => it is ColumnAttribute))
                            {
                                column.DbColumnName = (attributes.First(it => it is ColumnAttribute) as ColumnAttribute).Name;
                            }
                        }
                    }
                });
            }

            SqlSugarScope sqlSugarScope = new SqlSugarScope(connectConfigList,
                //全局上下文生效
                db =>
                {
                    /*
                     * 默认只会配置到第一个数据库，这里按照官方文档进行多数据库/多租户文档的说明进行循环配置
                     */
                    foreach (var c in connectConfigList)
                    {
                        var dbProvider = db.GetConnectionScope((string)c.ConfigId);
                        //执行超时时间
                        dbProvider.Ado.CommandTimeOut = 30;

                        dbProvider.Aop.OnLogExecuting = (sql, pars) =>
                        {
                            if (sql.StartsWith("SELECT"))
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                            }
                            if (sql.StartsWith("UPDATE") || sql.StartsWith("INSERT"))
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            if (sql.StartsWith("DELETE"))
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                            Console.WriteLine("Sql:" + "\r\n\r\n" + UtilMethods.GetSqlString(db.CurrentConnectionConfig.DbType, sql, pars));
                        };

                        dbProvider.Aop.DataExecuting = (oldValue, entityInfo) =>
                        {
                            //// 新增操作
                            //if (entityInfo.OperationType == DataFilterType.InsertByObject)
                            //{
                            //    // 主键(long)-赋值雪花Id
                            //    if (entityInfo.EntityColumnInfo.IsPrimarykey && entityInfo.EntityColumnInfo.PropertyInfo.PropertyType == typeof(long))
                            //    {
                            //        var id = ((dynamic)entityInfo.EntityValue).Id;
                            //        if (id == null || id == 0)
                            //            entityInfo.SetValue(Yitter.IdGenerator.YitIdHelper.NextId());
                            //    }


                            //    if (entityInfo.PropertyName == "CreatedTime")
                            //        entityInfo.SetValue(DateTime.Now);
                            //    if (App.User != null)
                            //    {
                            //        if (entityInfo.PropertyName == "TenantId")
                            //        {
                            //            var tenantId = ((dynamic)entityInfo.EntityValue).TenantId;
                            //            if (tenantId == null || tenantId == 0)
                            //                entityInfo.SetValue(App.User.FindFirst(ClaimConst.TENANT_ID)?.Value);
                            //        }
                            //        if (entityInfo.PropertyName == "CreatedUserId")
                            //        {
                            //            var createUserId = ((dynamic)entityInfo.EntityValue).CreatedUserId;
                            //            if (createUserId == null || createUserId == 0)
                            //                entityInfo.SetValue(App.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value);
                            //        }

                            //        if (entityInfo.PropertyName == "CreatedUserName")
                            //            entityInfo.SetValue(App.User.FindFirst(ClaimConst.CLAINM_NAME)?.Value);
                            //    }
                            //}
                            //// 更新操作
                            //if (entityInfo.OperationType == DataFilterType.UpdateByObject)
                            //{
                            //    if (entityInfo.PropertyName == "UpdatedTime")
                            //        entityInfo.SetValue(DateTime.Now);
                            //    if (entityInfo.PropertyName == "UpdatedUserId")
                            //        entityInfo.SetValue(App.User?.FindFirst(ClaimConst.CLAINM_USERID)?.Value);
                            //    if (entityInfo.PropertyName == "UpdatedUserName")
                            //        entityInfo.SetValue(App.User?.FindFirst(ClaimConst.CLAINM_NAME)?.Value);
                            //}
                        };
                    }
                });
            services.AddSingleton<ISqlSugarClient>(sqlSugarScope);
            // 注册非泛型仓储
            services.AddScoped<ISqlSugarRepository, SqlSugarRepository>();
            // 注册 SqlSugar 仓储
            services.AddScoped(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));
        }
        #endregion

        #region
        /// <summary>
        /// 日志采集转发配置
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddLogEventTransport(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerTransport, LogEventTransport>();
            services.AddSingleton<ILoggerDispatcher, AsyncQueueLoggerDispatcher>();
            return services;
        }
        #endregion

        #region 日志配置
        /// <summary>
        /// 添加日志应用
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddLog(this ILoggingBuilder builder, IConfiguration configuration, string projectName)
        {
            builder.ClearProviders();
            builder.AddConsole();
            builder.Services.AddLogEventBus(configuration);
            builder.Services.AddLogEventTransport();
            builder.Services.AddSingleton<ILoggerTransport, LogEventTransport>();
            builder.Services.AddSingleton<ILoggerDispatcher, AsyncQueueLoggerDispatcher>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<ILoggerProvider>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var loggerDispatcher = sp.GetRequiredService<ILoggerDispatcher>();
                return new NLogProvider(loggerDispatcher, httpContextAccessor, projectName);
            });
            return builder;
        }
        #endregion

        /// <summary>
        /// 添加事件总线
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddLogEventBus(this IServiceCollection services, IConfiguration configuration, string sectionPath = "LogEventTransportConfig")
        {
            var config = configuration.GetSection(sectionPath).Get<LogEventTransportOptions>();
            services.AddEventBus(option =>
            {
                option.IsStartSubribe = true;
                option.Transport = new RabbitmqClientOptions
                {
                    HostName = config.HostName,
                    UserName = config.UserName,
                    Password = config.Password,
                    Port = config.Port,
                    VirtualHost = config.VirtualHost,
                    ExchangeName = config.ExchangeName,
                    QueueName = config.QueueName,
                };
            });
            return services;
        }
    }
}
