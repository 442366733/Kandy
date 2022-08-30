using System;
using System.Collections.Generic;
using System.Text;

namespace Kindy.DDDTemplate.Application.IntegrationEvents.Order
{
    public interface ISubscriberService
    {
        void OrderPaymentSucceeded(OrderPaymentSucceededIntegrationEvent @event);
    }
}
