using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.repository.Base;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository
{
    public class CoreRptUnitOfWork : BaseUnitOfWork, IRptUnitOfWorkAsync
    {
        #region Private Members

        private readonly IDbContextAsync _context;
        /* private bool _disposed;
         private Hashtable _repositories;
         private Hashtable _repositoriesAsync;*/
        #endregion Private Members



        public CoreRptUnitOfWork()
        {
            _context = new BaseReportingCenterContext();
            SetContext(_context);
        }
    
    }
}
