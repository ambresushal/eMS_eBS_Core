using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tmg.equinox.domain.entities;
using tmg.equinox.repository.Base;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.email
{
   
    /// <summary>
    /// Concrete implementation of contract for unit of work
    /// </summary>
    public class EmailUnitOfWork : BaseUnitOfWork, IUnitOfWorkAsync
    {
        #region Private Members

        private readonly IDbContextAsync _context;
        /* private bool _disposed;
         private Hashtable _repositories;
         private Hashtable _repositoriesAsync;*/
        #endregion Private Members

     

        public EmailUnitOfWork()
        {
            _context = new EmailContext();
            SetContext(_context);
        }
        /*
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }       
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();          

            _disposed = true;
        }
        #endregion Constructor
        */
        /* #region Public Methods
         public void Save()
         {
             _context.SaveChanges();
         }
         public IRepository<T> Repository<T>() where T : Entity
         {
             if (_repositories == null)
                 _repositories = new Hashtable();

             var type = typeof(T).Name;

             if (!_repositories.ContainsKey(type))
             {
                 var repositoryType = typeof(Repository<>);

                 var repositoryInstance =
                     Activator.CreateInstance(repositoryType
                             .MakeGenericType(typeof(T)), _context);

                 _repositories.Add(type, repositoryInstance);
             }

             return (IRepository<T>)_repositories[type];
         }

         public Task<int> SaveChangesAsync()
         {
             return _context.SaveChangesAsync();
         }

         public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
         {
             return _context.SaveChangesAsync(cancellationToken);
         }

         public IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : Entity
         {
             if (_repositoriesAsync == null)
                 _repositoriesAsync = new Hashtable();

             var type = typeof(TEntity).Name;

             if (!_repositoriesAsync.ContainsKey(type))
             {
                 var repositoryType = typeof(Repository<>);

                 var repositoryInstance =
                     Activator.CreateInstance(repositoryType
                             .MakeGenericType(typeof(TEntity)), _context);

                 _repositoriesAsync.Add(type, repositoryInstance);
             }

             return (IRepositoryAsync<TEntity>)_repositoriesAsync[type];
         }

         public void Clear<T>(T item)
         {
             try
             {
                 _context.Entry(item).State = System.Data.Entity.EntityState.Detached;
             }
             catch (Exception ex)
             {
                 string message = ex.Message;
             }
         }
         #endregion Public Methods

         #region Private Methods
         #endregion Private Methods */   
    }
}
