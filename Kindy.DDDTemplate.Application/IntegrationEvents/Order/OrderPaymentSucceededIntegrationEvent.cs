using System;
using System.Collections.Generic;
using System.Text;

namespace Kindy.DDDTemplate.Application.IntegrationEvents.Order
{
    public class OrderPaymentSucceededIntegrationEvent
    {
        public OrderPaymentSucceededIntegrationEvent(long orderId) => OrderId = orderId;
        public long OrderId { get; }
    }
}
