using Kindy.Domain.Abstractions.Events;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kindy.Domain.Abstractions.Aggregates
{
    public abstract class Entity : IEntity
    {
        #region 
        /// <summary>
        /// 领域事件列表
        /// </summary>
        private List<IDomainEvent> _domainEvents;
        /// <summary>
        /// 领域时间要被外面读取到。所以这里定义个IReadOnlyCollection
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();
        /// <summary>
        /// 添加领域事件
        /// </summary>
        /// <param name="eventItem"></param>
        public void AddDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents = _domainEvents ?? new List<IDomainEvent>();
            _domainEvents.Add(eventItem);
        }
        /// <summary>
        /// 移除领域事件
        /// </summary>
        /// <param name="eventItem"></param>
        public void RemoveDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }
        /// <summary>
        /// 清空领域事件
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
        #endregion
    }

    public abstract class Entity<TKey> : Entity, IEntity<TKey>
    {
        #region 基础类型
        public int create_user_id { get; protected set; }

        public DateTime create_time { get; protected set; }

        public int update_user_id { get; protected set; }

        public int deleted { get; set; }

        public DateTime update_time { get; protected set; }
        public TKey id { get; protected set; }
        #endregion
    }
}
