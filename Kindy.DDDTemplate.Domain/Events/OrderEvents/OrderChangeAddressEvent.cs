using Kindy.Domain.Abstractions.Events;

namespace Kindy.DDDTemplate.Domain.Events.OrderEvents
{
    public class OrderChangeAddressEvent : IDomainEvent
    {
        public string Address { get; private set; }
        public OrderChangeAddressEvent(string address)
        {
            Address = address;
        }
    }
}
