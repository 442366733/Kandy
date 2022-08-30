using Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates;
using Kindy.DDDTemplate.Infrastructure.Context.Master.Repository;

namespace Kindy.DDDTemplate.Infrastructure.Repositories
{
    public interface IOrderRepository : IMasterRepository<Order, long>
    {
        void ChangeAddress(string address);
    }
}
