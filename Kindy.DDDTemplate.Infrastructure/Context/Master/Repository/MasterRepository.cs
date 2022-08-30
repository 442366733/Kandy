using Kindy.DDDTemplate.Infrastructure.Context.Master;
using Kindy.Domain.Abstractions.Aggregates;
using Kindy.Infrastructure.Core.Repository;

namespace Kindy.DDDTemplate.Infrastructure.Context.Master.Repository
{
    public class MasterRepository<TEntity, TKey> : Repository<TEntity, TKey, MasterDBContext>, IMasterRepository<TEntity, TKey> where TEntity : Entity<TKey>, IAggregateRoot
    {
        public MasterRepository(MasterDBContext context) : base(context)
        {
        }
    }
}