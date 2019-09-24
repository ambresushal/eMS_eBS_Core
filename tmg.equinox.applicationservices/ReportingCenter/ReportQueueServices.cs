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
using tmg.equinox.queueprocess.reporting;
using tmg.equinox.backgroundjob;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.repository;
//using tmg.equinox.reporting;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.applicationservices.viewmodels.ReportingCenter;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.reporting;
using tmg.equinox.applicationservices.ReportingCenter;
using tmg.equinox.repository.models;

namespace tmg.equinox.applicationservices
{
    public class ReportQueueServices : IReportQueueServices
    {
        #region Private Memebers
        private IRptUnitOfWorkAsync _unitOfWork { get; set; }
        private IFolderVersionServices _folderVersionService { get; set; }
        private IReportRepository _reportRepository { get; set; }
        private static readonly ILog _logger = LogProvider.For<ReportQueueServices>();
        IBackgroundJobManager _hangFireJobManager;
        //private readonly IBackgroundJobManager _backgroundJobManager;
        #endregion Private Memebers

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public ReportQueueServices(IRptUnitOfWorkAsync unitOfWork, IFolderVersionServices folderVersionService, IReportRepository reportRepository, IBackgroundJobManager hangFireJobManager)
        {
            this._unitOfWork = unitOfWork;
            this._folderVersionService = folderVersionService;
            this._reportRepository = reportRepository;
            _hangFireJobManager = hangFireJobManager;
        }

