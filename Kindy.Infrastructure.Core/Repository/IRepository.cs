using Kindy.Domain.Abstractions.Aggregates;
using Kindy.Infrastructure.Core.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kindy.Infrastructure.Core.Repository
{
    /// <summary>
    /// 未指定ID仓库接口
    /// 
    /// 聚合根才有仓储
    /// 
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> : IDisposable where TEntity : Entity, IAggregateRoot
    {
        /// <summary>
        /// 仓库里面要有工作单元
        /// </summary>
        IUnitOfWork UnitOfWork { get; }

        ITransaction Transaction { get; }

        #region 对应实体的  增删改
        TEntity Add(TEntity entity);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        TEntity Update(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        bool Remove(Entity entity);
        Task<bool> RemoveAsync(Entity entity);
        #endregion
    }

    /// <summary>
    /// 定义了id的仓库
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : Entity<TKey>, IAggregateRoot
    {
        bool Delete(TKey id);
        Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
        TEntity Get(TKey id);
        Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);
    }
}
