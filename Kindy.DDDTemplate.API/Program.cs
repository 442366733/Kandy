using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Kindy.Logging.Nlog;
using Kindy.DDDTemplate.API.Extension;

namespace Kindy.DDDTemplate.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var defaultBuilder = Host.CreateDefaultBuilder(args);
            return defaultBuilder
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddNacosV2Configuration(builder.Build().GetSection("NacosConfig"));
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddLog(hostingContext.Configuration, "Kindy.DDDTemplate.API");
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
        }
    }
}
