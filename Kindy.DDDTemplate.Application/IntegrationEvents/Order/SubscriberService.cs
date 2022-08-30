using DotNetCore.CAP;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kindy.DDDTemplate.Application.IntegrationEvents.Order
{
    /// <summary>
    /// 订阅其他微服务的集成事件
    /// </summary>
    public class SubscriberService : ISubscriberService, ICapSubscribe
    {
        IMediator _mediator;
        public SubscriberService(IMediator mediator)
        {
            _mediator = mediator;
        }


        [CapSubscribe("OrderPaymentSucceeded")]
        public void OrderPaymentSucceeded(OrderPaymentSucceededIntegrationEvent @event)
        {
            //Do SomeThing
        }

        [CapSubscribe("OrderCreated")]
        public void OrderCreated(OrderCreatedIntegrationEvent @event)
        {

            Console.WriteLine(GetHashCode());


            //Do SomeThing
        }
    }
}
