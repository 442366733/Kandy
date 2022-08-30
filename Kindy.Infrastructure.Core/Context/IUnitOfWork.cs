using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kindy.Infrastructure.Core.Context
{
    /// <summary>
    /// 工作单元，负责事物提提交。
    /// 
    /// 借助EF实现工作单元
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>影响条数</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>是否保存成功</returns>
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }
}
