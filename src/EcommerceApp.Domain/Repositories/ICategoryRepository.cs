﻿using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Repositories.BaseRepositories;

namespace Domain.Repositories
{
    public interface ICategoryRepository : ISqlRepository<Category>
    {
        Task<bool> CheckCategoryExist(int categoryId);
    }
}
