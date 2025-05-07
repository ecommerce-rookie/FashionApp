using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Repositories.BaseRepositories;

namespace Domain.Repositories
{
    public interface IOrderRepository : ISqlRepository<Order>
    {
        Task AddRangeOrderDetail(IEnumerable<OrderDetail> orderDetails);
        Task<IEnumerable<Order>> GetOrderCustomer(Guid customerId);
    }
}
