using Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kindy.DDDTemplate.Infrastructure.EntityConfigurations.OrderEntiryConfiguration
{
    class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("t_order");
            builder.HasKey(p => p.id);
            builder.Property(p => p.OrderAmount).HasColumnName("order_amount");
            builder.Property(p => p.OrderCode).HasColumnName("order_code");
            builder.Property(p => p.Address).HasColumnName("address");
            builder.Property(p => p.CustomerId).HasColumnName("customer_id");
        }
    }
}