        public ReportQueueServices(IRptUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
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
                                   join r in this._unitOfWork.RepositoryAsync<ReportSetting>().Get() on rq.ReportId equals r.ReportId
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
                                       //JobId = rq.JobId
                                       ErrorMessage = rq.ErrorMessage
                                   }).OrderByDescending(rq => rq.ReportQueueId).ToList();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("GetReportQueueList", ex);
                throw ex;
            }

            return reportQueuelist;
        }

        public List<ReportQueueDetailsViewModel> GetReportQueueFolderDetailsList(int queueId)
        {
            List<ReportQueueDetailsViewModel> reportQueueFolderlist = (from rq in this._unitOfWork.RepositoryAsync<ReportQueueDetail>()
                                   .Get().Where(x => x.ReportQueueId == queueId)
                                                                       group rq by new { rq.ReportQueueId, rq.FolderId, rq.FolderVersionId, } into gcs
                                                                       select new ReportQueueDetailsViewModel()
                                                                       {
                                                                           ReportQueueId = gcs.Key.ReportQueueId,
                                                                           FolderId = gcs.Key.FolderId,
                                                                           FolderVersionId = gcs.Key.FolderVersionId,
                                                                       }).ToList();
            return reportQueueFolderlist;
        }

        public GridPagingResponse<ReportQueueViewModel> GetReportQueueList(GridPagingRequest gridPagingRequest)
        {
            List<ReportQueueViewModel> reportQueuelist = null;
            List<ReportQueueViewModel> formattedReportQueueList = null;
            int count = 0;
            try
            {
                if (gridPagingRequest.rows != null)
                {
                    SearchCriteria criteria = new SearchCriteria();
                    criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                    reportQueuelist = (from rq in this._unitOfWork.RepositoryAsync<ReportQueue>()
                                       .Query()
                                       .Get()
                                       join r in this._unitOfWork.RepositoryAsync<ReportSetting>().Get() on rq.ReportId equals r.ReportId
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
                                           //JobId = rq.JobId
                                           ErrorMessage = rq.ErrorMessage
                                       }).OrderByDescending(rq => rq.ReportQueueId).ToList().ApplySearchCriteria(criteria)
                                           .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                           .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count)
                                           .ToList();

                    formattedReportQueueList = (from c in reportQueuelist
                                                select new ReportQueueViewModel
                                                {
                                                    ReportName = c.ReportName,
                                                    ReportQueueId = c.ReportQueueId,
                                                    ReportId = c.ReportId,
                                                    AddedBy = c.AddedBy,
                                                    AddedDate = c.AddedDate,
                                                    Status = c.Status,
                                                    CompletionDate = c.CompletionDate,
                                                    FileName = c.FileName,
                                                    DestinationPath = c.DestinationPath,
                                                    ErrorMessage = c.ErrorMessage
                                                }).ToList();

                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("GetReportQueueList", ex);
                throw ex;
            }

            //return reportQueuelist;
            return new GridPagingResponse<ReportQueueViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, formattedReportQueueList);

        }

        public List<FormInstanceViewModel> GetFormInstanceDetails(List<int> formInstanceIDs)
        {
            List<FormInstanceViewModel> formInstanceList = null;
            formInstanceList = this._folderVersionService.getFormInstanceDataList(formInstanceIDs);
            return formInstanceList;
        }

        public ServiceResult AddReportQueue(int ReportId, int[] FolderId, int[] FolderVersionId, int[] FormInstanceId, string AddedBy, int UserId, DateTime AddedDate, string Status)
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

                ReportQueueDetail reportDetails = new ReportQueueDetail();
                for (var i = 0; i < FolderId.Length; i++)
                {
                    reportDetails.ReportQueueId = ReportQueueId;
                    reportDetails.FolderId = FolderId[i];
                    reportDetails.FolderVersionId = FolderVersionId[i];
                    reportDetails.FormInstanceId = FormInstanceId[i];
                    this._unitOfWork.RepositoryAsync<ReportQueueDetail>().Insert(reportDetails);
                    this._unitOfWork.Save();
                }

                //Add to Hangfire Queue
                ReportEnqueue reportEnqueue = new ReportEnqueue(_hangFireJobManager);

                ReportSettingViewModel report = GetReportSetting(ReportId);

                reportEnqueue.Enqueue(new ReportQueueInfo { QueueId = ReportQueueId, UserId = AddedBy, FeatureId = ReportId.ToString(), Name = report.ReportName, AssemblyName = "tmg.equinox.applicationservices", ClassName = "ReportCustomQueue" });

                result.Result = ServiceResultStatus.Success;


            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                _logger.ErrorException("AddReportQueue", ex);
                throw ex;
            }
            return result;
        }



        public ServiceResult AddReportQueue(int ReportId, int[] FolderId, int[] FolderVersionId, string AddedBy, int UserId, DateTime AddedDate, string Status)
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

                //Add to Hangfire Queue
                ReportEnqueue reportEnqueue = new ReportEnqueue(_hangFireJobManager);

                ReportSettingViewModel report = GetReportSetting(ReportId);

                reportEnqueue.Enqueue(new ReportQueueInfo { QueueId = ReportQueueId, UserId = AddedBy, FeatureId = ReportId.ToString(), Name = report.ReportName, AssemblyName = "tmg.equinox.applicationservices", ClassName = "ReportCustomQueue" });

                result.Result = ServiceResultStatus.Success;


            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                _logger.ErrorException("AddReportQueue", ex);
                throw ex;
            }
            return result;
        }

        public ReportSettingViewModel GetReportSetting(int ReportId)
        {
            ReportSettingViewModel reportSetting = null;
            if (_unitOfWork == null)
                _unitOfWork = new RptUnitOfWork();
            reportSetting = (from r in this._unitOfWork.RepositoryAsync<ReportSetting>().Query().Get().Where(r => r.ReportId.Equals(ReportId))
                             select new ReportSettingViewModel
                             {
                                 ReportId = r.ReportId,
                                 ReportName = r.ReportName,
                                 ReportTemplatePath = r.ReportTemplatePath,
                                 OutputPath = r.OutputPath,
                                 ReportNameFormat = r.ReportNameFormat,
                                 Description = r.Description,
                                 SQLstatement = r.SQLstatement.Replace(@"\r\n", ""),
                                 AddedBy = r.AddedDate.HasValue ? r.AddedDate.Value.ToString() : new DateTime().ToString()
                             }).FirstOrDefault();
            return reportSetting;
        }

        public ServiceResult DeleteReportQueue(int ReportQueueId)
        {
            ServiceResult service = null;
            return service;
        }

        public ServiceResult UpdateReportQueue(int ReportQueueId, ReportQueueViewModel model)
        {
            ServiceResult result = new ServiceResult();
            try
            {

                ReportQueue rq = this._unitOfWork.RepositoryAsync<ReportQueue>().Query().Filter(x => x.ReportQueueId == model.ReportQueueId).Get().FirstOrDefault();

                rq.JobId = (model.JobId == 0 ? rq.JobId : model.JobId);
                rq.Status = model.Status;
                rq.FileName = (model.FileName == null ? rq.FileName : model.FileName);
                rq.DestinationPath = (model.DestinationPath == null ? rq.DestinationPath : model.DestinationPath);
                rq.ErrorMessage = model.ErrorMessage;

                this._unitOfWork.RepositoryAsync<ReportQueue>().Update(rq);
                this._unitOfWork.Save();

                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                //  bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                _logger.ErrorException("UpdateReportQueue", ex);
                throw ex;
            }
            return result;
        }
        #endregion Public Methods
    }



}
