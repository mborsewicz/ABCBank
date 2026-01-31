using Application.Repositories;
using Domain.Contracts;
using Infrastructure.Contexts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class WriteRepositoryAsync<T, TId> : IWriteRepositoryAsync<T, TId> where T : BaseEntity<TId>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WriteRepositoryAsync<T, TId>> _logger;

        public WriteRepositoryAsync(ApplicationDbContext context, ILogger<WriteRepositoryAsync<T, TId>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<T> AddAsync(T entity)
        {
            _logger.LogInformation("Adding entity {@Entity} to database", entity);
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public Task DeleteAsync(T entity)
        {
            _logger.LogInformation("Deleting entity {@Entity} from database", entity);
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _logger.LogInformation("Updating entity {@Entity} in database", entity);
            T entityInDb = await _context.Set<T>().FindAsync(entity.Id);
            _context.Entry(entityInDb).CurrentValues.SetValues(entity);
            return entity;
        }
    }
}
