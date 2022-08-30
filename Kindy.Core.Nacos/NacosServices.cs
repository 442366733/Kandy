using Kindy.Core.Nacos.Algorithm;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nacos.V2;
using Nacos.V2.Naming.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kindy.Core.Nacos
{
    /// <summary>
    /// Nacos服务
    /// </summary>
    public class NacosServices : INacosServices
    {
        #region private fields
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INacosNamingService _nacosNaming;
        private readonly IConfiguration _configuration;
        #endregion

        #region construction
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="nacosNaming"></param>
        /// <param name="configuration"></param>
        /// <param name="loggerFactory"></param>
        public NacosServices(
            IHttpContextAccessor httpContextAccessor,
            INacosNamingService nacosNaming,
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _nacosNaming = nacosNaming;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<NacosServices>();
        }
        #endregion

        #region public method
        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="groupName">分组名称，默认cncop</param>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        public async Task<string> GetServiceAsync(string serviceName, string groupName = "cncop", AlgorithmEnum algorithm = AlgorithmEnum.Random)
        {
            var listService = await _nacosNaming.SelectInstances(serviceName: serviceName, groupName: groupName, healthy: true);
            if (listService == null || listService.Any() == false)
            {
                string errorMsg = $"在 [{groupName}] 组中未找到 [{serviceName}] 服务，可能原因：[{serviceName}] 未注册到nacos中。";
                _logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
            if (listService.Count == 1)
            {
                var sl = listService[0];
                return $"{sl.Ip}:{sl.Port}";
            }
            return GetInstancesHost(listService, serviceName, algorithm);
        }
        #endregion

        #region private method
        /// <summary>
        /// 根据算法获取实例
        /// </summary>
        /// <param name="listService">服务列表</param>
        /// <param name="algorithm">数据算法</param>
        /// <returns></returns>
        private string GetInstancesHost(List<Instance> listService, string serviceName, AlgorithmEnum algorithm)
        {
            switch (algorithm)
            {
                case AlgorithmEnum.Random:
                    return RandomAlgorithm.Get(listService);
                case AlgorithmEnum.Polling:
                    return PollingAlgorithm.Get(listService, serviceName);
                case AlgorithmEnum.Weight:
                    return RandomAlgorithm.Get(listService);
                default:
                    return PollingAlgorithm.Get(listService, serviceName);
            }
        }
        #endregion
    }
}
