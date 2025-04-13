﻿using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class ProductRepository : SqlRepository<Product>, IProductRepository
    {
        public ProductRepository(EcommerceContext context) : base(context)
        {
        }
    }
}
