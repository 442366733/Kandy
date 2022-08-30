using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using Kindy.DDDTemplate.API.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kindy.DDDTemplate.API.Extension
{
    /// <summary>
    /// 主机扩展
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// 添加apollo配置
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder ConfigureApollo(this IHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
            {
                configurationBuilder
                    .AddApollo(hostingContext.Configuration.GetSection("apollo"))
                    .AddDefault(ConfigFileFormat.Json);
            });
        }
        /// <summary>
        /// 添加日志配置
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder ConfigureLogging(this IHostBuilder builder)
        {
            return builder.ConfigureLogging((hostingContext, logging) =>
             {
                 logging.ClearProviders();
             });
        }
    }
}
