using Application.Repositories;
using Domain.Contracts;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class ReadRepositoryAsync<T, Tid> : IReadRepositoryAsync<T, Tid> where T : BaseEntity<Tid>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReadRepositoryAsync<T, Tid>> _logger;
        public ReadRepositoryAsync(ApplicationDbContext context, ILogger<ReadRepositoryAsync<T, Tid>> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<T>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all entities of type {EntityType}", typeof(T).Name);

            var result = await _context.Set<T>().ToListAsync();

            _logger.LogInformation("Retrieved {Count} entities of type {EntityType}", result.Count, typeof(T).Name);
            return result;
        }

        public async Task<T> GetByIdAsync(Tid id)
        {
            _logger.LogInformation("Retrieving entity of type {EntityType} with Id {Id}", typeof(T).Name, id);

            var entity = await _context.Set<T>().FindAsync(id);

            if (entity is not null)
            {
                _logger.LogInformation("Entity found: {@Entity}", entity);
            }
            else
            {
                _logger.LogWarning("Entity of type {EntityType} with Id {Id} not found", typeof(T).Name, id);
            }

            return entity;
        }
    }
}
