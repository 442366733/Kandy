using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kindy.Domain.Abstractions.Events
{
    /// <summary>
    /// 领域事件处理接口
    /// 
    /// 
    /// 只处理  集成IDomainEvent类型事件。领域事件的处理在应用层完成。
    /// </summary>
    public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        //这里我们使用了INotificationHandler的Handle方法来作为处理方法的定义
        //Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken);
    }
}
