using System;
using tmg.equinox.integration.data;
using System.Collections.Generic;
using System.Data.Entity;

namespace tmg.equinox.integration.domain
{
    public interface IFacetUnitOfWork : IDisposable
    {
        void Dispose();
        void Save();
        void Dispose(bool disposing);
        IFacetRepository<T> Repository<T>() where T : Entity;
        void Clear<T>(IList<T> list);
        void Clear<T>(T item);
        void BulkInsert<T>(IList<T> list);
    }
}
