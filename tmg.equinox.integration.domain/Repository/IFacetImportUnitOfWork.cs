using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.domain
{
    public interface IFacetImportUnitOfWork : IDisposable
    {
        void Dispose();
        void Save();
        void Dispose(bool disposing);
        IFacetImportRepository<T> Repository<T>() where T : Entity;
    }
}
