using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository
{
    public class RptUnitOfWork : Base.BaseUnitOfWork, IRptUnitOfWorkAsync
    {
        #region Private Members

        private readonly Base.IDbContextAsync _context;
        /* private bool _disposed;
         private Hashtable _repositories;
         private Hashtable _repositoriesAsync;*/
        #endregion Private Members



        public RptUnitOfWork()
        {
            _context = new ReportingCenterContext();
            SetContext(_context);
        }

    }
}
