using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tmg.equinox.domain.entities;

namespace tmg.equinox.repository.interfaces
{
    /// <summary>
    /// VS: Provides the contract for employing asynchronous operations through unit of work
    /// </summary>
    public interface IUnitOfWorkAsync : IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : Entity;
        void Clear<T>(T item);
    }

    public interface ICoreUnitOfWorkAsync : IUnitOfWorkAsync
    {
     
    }
}
