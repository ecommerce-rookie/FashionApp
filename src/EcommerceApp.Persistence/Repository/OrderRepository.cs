using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Repositories;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class OrderRepository : SqlRepository<Order>, IOrderRepository
    {
        private readonly EcommerceContext _context;

        public OrderRepository(EcommerceContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRangeOrderDetail(IEnumerable<OrderDetail> orderDetails)
        {
            await _context.OrderDetails.AddRangeAsync(orderDetails);
        }
    }
}
