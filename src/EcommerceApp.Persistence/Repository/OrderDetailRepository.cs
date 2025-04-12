using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repository
{
    public class OrderDetailRepository : SqlRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(DbContext context) : base(context)
        {
        }
    }
}
