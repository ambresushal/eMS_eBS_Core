using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.infrastructure.util;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using System.Transactions;
using tmg.equinox.domain.entities.Utility;
using tmg.equinox.applicationservices.viewmodels.WCReport;
using System.Text.RegularExpressions;
using tmg.equinox.applicationservices.viewmodels;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;


namespace tmg.equinox.applicationservices
{
    public partial class ReportMasterService : IReportMasterService
    {
        #region Private Members
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public ReportMasterService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        #region Public Methods
        public IList<ReportViewModel> GetReportList()
        {
            dynamic reportList = null;
            try
            {
                reportList = (from r in this._unitOfWork.RepositoryAsync<Report>()
                                       .Query()
                                       .Get()
                              select new ReportViewModel
                              {
                                  ReportId = r.ReportId,
                                  ReportName = r.ReportName,
                              }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return reportList;
        }

        //public ReportViewModel GetReportByReportId(int reportID)
        //{

        //}
        #endregion Public Methods
    }
}
