using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using tmg.equinox.applicationservices.FolderVersionDetail;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.infrastructure.util;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using System.Transactions;
using tmg.equinox.domain.entities.Utility;
using FolderVersionState = tmg.equinox.domain.entities.Enums.FolderVersionState;
using DocumentDesignTypes = tmg.equinox.domain.entities.Enums.DocumentDesignTypes;
using VersionType = tmg.equinox.domain.entities.Enums.VersionType;
using tmg.equinox.applicationservices.viewmodels.Settings;
using Newtonsoft.Json;
using tmg.equinox.applicationservices.viewmodels.EmailNotitication;
using System.Net.Mail;
using tmg.equinox.applicationservices.viewmodels.WCReport;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using tmg.equinox.applicationservices.viewmodels;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.domain.entities.Enums;

namespace tmg.equinox.applicationservices
{
    public class ReportQueueServices : IReportQueueServices
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private IFolderVersionServices _folderVersionService { get; set; }
        #endregion Private Memebers

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public ReportQueueServices(IUnitOfWorkAsync unitOfWork, IFolderVersionServices folderVersionService)
        {
            this._unitOfWork = unitOfWork;
            this._folderVersionService = folderVersionService;
        }
        #endregion Constructor

        #region Public Methods
        public IList<ReportQueueViewModel> GetReportQueueList()
        {
            dynamic reportQueuelist = null;
            try
            {

                reportQueuelist = (from rq in this._unitOfWork.RepositoryAsync<ReportQueue>()
                                       .Query()
                                       .Get()
                                   join r in this._unitOfWork.RepositoryAsync<Report>().Get() on rq.ReportId equals r.ReportId
                                   //join rd in this._unitOfWork.RepositoryAsync<ReportDetails>().Get() on rq.ReportQueueId  equals rd.ReportQueueId 
                                   select new ReportQueueViewModel
                                   {
                                       ReportName = r.ReportName,
                                       ReportQueueId = rq.ReportQueueId,
                                       ReportId = rq.ReportId,
                                       //FolderId = rd.FolderId,
                                       //FolderVersionId = rd.FolderVersionId,
                                       AddedBy = rq.AddedBy,
                                       AddedDate = rq.AddedDate,
                                       Status = rq.Status,
                                       CompletionDate = rq.CompletionDate,
                                       FileName = rq.FileName,
                                       DestinationPath = rq.DestinationPath,
                                   }).OrderByDescending(rq => rq.ReportQueueId).ToList();


            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return reportQueuelist;
        }

        public ServiceResult AddReportQueue(int ReportId, int[] FolderId, int[] FolderVersionId, string AddedBy, DateTime AddedDate, string Status)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                ReportQueue reportQueueToAdd = new ReportQueue();

                reportQueueToAdd.ReportId = ReportId;
                reportQueueToAdd.AddedBy = AddedBy;
                reportQueueToAdd.AddedDate = AddedDate;
                reportQueueToAdd.CompletionDate = DateTime.Now;
                reportQueueToAdd.Status = Status;


                this._unitOfWork.RepositoryAsync<ReportQueue>().Insert(reportQueueToAdd);
                this._unitOfWork.Save();
                int ReportQueueId = reportQueueToAdd.ReportQueueId;

                List<FormInstanceViewModel> formInstanceList = null;
                ReportQueueDetail reportDetails = new ReportQueueDetail();
                for (var i = 0; i < FolderId.Length; i++)
                {
                    reportDetails.ReportQueueId = ReportQueueId;
                    reportDetails.FolderId = FolderId[i];
                    reportDetails.FolderVersionId = FolderVersionId[i];

                    formInstanceList = this._folderVersionService.GetFormInstanceList(1, FolderVersionId[i], FolderId[i], 0);

                    foreach (FormInstanceViewModel FormInstance in formInstanceList)
                    {
                        reportDetails.FormInstanceId = FormInstance.FormInstanceID;
                        this._unitOfWork.RepositoryAsync<ReportQueueDetail>().Insert(reportDetails);
                        this._unitOfWork.Save();
                    }

                   
                }


                result.Result = ServiceResultStatus.Success;


            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        public ServiceResult DeleteReportQueue(int ReportQueueId)
        {
            ServiceResult service = null;
            return service;
        }

        public ServiceResult UpdateReportQueue(int ReportQueueId, ReportQueueViewModel model)
        {
            ServiceResult service = null;
            return service;
        }
        #endregion Public Methods
    }
}
