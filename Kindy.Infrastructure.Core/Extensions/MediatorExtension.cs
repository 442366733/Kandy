using Kindy.Domain.Abstractions.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Kindy.Infrastructure.Core.Extensions
{
    static class MediatorExtension
    {
        /// <summary>
        /// 事物提交后
        /// 发送领域事件
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext ctx)
        {
            ///跟踪的实体
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());
            //取出实体领域事件
            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();
            //清空实体领域事件
            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            //中介者发出去。将领域事件发出，找到对应的handler处理
            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}
