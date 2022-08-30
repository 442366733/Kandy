using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace Kindy.Infrastructure.Core.Context
{
    /// <summary>
    /// 事物接口
    /// 
    /// 事物实现，依靠ef实现工作单元。
    /// </summary>
    public interface ITransaction
    {
        /// <summary>
        /// 获取当前事物
        /// </summary>
        /// <returns></returns>
        IDbContextTransaction GetCurrentTransaction();
        /// <summary>
        /// 当前是否是否开启
        /// </summary>
        bool HasActiveTransaction { get; }
        /// <summary>
        /// 开启事物
        /// </summary>
        /// <returns></returns>
        Task<IDbContextTransaction> BeginTransactionAsync();
        /// <summary>
        /// 提交事物
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task CommitTransactionAsync(IDbContextTransaction transaction);
        /// <summary>
        /// 回滚事物
        /// </summary>
        void RollbackTransaction();
    }
}
