using System;
using System.Collections.Generic;
using System.Text;

namespace Kindy.Domain.Abstractions.Aggregates
{
    /// <summary>
    /// 实体基类接口
    /// </summary>
    public interface IEntity
    {
    }
    /// <summary>
    /// 包含id的实体接口
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IEntity<TKey> : IEntity
    {
        TKey id { get; }
    }
}
