﻿using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Repositories.BaseRepositories;

namespace Domain.Repositories
{
    public interface IOrderDetailRepository : ISqlRepository<OrderDetail>
    {
    }
}
