using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.domain
{
    /// <summary>
    /// VS: Provides the additional contracts for employing asynchronous operations 
    /// </summary>
    public interface IRepositoryAsync<TEntity>:IRepository<TEntity> where TEntity : Entity
    {
        Task<TEntity> FindAsync(params object[] keyValues);
        Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);
        Task<bool> DeleteAsync(params object[] keyValues);
        Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues);
    }
}
