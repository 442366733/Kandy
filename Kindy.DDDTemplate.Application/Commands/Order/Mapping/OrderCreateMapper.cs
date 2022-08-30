using AutoMapper;
using Kindy.DDDTemplate.Application.Commands.Order.Dtos;

namespace Kindy.DDDTemplate.Application.Commands.Order.Mapping
{
    public class OrderCreateMapper : Profile
    {
        public OrderCreateMapper()
        {
            CreateMap<CreateOrderCommand, Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates.Order>();
            CreateMap<OrderItemDto, Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates.OrderItem>();
        }
    }
}
