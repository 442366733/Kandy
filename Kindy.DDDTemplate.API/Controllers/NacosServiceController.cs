using Kindy.DDDTemplate.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nacos.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kindy.DDDTemplate.API.Controllers
{
    /// <summary>
    /// Nacos控制器
    /// </summary>
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class NacosServiceController : ControllerBase
    {
        private readonly ILogger<NacosServiceController> _logger;
        private readonly INacosNamingService _svc;
        private readonly IConfiguration _configuration;

        public NacosServiceController(
            ILogger<NacosServiceController> logger,
            INacosNamingService svc,
            IConfiguration configuration)
        {
            _logger = logger;
            _svc = svc;
            _configuration = configuration;
        }

        /// <summary>
        /// 查询Nacos服务实例
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpGet("testservice")]
        public async Task<string> TestService()
        {
            // need to know the service name.
            var instance = await _svc.SelectOneHealthyInstance("NetcoreBootstrap", "cncop");
            var host = $"{instance.Ip}:{instance.Port}";

            var baseUrl = instance.Metadata.TryGetValue("secure", out _)
                ? $"https://{host}"
                : $"http://{host}";

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return "empty";
            }

            var url = $"{baseUrl}/healthChecks";
            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetAsync(url);
                return await result.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// 查询Nacos配置信息
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        [HttpGet("testconfig")]
        public string TestConfig(string key)
        {
            return _configuration.GetValue<string>(key);
        }

        /// <summary>
        /// 查询所有配置信息
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        [HttpGet("allconfig")]
        public string TestAllConfig()
        {
            var sb = new StringBuilder();
            foreach (var kv in _configuration.AsEnumerable())
            {
                sb.AppendLine($"key:{kv.Key},value:{kv.Value}");
            }
            return sb.ToString();
        }
    }
}
