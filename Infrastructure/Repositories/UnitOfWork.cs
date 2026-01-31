using Application.Repositories;
using Domain.Contracts;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    internal class UnitOfWork<TId> : IUnitOfWork<TId>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UnitOfWork<TId>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private bool _disposed;
        private Hashtable _repositories;

        public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork<TId>>logger, IServiceProvider serviceProvider)
        {
            _context = context;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public async Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Committing changes to the database");
            var result = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Commit completed, {Count} changes saved", result);
            return result;
        }

        public IReadRepositoryAsync<T, TId> ReadRepositoryFor<T>() where T : BaseEntity<TId>
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }
            var type = $"{typeof(T).Name}_Read";
            _logger.LogInformation("Getting ReadRepository for {EntityType}", typeof(T).Name);
            //Console.WriteLine("typ: " + type);

            if (!_repositories.ContainsKey(type))
            {
                var repository = _serviceProvider.GetRequiredService<IReadRepositoryAsync<T, TId>>();
                _repositories.Add(type, repository);

            }
            return (IReadRepositoryAsync<T, TId>)_repositories[type];
        }

        public IWriteRepositoryAsync<T, TId> WriteRepositoryFor<T>() where T : BaseEntity<TId>
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }
            var type = $"{typeof(T).Name}_Write";
            _logger.LogInformation("Getting WriteRepository for {EntityType}", typeof(T).Name);

            if (!_repositories.ContainsKey(type))
            {
                var repository = _serviceProvider.GetRequiredService<IWriteRepositoryAsync<T, TId>>();
                _repositories.Add(type, repository);

            }
            return (IWriteRepositoryAsync<T, TId>)_repositories[type];
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _logger.LogInformation("Disposing UnitOfWork and DbContext");
                    _context.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
