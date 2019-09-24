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
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.repository.models;

namespace tmg.equinox.applicationservices
{
    public partial class ReportMasterService : IReportMasterService
    {
        #region Private Members
        private IReportRepository _reportRepository { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public ReportMasterService(IReportRepository reportRepository)
        {
            this._reportRepository = reportRepository;
        }
        #endregion Constructor

        #region Public Methods
        public IList<ReportViewModel> GetReportList()
        {
            dynamic reportList = null;

            reportList = (from r in this._reportRepository.Get().Where(
                        r => r.Visible==true)
                          select new ReportViewModel
                          {
                              ReportId = r.ReportId,
                              ReportName = r.ReportName,
                          }).ToList();


            return reportList;
        }

        public ReportSetting GetReportByReportId(int reportId)
        {
            var result = this._reportRepository.Get().ToList().Where(r => r.ReportId.Equals(reportId)).FirstOrDefault();
            return result;
        }
        #endregion Public Methods
    }
}
