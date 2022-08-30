using System;
using System.Collections.Generic;
using System.Text;

namespace Kindy.DDDTemplate.Application.IntegrationEvents.Order
{
    public class OrderCreatedIntegrationEvent
    {
        public OrderCreatedIntegrationEvent(long orderId) => OrderId = orderId;
        public long OrderId { get; }
    }
}
