using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class OrderRepository : SqlRepository<Order>, IOrderRepository
    {
        public OrderRepository(EcommerceContext context) : base(context)
        {
        }
    }
}
