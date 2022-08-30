using Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates;
using Kindy.DDDTemplate.Infrastructure.Context.CRM.Repository;

namespace Kindy.DDDTemplate.Infrastructure.Repositories
{
    public interface ICrmOrderRepository : ICRMRepository<Order, long>
    {
    }
}
