using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class OrderDetailRepository : SqlRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(EcommerceContext context) : base(context)
        {
        }
    }
}
