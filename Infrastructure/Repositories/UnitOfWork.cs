using Application.Repositories;
using Domain.Contracts;
using Infrastructure.Contexts;
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
        private bool _disposed;
        private Hashtable _repositories;

        public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork<TId>>logger)
        {
            _context = context;
            _logger = logger;
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
                var repositoryType = typeof(ReadRepositoryAsync<,>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T), typeof(TId)), _context);
                _repositories.Add(type, repositoryInstance);

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
                var repositoryType = typeof(WriteRepositoryAsync<,>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T), typeof(TId)), _context);
                _repositories.Add(type, repositoryInstance);

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
