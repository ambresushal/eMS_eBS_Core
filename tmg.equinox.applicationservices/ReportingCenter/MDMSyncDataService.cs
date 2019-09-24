using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ReportingCenter;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.extensions;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class MDMSyncDataService : IMDMSyncDataService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private static readonly Object _updateMDMStatusLock = new Object();
        #endregion Private Members

        #region Constructor
        public MDMSyncDataService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        public List<int> GetReadyForUpdateDocumentUpdateTracker(int status)
        {
            List<int> documentUpdateTrackerList = new List<int>();
            documentUpdateTrackerList = (from s in this._unitOfWork.RepositoryAsync<DocumentUpdateTracker>()
                                                                       .Query()
                                                                       .Filter(c => c.Status == status)
                                                                       .Get().OrderByDescending(s => s.UpdatedDate).Take<DocumentUpdateTracker>(20)
                                         select s.ForminstanceID).ToList();
            return documentUpdateTrackerList;
        }

        public List<SchemaUpdateTracker> GetSchemaUpdateTracker()
        {
            List<SchemaUpdateTracker> schemaUpdateTrackerList = new List<SchemaUpdateTracker>();
            schemaUpdateTrackerList = this._unitOfWork.RepositoryAsync<SchemaUpdateTracker>()
                                                                        .Query()
                                                                        .Filter(c => c.Status == 1)
                                                                        .Get().ToList();
            return schemaUpdateTrackerList;
        }

        public void UpdateSchemaUpdateTracker(SchemaUpdateTracker schemaUpdateTracker)
        {
            if (schemaUpdateTracker != null)
            {
                this._unitOfWork.RepositoryAsync<SchemaUpdateTracker>().Update(schemaUpdateTracker);
                this._unitOfWork.Save();
            }
        }

        public void UpdateDocumentUpdateTrackerStatus(int formInstanceID, int status)
        {
            var documentUpdateTrackerList = this._unitOfWork.RepositoryAsync<DocumentUpdateTracker>()
                                                                        .Query()
                                                                        .Filter(c => c.Status == 1)
                                                                        .Get().FirstOrDefault();
            if (documentUpdateTrackerList != null)
            {
                documentUpdateTrackerList.Status = status;
                this._unitOfWork.RepositoryAsync<DocumentUpdateTracker>().Update(documentUpdateTrackerList);
                this._unitOfWork.Save();
            }
        }

        public void UpdateDocumentUpdateTracker(DocumentUpdateTracker documentUpdateTracker)
        {
            if (documentUpdateTracker != null)
            {
                this._unitOfWork.RepositoryAsync<DocumentUpdateTracker>().Update(documentUpdateTracker);
                this._unitOfWork.Save();
            }
        }

        public void UpdateDocumentUpdateTracker(int formInstanceID, int status)
        {
            lock (_updateMDMStatusLock)
            {
                var documentUpdateTracker = this._unitOfWork.RepositoryAsync<DocumentUpdateTracker>()
                                                                        .Query()
                                                                        .Filter(c => c.ForminstanceID == formInstanceID)
                                                                        .Get().ToList().FirstOrDefault();
                if (documentUpdateTracker != null)
                {
                    if (status == (int)MSMSyncStatus.Completed && documentUpdateTracker.Status == (int)MSMSyncStatus.InQueue)
                    {
                        documentUpdateTracker.Status = (int)MSMSyncStatus.ReadyForUpdate;
                        documentUpdateTracker.UpdatedDate = DateTime.Now;
                        this._unitOfWork.RepositoryAsync<DocumentUpdateTracker>().Update(documentUpdateTracker,true);
                        this._unitOfWork.Save();

                    }
                    else
                    {
                        documentUpdateTracker.Status = status;
                        documentUpdateTracker.UpdatedDate = DateTime.Now;
                        this._unitOfWork.RepositoryAsync<DocumentUpdateTracker>().Update(documentUpdateTracker, true);
                        this._unitOfWork.Save();
                    }
                }
            }
        }
        public void AddLogForMDMProcess(MDMLog mDMLog)
        {
            try
            {
                SqlParameter forminstanceID = new SqlParameter("@forminstanceID", mDMLog.ForminstanceID);
                SqlParameter formDesignID = new SqlParameter("@formDesignID", mDMLog.FormDesignID);
                SqlParameter formDesignVersionID = new SqlParameter("@formDesignVersionID", mDMLog.FormDesignVersionID);
                SqlParameter errorDescription = new SqlParameter("@errorDescription", mDMLog.ErrorDescription);
                SqlParameter error = new SqlParameter("@error", mDMLog.Error);
                SqlParameter addedDate = new SqlParameter("@addedDate", mDMLog.AddedDate);
                //SqlParameter updatedDate = new SqlParameter("@updatedDate", mDMLog.UpdatedDate);
                var log = this._unitOfWork.Repository<LogActivityDetail>().ExecuteSql("exec [dbo].[AddMDMLog] @forminstanceID, @formDesignID,@formDesignVersionID,@errorDescription,@error,@addedDate", forminstanceID, formDesignID, formDesignVersionID, errorDescription, error, addedDate).ToList();
            }
            catch (System.Exception)
            {
            }
        }

        public DocumentUpdateTracker GetDocumentUpdateTrackerStatusByFormInstanceId(int formInstanceID)
        {
            DocumentUpdateTracker documentUpdateTracker;
            documentUpdateTracker = this._unitOfWork.RepositoryAsync<DocumentUpdateTracker>()
                                                                        .Query()
                                                                        .Filter(c => c.ForminstanceID == formInstanceID)
                                                                        .Get().FirstOrDefault();
            return documentUpdateTracker;
        }

        public GridPagingResponse<ReportingCenterDataSyncViewModel> GetReportCenterDataSynclogList(GridPagingRequest gridPagingRequest)
        {
            List<ReportingCenterDataSyncViewModel> reportDataSynclist = new List<ReportingCenterDataSyncViewModel>();
            int count = 0;
            SearchCriteria criteria = new SearchCriteria();
            criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
            try
            {

                reportDataSynclist = (from dut in this._unitOfWork.RepositoryAsync<DocumentUpdateTracker>().Get()

                                      join fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get() on dut.ForminstanceID equals fi.FormInstanceID
                                      join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on fi.FormDesignID equals fd.FormID
                                      join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get() on fi.FolderVersionID equals fv.FolderVersionID
                                      join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get() on fv.FolderID equals fldr.FolderID
                                      select new ReportingCenterDataSyncViewModel
                                      {
                                          EffectiveDate = fv.EffectiveDate,
                                          FolderName = fldr.Name,
                                          FormInstanceId = dut.ForminstanceID,
                                          Name = fi.Name,
                                          Status = ((MSMSyncStatus)dut.Status).ToString(),
                                          UpdateDate = dut.UpdatedDate == null ? (DateTime)dut.AddedDate : (DateTime)dut.UpdatedDate,
                                          Version = fv.FolderVersionNumber,
                                          ProcessType = dut.ForminstanceID > 0 ? "Data" : "Schema",
                                          ViewType = fd.DisplayText
                                      }).ToList();

                List<ReportingCenterDataSyncViewModel> reportDesignSynclist = (from sut in this._unitOfWork.RepositoryAsync<SchemaUpdateTracker>().Get()

                                                                               join fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get() on sut.FormdesignVersionID equals fdv.FormDesignVersionID
                                                                               join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on fdv.FormDesignID equals fd.FormID

                                                                               select new ReportingCenterDataSyncViewModel
                                                                               {
                                                                                   EffectiveDate = (DateTime)fdv.EffectiveDate,
                                                                                   FolderName = fd.FormName,
                                                                                   FormDesignVersionId = sut.FormdesignVersionID,
                                                                                   Name = fd.FormName,
                                                                                   Status = ((MSMSyncStatus)sut.Status).ToString(),
                                                                                   UpdateDate = sut.UpdatedDate == null ? (DateTime)sut.AddedDate : (DateTime)sut.UpdatedDate,
                                                                                   Version = fdv.VersionNumber,
                                                                                   ProcessType = sut.FormdesignVersionID > 0 ? "Schema" : "Data"
                                                                               }).ToList();

                reportDataSynclist.AddRange(reportDesignSynclist.ToList());
                reportDataSynclist = reportDataSynclist.ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                        .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new GridPagingResponse<ReportingCenterDataSyncViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, reportDataSynclist);
        }

        public string GetMDMErrordata(int formInstanceId, int formDesignVersionId)
        {
            var errorData = String.Empty;
            try
            {
                if (formInstanceId > 0)
                {
                    var logRecord = this._unitOfWork.RepositoryAsync<MDMLog>()
                                                                                .Query()
                                                                                .Filter(c => c.ForminstanceID == formInstanceId)
                                                                                .Get().FirstOrDefault();
                    if (logRecord != null)
                    {
                        errorData = logRecord.Error + "|" + logRecord.ErrorDescription;
                    }
                }
                else
                {
                    var logRecord = this._unitOfWork.RepositoryAsync<MDMLog>()
                                                                                .Query()
                                                                                .Filter(c => c.FormDesignVersionID == formDesignVersionId)
                                                                                .Get().FirstOrDefault();
                    if (logRecord != null)
                    {
                        errorData = logRecord.Error + "|" + logRecord.ErrorDescription;
                    }
                }
            }
            catch (System.Exception)
            {
            }
            return errorData;
        }

    }


}
