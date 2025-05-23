﻿using EFCorePracticeAPI.Data;
using EFCorePracticeAPI.Repository.Interface;
using EFCorePracticeAPI.ViewModals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EFCorePracticeAPI.Repository.Implement
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return await Task.FromResult(entity);
        }

        public async Task<T> DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return await Task.FromResult(entity);
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.FirstOrDefaultAsync(expression);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.Where(expression).ToListAsync();
        }

        public async Task<PagedResult<T>> GetAllAsync(int pageNumber = 1,
                    int pageSize = 10,
                    Expression<Func<T, bool>> filter = null!,
                    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null!,
                    Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null!)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include(query);

            if (orderBy != null)
                query = orderBy(query);

            var totalRecords = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return null!;
            }
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return await Task.FromResult(entity);
        }

        Task<T> IGenericRepository<T>.FindAsync(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
