using DotNetCore.CAP;
using Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates;
using Kindy.DDDTemplate.Infrastructure.EntityConfigurations.OrderEntiryConfiguration;
using Kindy.Infrastructure.Core.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kindy.DDDTemplate.Infrastructure.Context.Master
{
    /// <summary>
    /// EF上下文
    /// 该项目对应主库的上下文
    /// </summary>
    public class MasterDBContext : EFContext
    {
        public MasterDBContext(DbContextOptions<MasterDBContext> options, IMediator mediator, ICapPublisher capBus) : base(options, mediator, capBus)
        {
        }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderItem> OrderItem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //不用EF模型配置  看自己实际使用情况
            //不用EF模型配置  看自己实际使用情况

            //#region 注册领域模型与数据库的映射关系
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
            //#endregion
            //base.OnModelCreating(modelBuilder);
        }
    }
}
