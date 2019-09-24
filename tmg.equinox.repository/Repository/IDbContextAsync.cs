using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tmg.equinox.repository
{
    /// <summary>
    /// VS:Addding a contract for having async implementation for EF context 
    /// pattern
    /// </summary>
    public interface IDbContextAsync:IDbContext    
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();
    }
}
