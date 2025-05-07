using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Enums;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;

namespace Domain.Repositories
{
    public interface IOrderRepository : ISqlRepository<Order>
    {
        Task AddRangeOrderDetail(IEnumerable<OrderDetail> orderDetails);
    }
}
