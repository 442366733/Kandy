using Nacos.V2.Naming.Dtos;
using System;
using System.Collections.Generic;

namespace Kindy.Core.Nacos.Algorithm
{
    /// <summary>
    /// Nacos服务实例算法-随机
    /// </summary>
    internal class RandomAlgorithm
    {
        #region private fields
        private readonly static Random random = new Random();
        #endregion
        #region public method
        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <param name="serviceList"></param>
        /// <returns></returns>
        public static string Get(List<Instance> serviceList)
        {
            if (serviceList == null)
            {
                return null;
            }
            if (serviceList.Count == 1)
            {
                return $"{serviceList[0].Ip}:{serviceList[0].Port}";
            }

            int index = random.Next(serviceList.Count);
            string url = $"{serviceList[index].Ip}:{serviceList[index].Port}";

            return url;
        }
        #endregion
    }
}
