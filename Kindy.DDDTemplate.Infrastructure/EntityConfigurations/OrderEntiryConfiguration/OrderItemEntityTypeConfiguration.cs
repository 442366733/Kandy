using Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kindy.DDDTemplate.Infrastructure.EntityConfigurations.OrderEntiryConfiguration
{
    class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("t_order_item");
            builder.HasKey(p => p.id);
            builder.Property(p => p.SkuCode).HasColumnName("sku_code");
            builder.Property(p => p.OrderID).HasColumnName("order_id");
            builder.Property(p => p.SkuNumber).HasColumnName("sku_number");
        }
    }
}
