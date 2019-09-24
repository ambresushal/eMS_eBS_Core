using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Infrastructure;

namespace tmg.equinox.integration.domain
{
    public interface IDbContext
    {
        IQueryable<T> Table<T>() where T : class;
        IDbSet<T> Set<T>() where T : class;
        int SaveChanges();
        DbEntityEntry Entry(object o);
        void Dispose();
    }
}
