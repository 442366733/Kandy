using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kindy.Domain.Abstractions.Events
{
    /// <summary>
    /// 领域事件
    /// 标记对象是否未领域事件
    /// 
    /// 
    /// INotification  MediatR的，实现事件传递的
    /// </summary>
    public interface IDomainEvent : INotification
    {
    }
}
