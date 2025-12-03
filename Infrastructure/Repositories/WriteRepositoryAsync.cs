using Application.Repositories;
using Domain.Contracts;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class WriteRepositoryAsync<T, TId> : IWriteRepositoryAsync<T, TId> where T : BaseEntity<TId>
    {
        private readonly ApplicationDbContext _context;
        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            T entityInDb = await _context.Set<T>().FindAsync(entity.Id);
            _context.Entry(entityInDb).CurrentValues.SetValues(entity);
            return entity;
        }
    }
}
