using DotNetCore.CAP;
using Kindy.DDDTemplate.Application.IntegrationEvents;
using Kindy.Domain.Abstractions;
using Kindy.DDDTemplate.Domain.Events.OrderEvents;
using Kindy.Domain.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kindy.DDDTemplate.Application.IntegrationEvents.Order;

namespace Kindy.DDDTemplate.Application.DomainEventHandlers.Order
{
    /// <summary>
    /// 领域事件把集成事件发布出去
    /// </summary>
    public class OrderCreatedDomainEventHandler : IDomainEventHandler<OrderCreatedDomainEvent>
    {
        ICapPublisher _capPublisher;
        public OrderCreatedDomainEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            //发布集成事件
            await _capPublisher.PublishAsync("OrderCreated", new OrderCreatedIntegrationEvent(notification.Order.id));
        }
    }
}
