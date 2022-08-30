using Kindy.Domain.Abstractions.Aggregates;
using Kindy.Infrastructure.Core.Repository;

namespace Kindy.DDDTemplate.Infrastructure.Context.Master.Repository
{
    public interface IMasterRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : Entity<TKey>, IAggregateRoot
    {
    }
}
