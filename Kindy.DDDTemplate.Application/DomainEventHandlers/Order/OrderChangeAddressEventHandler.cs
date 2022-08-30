using DotNetCore.CAP;
using Kindy.Domain.Abstractions;
using Kindy.DDDTemplate.Domain.Events.OrderEvents;
using Kindy.Domain.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kindy.DDDTemplate.Application.DomainEventHandlers.Order
{
    public class OrderChangeAddressEventHandler : IDomainEventHandler<OrderChangeAddressEvent>
    {
        ICapPublisher _capPublisher;
        public OrderChangeAddressEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        /// <summary>
        /// 当我们修改地址时候。我们向nsb发布一个事件。
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Handle(OrderChangeAddressEvent notification, CancellationToken cancellationToken)
        {
            await _capPublisher.PublishAsync("OrderChangeAddress", new OrderChangeAddressEvent(notification.Address));
        }
    }
}
