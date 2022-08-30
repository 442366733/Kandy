using Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates;
using Kindy.Domain.Abstractions.Events;

namespace Kindy.DDDTemplate.Domain.Events.OrderEvents
{
    public class OrderCreatedDomainEvent : IDomainEvent
    {
        public Order Order { get; private set; }
        public OrderCreatedDomainEvent(Order order)
        {
            Order = order;
        }
    }
}
