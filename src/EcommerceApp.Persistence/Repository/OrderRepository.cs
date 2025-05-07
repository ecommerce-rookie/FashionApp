using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Order>> GetOrderCustomer(Guid customerId)
        {
            return await _dbSet
                .Where(o => o.CustomerId.Equals(customerId))
                .Include(o => o.OrderDetails!)
                    .ThenInclude(od => od.Product)
                    .ThenInclude(od => od.ImageProducts)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

    }
}
