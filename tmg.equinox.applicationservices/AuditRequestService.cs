using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using System.Diagnostics.Contracts;
using System.Transactions;
using System.Text.RegularExpressions;
using tmg.equinox.repository.extensions;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.applicationservices
{
    public class AuditRequestService : IAuditRequestService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private delegate ServiceResult AuditRequestDelegate(string userName, string ipAddress, string areaAccessed, DateTime timeAccesed);
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public AuditRequestService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        #region Public Methods
       

        public ServiceResult AddAuditData(string userName, string ipAddress, string areaAccessed, DateTime timeAccesed)
        {           
            ServiceResult result = new ServiceResult();
            AuditRequestDelegate auditDelegate = LogAuditData;
            try
            {                
                IAsyncResult rslt = auditDelegate.BeginInvoke(userName,ipAddress,areaAccessed,timeAccesed,null,null);
                //ServiceResult res = auditDelegate.EndInvoke(rslt);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;

        }

       
        #endregion Public Methods

        #region Private Methods

        private ServiceResult LogAuditData(string userName, string ipAddress, string areaAccessed, DateTime timeAccesed)
        {
            //TO DO : Call the Audit respository here
            return new ServiceResult();
        }
        #endregion Private Methods
    }
}
