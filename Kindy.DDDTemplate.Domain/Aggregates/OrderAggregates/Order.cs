using Kindy.DDDTemplate.Domain.Events.OrderEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates
{
    public partial class Order
    {
        /// <summary>
        /// 具有业务含义的动作来操作模型字段
        /// 区分领域模型的内在动作。外在动作。
        /// 
        /// 。
        /// 领域模型只负责自己数据的处理
        /// 。
        /// 
        /// 领域服务或者命令矗立着  负责调用领域模型业务动作。
        /// </summary>
        /// <param name="address"></param>
        public void ChangeAddress(string address)
        {
            this.Address = address;

            //领域事件的添加修改，应该是在领域模型内部完成。
            this.AddDomainEvent(new OrderChangeAddressEvent(this.Address));
        }

        public void Init()
        {
            this.update_user_id = this.create_user_id = 1;
            this.update_time = this.create_time = DateTime.Now;
            this.deleted = 0;
        }

        public void AddOrderItem(OrderItem orderItem)
        {
            if (this.OrderItem == null)
            {
                this.OrderItem = new List<OrderItem>();
            }
            orderItem.OrderID = this.id;

            OrderCreatedDomainEvent orderCreated = new OrderCreatedDomainEvent(this);
            this.AddDomainEvent(orderCreated);
            this.OrderItem.Add(orderItem);
        }
    }
}
