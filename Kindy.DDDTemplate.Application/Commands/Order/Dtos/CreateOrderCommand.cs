using MediatR;
using System.Collections.Generic;

namespace Kindy.DDDTemplate.Application.Commands.Order.Dtos
{
    public class CreateOrderCommand : IRequest<long>
    {
        public CreateOrderCommand(decimal orderAmount)
        {
            OrderAmount = orderAmount;
        }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderAmount { get; set; }
        /// <summary>
        /// 客户id
        /// </summary>
        public int CustomerId { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 订单商品明细
        /// </summary>
        public OrderItemDto OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string SkuCode { get; set; }
        /// <summary>
        /// 库存号
        /// </summary>
        public int SkuNumber { get; set; }
    }
}
