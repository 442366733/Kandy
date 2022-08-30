using Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates;
using Kindy.DDDTemplate.Infrastructure.Context.Master;
using Kindy.DDDTemplate.Infrastructure.Context.Master.Repository;

namespace Kindy.DDDTemplate.Infrastructure.Repositories.Impl
{
    public class OrderRepository : MasterRepository<Order, long>, IOrderRepository
    {
        public OrderRepository(MasterDBContext context) : base(context)
        {
        }

        public void ChangeAddress(string address)
        {
            throw new System.NotImplementedException();
        }
    }
}
