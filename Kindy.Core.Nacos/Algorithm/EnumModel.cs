using System.ComponentModel;

namespace Kindy.Core.Nacos.Algorithm
{
    /// <summary>
    /// 算法枚举
    /// </summary>
    public enum AlgorithmEnum
    {
        // <summary>
        /// 随机
        /// </summary>
        [Description("随机")]
        Random = 1,

        /// <summary>
        /// 轮循
        /// </summary>
        [Description("轮循")]
        Polling = 2,
        /// <summary>
        /// 权重
        /// </summary>
        [Description("权重")]
        Weight = 3
    }
}
