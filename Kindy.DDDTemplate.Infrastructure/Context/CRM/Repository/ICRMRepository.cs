using Kindy.Domain.Abstractions.Aggregates;
using Kindy.Infrastructure.Core.Repository;

namespace Kindy.DDDTemplate.Infrastructure.Context.CRM.Repository
{
    public interface ICRMRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : Entity<TKey>, IAggregateRoot
    {
    }
}
