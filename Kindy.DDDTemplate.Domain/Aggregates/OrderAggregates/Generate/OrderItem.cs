using Kindy.Domain.Abstractions.Aggregates;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates
{
    [Table("t_order_item")]
    public partial class OrderItem : Entity<long>
    {
        [Column("sku_code")]
        public string SkuCode { get; set; }

        [Column("order_id")]
        public long OrderID { get; set; }

        [Column("sku_number")]
        public int? SkuNumber { get; set; }
    }
}
