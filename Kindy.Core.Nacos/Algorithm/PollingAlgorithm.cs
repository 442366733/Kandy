using Nacos.V2.Naming.Dtos;
using System.Collections.Generic;
using System.Threading;

namespace Kindy.Core.Nacos.Algorithm
{
    /// <summary>
    /// Nacos服务实例算法-轮询
    /// </summary>
    internal class PollingAlgorithm
    {
        #region private fields
        private static Dictionary<string, int> _serviceDic = new Dictionary<string, int>();
        private static SpinLock _spinLock = new SpinLock();
        #endregion
        #region public method
        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <param name="serviceList"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static string Get(List<Instance> serviceList, string serviceName)
        {
            if (serviceList == null || string.IsNullOrEmpty(serviceName))
            {
                return null;
            }
            if (serviceList.Count == 1)
            {
                return $"{serviceList[0].Ip}:{serviceList[0].Port}";
            }

            bool locked = false;
            _spinLock.Enter(ref locked);//获取锁

            int index = -1;
            if (!_serviceDic.ContainsKey(serviceName))
            {
                _serviceDic.TryAdd(serviceName, index);
            }
            else
            {
                _serviceDic.TryGetValue(serviceName, out index);
            }
            ++index;

            string url;
            if (index > serviceList.Count - 1) //当前索引 > 最新服务最大索引
            {
                index = 0;
                url = $"{serviceList[0].Ip}:{serviceList[0].Port}";
            }
            else
            {
                url = $"{serviceList[index].Ip}:{serviceList[index].Port}";
            }
            _serviceDic[serviceName] = index;

            // 释放锁
            if (locked)
            {
                _spinLock.Exit();
            }
            return url;
        }
        #endregion
    }
}
