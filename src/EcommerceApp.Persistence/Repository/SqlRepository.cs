﻿using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Repository
{
    public class SqlRepository<T> : ISqlRepository<T> where T : class
    {
        private readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public SqlRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        //Base Repository
        public async Task<T?> GetById(dynamic id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetById(dynamic[] id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[]? includeProperties)
        {
            var query = _dbSet
                .Where(predicate)
                .AsQueryable();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRange(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task Delete(dynamic id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return;
            }
            _dbSet.Remove(entity);
        }

        public async Task Delete(dynamic[] id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return;
            }
            _dbSet.Remove(entity);
        }

        public async Task Delete(T entity)
        {
            _dbSet.Remove(entity);

            await Task.CompletedTask;
        }

        public Task Update(T entity)
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
                entry.State = EntityState.Modified;
            }

            return Task.CompletedTask;
        }

        public Task UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);

            return Task.CompletedTask;
        }

        //Custom Repository Get All 

        public async Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[]? includeProperties)
        {
            var query = _dbSet.AsQueryable();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[]? includeProperties)
        {
            var query = _dbSet
                .Where(predicate)
                .AsQueryable();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<PagedList<T>> GetAll(int page, int eachPage, params Expression<Func<T, object>>[]? includeProperties)
        {
            var query = _dbSet.AsQueryable();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToPagedListAsync(page, eachPage);
        }

        public async Task<PagedList<T>> GetAll(Expression<Func<T, bool>> predicate,
                                            int page, int eachPage,
                                            params Expression<Func<T, object>>[]? includeProperties)
        {
            var query = _dbSet
                .Where(predicate)
                .AsQueryable();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToPagedListAsync(page, eachPage);
        }

        public async Task<PagedList<T>> GetAll(int page, int eachPage,
                                            string sortBy, bool isAscending = false,
                                            params Expression<Func<T, object>>[]? includeProperties)
        {
            var query = _dbSet.AsQueryable();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            var result = query.ToPaginateAndSort(page, eachPage, sortBy, isAscending);

            return await Task.FromResult(result);

        }

        public async Task<PagedList<T>> GetAll(Expression<Func<T, bool>> predicate,
                                                int page, int eachPage,
                                                string sortBy, bool isAscending = true,
                                                params Expression<Func<T, object>>[]? includeProperties)
        {
            var query = _dbSet
                .Where(predicate)
                .AsQueryable()
                .AsNoTracking()
                .AsSplitQuery();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            var result = query.ToPaginateAndSort(page, eachPage, sortBy, isAscending);

            return await Task.FromResult(result);

        }

        public async Task<PagedList<TResult>> GetAll<TResult>(Expression<Func<T, bool>> predicate,
                                                Expression<Func<T, TResult>> selector,
                                                int page, int eachPage,
                                                string sortBy, bool isAscending = true,
                                                params Expression<Func<T, object>>[]? includeProperties) where TResult : class
        {
            var query = _dbSet
                .Where(predicate)
                .AsQueryable()
                .AsNoTracking()
                .AsSplitQuery();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            query = query.Sort(sortBy, isAscending);

            var result = await query.Select(selector).ToPagedListAsync(page, eachPage);

            return await Task.FromResult(result);

        }
    }
}
