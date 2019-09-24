using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using System.Data;
//using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.pbpimport
{
    public class Common
    {
        IUnitOfWorkAsync _unitOfWorkAsync = null;

        public Common(IUnitOfWorkAsync _unitOfWorkAsync)
        {
            this._unitOfWorkAsync = _unitOfWorkAsync;
        }
        public IEnumerable<PBPImportTablesViewModel> GetPBPImportTablesList()
        {
            IEnumerable<PBPImportTablesViewModel> objPBPImportQueueList = null;
            try
            {
                objPBPImportQueueList = (from c in this._unitOfWorkAsync.RepositoryAsync<PBPImportTables>()
                                                 .Query()
                                                 .OrderBy(c => c.OrderBy(d => d.PBPTableSequence))
                                                 .Get()
                                         select new PBPImportTablesViewModel
                                         {
                                             PBPTableID = c.PBPTableID,
                                             PBPTableName = c.PBPTableName,
                                             PBPTableSequence = c.PBPTableSequence,
                                             EBSTableName = c.EBSTableName
                                         }).ToList();
            }
            catch (Exception ex)
            {
                //bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                //if (reThrow)
                throw ex;
            }
            return objPBPImportQueueList;
        }
    }
}
