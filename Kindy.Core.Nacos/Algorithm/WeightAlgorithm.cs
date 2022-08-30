using Nacos.V2.Naming.Dtos;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Kindy.Core.Nacos.Algorithm
{
    /// <summary>
    /// Nacos服务实例算法-权重
    /// </summary>
    internal class WeightAlgorithm
    {
        #region private fields
        private static ConcurrentDictionary<string, WeightAlgorithmItem> _serviceDic = new ConcurrentDictionary<string, WeightAlgorithmItem>();
        private static SpinLock _spinLock = new SpinLock();
        #endregion
        #region private method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weightAlgorithmItem"></param>
        /// <param name="serviceList"></param>
        private static void BuildWeightAlgorithmItem(WeightAlgorithmItem weightAlgorithmItem, List<Instance> serviceList)
        {
            serviceList.ForEach(service => // 有几个权重就加几个实例
            {
                int weight = 1;
                if (service.Weight > 0)
                {
                    // 获取权重值
                    int.TryParse(service.Weight.ToString(), out weight);
                }
                for (int i = 0; i < weight; i++)
                {
                    weightAlgorithmItem.Urls.Add($"{service.Ip}:{service.Port}");
                }
            });
        }
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
            if (serviceList == null)
            {
                return null;
            }
            if (serviceList.Count == 1)
            {
                return $"{serviceList[0].Ip}:{serviceList[0].Port}";
            }

            bool locked = false;
            //获取锁
            _spinLock.Enter(ref locked);

            WeightAlgorithmItem weightAlgorithmItem = null;
            if (!_serviceDic.ContainsKey(serviceName))
            {
                weightAlgorithmItem = new WeightAlgorithmItem()
                {
                    Index = -1,
                    Urls = new List<string>()
                };
                BuildWeightAlgorithmItem(weightAlgorithmItem, serviceList);
                _serviceDic.TryAdd(serviceName, weightAlgorithmItem);
            }
            else
            {
                _serviceDic.TryGetValue(serviceName, out weightAlgorithmItem);
                weightAlgorithmItem.Urls.Clear();
                BuildWeightAlgorithmItem(weightAlgorithmItem, serviceList);
            }

            ++weightAlgorithmItem.Index;

            string url;
            if (weightAlgorithmItem.Index > weightAlgorithmItem.Urls.Count - 1) // 当前索引 > 最新服务最大索引
            {
                weightAlgorithmItem.Index = 0;
                url = $"{serviceList[0].Ip}:{serviceList[0].Port}";
            }
            else
            {
                url = weightAlgorithmItem.Urls[weightAlgorithmItem.Index];
            }
            _serviceDic[serviceName] = weightAlgorithmItem;

            // 释放锁
            if (locked)
            {
                _spinLock.Exit();
            }
            return url;
        }
        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    internal class WeightAlgorithmItem
    {
        public List<string> Urls { get; set; }
        public int Index { get; set; }
    }
}
