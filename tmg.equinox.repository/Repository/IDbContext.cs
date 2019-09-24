using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.repository
{
    /// <summary>
    /// VS:Addding a contract for having cleaner implementation and as well as the cleaner UnitOfWork
    /// pattern
    /// </summary>
    public interface IDbContext
    {
        IQueryable<T> Table<T>() where T : class;//VS:needed for efcaching
        IDbSet<T> Set<T>() where T : class;
        int SaveChanges();
        DbEntityEntry Entry(object o);
        void Dispose();
    }
}
