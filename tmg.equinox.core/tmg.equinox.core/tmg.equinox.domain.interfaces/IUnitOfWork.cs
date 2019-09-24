using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities;

namespace tmg.equinox.repository.interfaces
{
    /// <summary>
    /// VS: Adding the contract for unitofwork
    /// </summary>
    public interface IUnitOfWork:IDisposable
    {
        void Dispose();
        void Save();
        void Dispose(bool disposing);
        IRepository<T> Repository<T>() where T : Entity;
    }
}
