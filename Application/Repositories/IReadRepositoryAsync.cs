using Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repositories
{
    public interface IReadRepositoryAsync<T, in TId> where T : class, IEntity<TId>
    {
        Task<T> GetByIdAsync(TId id);
        Task<List<T>> GetAllAsync();
        IQueryable<T> Entities { get; }
    }
}
