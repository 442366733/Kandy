using Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates;
using Kindy.DDDTemplate.Infrastructure.Context.CRM;
using Kindy.DDDTemplate.Infrastructure.Context.CRM.Repository;
using Kindy.DDDTemplate.Infrastructure.Repositories;

namespace Kindy.DDDTemplate.Infrastructure.Repositories.Impl
{
    public class CrmOrderRepository : CRMRepository<Order, long>, ICrmOrderRepository
    {
        public CrmOrderRepository(CRMDBContext context) : base(context)
        {
        }
    }
}
