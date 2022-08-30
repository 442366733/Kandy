using Kindy.Domain.Abstractions.Aggregates;
using SqlSugar;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates
{
    /// <summary>
    /// 
    /// </summary>
    [Table("t_order")]
    public partial class Order : Entity<long>, IAggregateRoot
    {
        /// <summary>
        /// private 封闭开放原则
        /// </summary>
        [Column("order_code")]
        public string OrderCode { get; private set; }

        [Column("order_amount")]
        public decimal OrderAmount { get; private set; }

        [Column("address")]
        public string Address { get; private set; }

        [Column("customer_id")]
        public string CustomerId { get; set; }

        #region 导航属性
        [SugarColumn(IsIgnore = true)]
        public List<OrderItem> OrderItem { get; private set; }
        #endregion
    }
}
