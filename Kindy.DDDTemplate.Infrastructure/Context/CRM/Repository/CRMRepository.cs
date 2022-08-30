using Kindy.DDDTemplate.Infrastructure.Context.CRM;
using Kindy.Domain.Abstractions.Aggregates;
using Kindy.Infrastructure.Core.Repository;

namespace Kindy.DDDTemplate.Infrastructure.Context.CRM.Repository
{
    public class CRMRepository<TEntity, TKey> : Repository<TEntity, TKey, CRMDBContext>, ICRMRepository<TEntity, TKey> where TEntity : Entity<TKey>, IAggregateRoot
    {
        public CRMRepository(CRMDBContext context) : base(context)
        {
        }
    }
}
