using AutoMapper;
using DotNetCore.CAP;
using Kindy.DDDTemplate.Application.Commands.Order.Dtos;
using Kindy.DDDTemplate.Infrastructure.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kindy.DDDTemplate.Application.Commands.Order.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, long>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICapPublisher _capPublisher;
        private readonly IMapper _mapper;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, ICapPublisher capPublisher, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _capPublisher = capPublisher;
            _mapper = mapper;
        }


        public async Task<long> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = _mapper.Map<Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates.Order>(request);
            var orderItem = _mapper.Map<Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates.OrderItem>(request.OrderItems);
            order.AddOrderItem(orderItem);
            _orderRepository.Add(order);
            var re = _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken).Result;
            return order.id;
        }
    }
}
