using Kindy.Core.Nacos.Algorithm;
using System.Threading.Tasks;

namespace Kindy.Core.Nacos
{
    /// <summary>
    /// Nacos服务
    /// </summary>
    public interface INacosServices
    {
        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="groupName">分组名称，默认cncop</param>
        /// <param name="algorithm">算法，默认轮询</param>
        /// <returns></returns>
        Task<string> GetServiceAsync(string serviceName, string groupName = "cncop", AlgorithmEnum algorithm = AlgorithmEnum.Polling);
    }
}
