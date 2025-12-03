using Application.Repositories;
using Domain.Contracts;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    internal class ReadRepositoryAsync<T, Tid> : IReadRepositoryAsync<T, Tid> where T : BaseEntity<Tid>
    {
        private readonly ApplicationDbContext _context;
        public ReadRepositoryAsync(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Tid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
    }
}
