using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.IO;
using tmg.equinox.applicationservices.FolderVersionDetail;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.Qhp;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.infrastructure.util;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using System.Transactions;
using tmg.equinox.applicationservices.viewmodels.Settings;
using Newtonsoft.Json;
using tmg.equinox.integration.qhplite.DocumentBuilder;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.integration.qhplite;
using tmg.equinox.integration.qhplite.DocumentExporter;
using tmg.equinox.integration.qhplite.DocumentMerger;
using tmg.equinox.notification;
using System.Data;
using System.Data.SqlClient;

namespace tmg.equinox.applicationservices
{

    public class QhpIntegrationService : IQhpIntegrationService
    {
        #region Private Memebers
        private IFormDesignService _formDesignService { get; set; }
        private IFolderVersionServices _folderVersionService { get; set; }
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        INotificationService _notificationService;
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public QhpIntegrationService(IUnitOfWorkAsync unitOfWork, IFormDesignService formDesignService, IFolderVersionServices folderVersionService, INotificationService notificationService)
        {
            this._unitOfWork = unitOfWork;
            this._formDesignService = formDesignService;
            this._folderVersionService = folderVersionService;
            _notificationService = notificationService;
        }
        #endregion Constructor

        #region Public Methods
        //public ServiceResult ImportQhpExcelTemplate(int tenantID, int formDesignVersionID, int folderVersionID, int folderID, string addedBy, string filePath)
        //{
        //    ServiceResult result = null;
        //    try
        //    {
        //        DateTime formDesignVersionEffectiveDate = this._formDesignService.GetFormDesignVersionById(formDesignVersionID).EffectiveDate.Value;
        //        string defaultJSON = this.GetDefaultJsonDataForFormDesign(tenantID, formDesignVersionID);
        //        DocumentBuilderFactory factory = new DocumentBuilderFactory();
        //        IList<QhpDocumentViewModel> documentList = factory.BuildDocumentsFromQHPFile(formDesignVersionID, formDesignVersionEffectiveDate, defaultJSON, filePath);
        //        using (var scope = new TransactionScope())
        //        {
        //            foreach (var document in documentList)
        //            {
        //                string formName = document.DocumentName;
        //                result = this.AddQhpFormInstance(tenantID, formDesignVersionID, folderVersionID, folderID, formName, addedBy, document.DocumentData);
        //                if (result.Result == ServiceResultStatus.Failure)
        //                {
        //                    throw new Exception("Error occurred while adding formInstance");
        //                }
        //            }
        //            scope.Complete();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = ex.ExceptionMessages();
        //        result.Result = ServiceResultStatus.Failure;
        //        bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
        //        if (reThrow)
        //            throw;
        //    }
        //    return result;
        //}

        public List<QhpPackageGroup> BuildQhpPackageGroups(List<int> documents)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FormInstanceId", typeof(int));
            DataRow row = null;
            foreach (int instance in documents)
            {
                row = dt.NewRow();
                row[0] = instance;
                dt.Rows.Add(row);
            }

            SqlParameter formInstances = new SqlParameter("@ForminstanceList", SqlDbType.Structured) { TypeName = "[Qhp].[FormInstanceList]", Value = dt };
            List<QhpPackageGroup> groupedInstances = this._unitOfWork.RepositoryAsync<QhpPackageGroup>().ExecuteSql("exec [Qhp].[GenerateGeneralInformationHash] @ForminstanceList", formInstances).ToList();

            return groupedInstances;
        }

        public ServiceResult ExportQhpExcelTemplate(int tenantID, int folderVersionID, List<int> formInstanceList, string folderPath, out string filePath, bool offExchangeOnly)
        {
            ServiceResult result = new ServiceResult();
            filePath = string.Empty;
            try
            {
                if (formInstanceList.Count() > 0)
                {
                    List<QhpDocumentViewModel> qhpDocumentList = new List<QhpDocumentViewModel>();

                    qhpDocumentList = (from c in this._unitOfWork.Repository<FormInstance>()
                                                                    .Query()
                                                                    .Filter(d => formInstanceList.Contains(d.FormInstanceID) && d.TenantID == tenantID)
                                                                    .Get()
                                       select new QhpDocumentViewModel
                                       {
                                           DocumentID = c.FormInstanceID,
                                           DocumentName = c.Name,
                                       }).ToList();

                    if (qhpDocumentList.Count() > 0)
                    {
                        //Get FormInstance data for selected documents
                        qhpDocumentList.ForEach(doc => { doc.DocumentData = GetSourceDocumentData(doc.DocumentID); });

                        //Merge and create a Benefit Package
                        List<int> documents = qhpDocumentList.Select(s => s.DocumentID).ToList();
                        var groupedInstances = BuildQhpPackageGroups(documents);
                        DocumentMerger merger = new DocumentMerger(qhpDocumentList);
                        qhpDocumentList = merger.GroupAndMerge(groupedInstances, offExchangeOnly);

                        //DateTime formDesignVersionEffectiveDate = this._formDesignService.GetFormDesignVersionById(formDesignVersionID).EffectiveDate.Value;
                        DateTime formDesignVersionEffectiveDate = _folderVersionService.GetFolderVersionEffectiveDate(1, folderVersionID);
                        DocumentExporterFactory factory = new DocumentExporterFactory();
                        filePath = factory.ExportQhpFileFromDocuments(folderVersionID, formDesignVersionEffectiveDate, folderPath, qhpDocumentList);
                        result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        result.Result = ServiceResultStatus.Failure;
                        ServiceResultItem serviceResultItem = new ServiceResultItem();
                        serviceResultItem.Messages = new string[1] { "No Documents were exported." };
                        IList<ServiceResultItem> serviceResultItemList = new List<ServiceResultItem>();
                        serviceResultItemList.Add(serviceResultItem);
                        result.Items = serviceResultItemList;
                    }
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    ServiceResultItem serviceResultItem = new ServiceResultItem();
                    serviceResultItem.Messages = new string[1] { "No Documents were selected." };
                    IList<ServiceResultItem> serviceResultItemList = new List<ServiceResultItem>();
                    serviceResultItemList.Add(serviceResultItem);
                    result.Items = serviceResultItemList;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        public List<QHPFormInstanceViewModel> GetFormInstances(List<int> formInstanceList)
        {
            List<QHPFormInstanceViewModel> qhpFormInstances = new List<QHPFormInstanceViewModel>();

            qhpFormInstances = (from inst in this._unitOfWork.Repository<FormInstance>().Get()
                                join i in formInstanceList on inst.AnchorDocumentID equals i
                                where inst.FormDesignID == 2424
                                select new QHPFormInstanceViewModel
                                {
                                    FormInstanceID = inst.FormInstanceID,
                                    Name = inst.Name,
                                    FolderVersionId = inst.FolderVersionID,
                                    FormDesignVersionID = inst.FormDesignVersionID
                                }).ToList();

            return qhpFormInstances;
        }

        public string GetSourceDocumentData(int formInstanceID)
        {
            string data = "";
            FormInstanceDataMap formInstance = null;
            try
            {
                formInstance = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().GetFormInstanceDataDecompressed(formInstanceID);

                if (formInstance != null)
                {
                    data = formInstance.FormData;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return data;
        }

        //public IList<tmg.equinox.applicationservices.viewmodels.Qhp.QHPValidationError> ValidateQhpDocument(int tenantID, int formDesignVersionID, int folderVersionID, int folderID, List<int> formInstanceList)
        //{
        //    IList<tmg.equinox.applicationservices.viewmodels.Qhp.QHPValidationError> validationErrorList = new List<tmg.equinox.applicationservices.viewmodels.Qhp.QHPValidationError>();
        //    try
        //    {
        //        if (formInstanceList.Count() > 0)
        //        {
        //            IList<tmg.equinox.integration.qhplite.QhpDocumentViewModel> qhpDocumentList = new List<QhpDocumentViewModel>();

        //            qhpDocumentList = (from c in this._unitOfWork.Repository<FormInstance>()
        //                                                            .Query()
        //                                                            .Filter(d => formInstanceList.Contains(d.FormInstanceID) && d.TenantID == tenantID && d.FolderVersionID == folderVersionID)
        //                                                            .Get()
        //                               select new QhpDocumentViewModel
        //                               {
        //                                   DocumentName = c.Name,
        //                                   DocumentData = c.FormInstanceDataMaps.Select(d => d.FormData).FirstOrDefault()
        //                               }).ToList();
        //            DateTime formDesignVersionEffectiveDate = this._formDesignService.GetFormDesignVersionById(formDesignVersionID).EffectiveDate.Value;
        //            DocumentValidatorFactory factory = new DocumentValidatorFactory();
        //            validationErrorList = (from q in factory.ValidateQhpDocument(formDesignVersionID, formDesignVersionEffectiveDate, qhpDocumentList)
        //                                   select new tmg.equinox.applicationservices.viewmodels.Qhp.QHPValidationError
        //                                   {
        //                                       ErrorCode = q.ErrorCode,
        //                                       ErrorMessage = q.ErrorMessage,
        //                                       ErrorType = q.ErrorType
        //                                   }).ToList();
        //        }
        //        else
        //        {
        //            throw new NullReferenceException("No Documents were selected");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
        //        if (reThrow)
        //            throw;
        //    }
        //    return validationErrorList;
        //}

        //  public string GetQHPDataForFolder(int tenantID, string folderName, string folderVersion, string accountName)
        //  {
        //      string xmlPackages = "";
        //      var fv = (from fldr in this._unitOfWork.Repository<Folder>()
        //                                          .Query()
        //                                          .Include(a => a.AccountFolderMaps)
        //                                          .Filter(r => (((r.Name == folderName) && (r.IsPortfolio == true && accountName == ""))
        //                                                  || ((r.IsPortfolio == false) && (r.AccountFolderMaps.Select(d => d.Account.AccountName == accountName).Count()) > 0))).Get()
        //                join fldrVr in this._unitOfWork.Repository<FolderVersion>()
        //                .Query()
        //                .Include(e => e.FormInstances)
        //                .Filter(s => s.FolderVersionNumber == folderVersion)
        //                .Get()
        //on fldr.FolderID equals fldrVr.FolderID
        //                select fldrVr);
        //      if (fv != null && fv.Count() > 0)
        //      {
        //          FolderVersion folder = fv.First();
        //          var formDocs = from doc in this._unitOfWork.Repository<FormInstance>()
        //                                          .Query()
        //                                          .Include(m => m.FormInstanceDataMaps)
        //                                          .Filter(g => g.Name.StartsWith("Benefits Package") && g.FolderVersionID == folder.FolderVersionID)
        //                                          .Get()
        //                                          .OrderBy(e => e.FormInstanceID)
        //                         select new
        //                         {
        //                             FormDesignVersionID = doc.FormDesignVersionID,
        //                             DocumentName = doc.Name,
        //                             DocumentData = doc.FormInstanceDataMaps.FirstOrDefault().FormData
        //                         };
        //          if (formDocs != null && formDocs.Count() > 0)
        //          {
        //              var formDoc = formDocs.First();
        //              List<QhpDocumentViewModel> forms = (from fr in formDocs
        //                                                  select new QhpDocumentViewModel
        //                                                  {
        //                                                      DocumentName = fr.DocumentName,
        //                                                      DocumentData = fr.DocumentData
        //                                                  }).ToList();

        //              var formDesign = from frm in this._unitOfWork.Repository<FormDesignVersion>()
        //                                          .Query()
        //                                          .Filter(d => d.FormDesignVersionID == formDoc.FormDesignVersionID)
        //                                          .Get()
        //                               select frm;
        //              if (formDesign != null && formDesign.Count() > 0)
        //              {
        //                  DocumentExporterFactory factory = new DocumentExporterFactory();
        //                  xmlPackages = factory.GetQHPXmlFromFolderVersion(forms, formDesign.First().EffectiveDate.Value);
        //              }
        //          }
        //      }
        //      return xmlPackages;
        //  }

        public QHPReportingQueue UpdateQHPReportQueue(List<DocumentViewModel> documents, string CurrentUserName, bool offExchangeOnly)
        {
            QHPReportingQueue queue = new QHPReportingQueue();
            try
            {
                if (documents.Count > 0)
                {
                    queue.QueuedDate = DateTime.Now;
                    queue.AddedBy = CurrentUserName;
                    queue.AddedDate = DateTime.Now;
                    queue.OffExchangeOnly = offExchangeOnly;
                    queue.Status = "New";
                    queue.QueueDetails = new List<QHPReportingQueueDetails>();
                    foreach (DocumentViewModel selectedDoc in documents)
                    {
                        QHPReportingQueueDetails details = new QHPReportingQueueDetails();
                        details.FolderID = selectedDoc.FolderID;
                        details.FolderVersionID = selectedDoc.FolderVersionID;
                        details.FormInstanceID = selectedDoc.FormInstanceID;
                        details.FormDesignID = selectedDoc.FormDesignID;
                        queue.QueueDetails.Add(details);
                    }
                    this._unitOfWork.RepositoryAsync<QHPReportingQueue>().Insert(queue);
                    this._unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                queue = null;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            if (queue != null)
            {
                List<Paramters> paramater = new List<Paramters>();
                _notificationService.SendNotification(
                                new NotificationInfo
                                {
                                    SentTo = CurrentUserName,
                                    MessageKey = MessageKey.TASK_QHP_UPDETED,
                                    ParamterValues = paramater,
                                    loggedInUserName = CurrentUserName,
                                });
            }
            return queue;
        }

        public void UpdateQHPReportQueueStatus(QHPReportingQueue queue, string QueueStatus, string FilePath, string MessageText)
        {
            if (queue != null)
            {
                queue.Status = QueueStatus;
                queue.DocumentLocation = FilePath;
                queue.Message = MessageText;

                this._unitOfWork.Repository<QHPReportingQueue>().Update(queue);
                this._unitOfWork.Save();
            }
        }
        public GridPagingResponse<QHPReportQueueViewModel> GetQHPReportQueueList(int tenantID, GridPagingRequest gridPagingRequest)
        {
            List<QHPReportQueueViewModel> documentList = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                documentList = (from fi in this._unitOfWork.RepositoryAsync<QHPReportingQueue>().Get()
                                select new QHPReportQueueViewModel
                                {
                                    QueueID = fi.QueueID,
                                    QueuedDate = fi.QueuedDate,
                                    AddedBy = fi.AddedBy,
                                    AddedDate = fi.AddedDate,
                                    DocumentLocation = fi.DocumentLocation,
                                    Status = fi.Status,
                                    Message = fi.Message
                                })
                                .ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count)
                                .ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<QHPReportQueueViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, documentList);
        }

        public string GetQHPReportPath(int QueueID)
        {
            return (from s in this._unitOfWork.RepositoryAsync<QHPReportingQueue>().Get()
                    where s.QueueID == QueueID
                    select s.DocumentLocation).FirstOrDefault().ToString();
        }
        public QHPReportQueueViewModel GetErrorDescription(int QueueID)
        {
            return (from fi in this._unitOfWork.RepositoryAsync<QHPReportingQueue>().Get().Where(x => x.QueueID == QueueID)
                    select new QHPReportQueueViewModel
                    {
                        QueueID = fi.QueueID,
                        QueuedDate = fi.QueuedDate,
                        AddedBy = fi.AddedBy,
                        AddedDate = fi.AddedDate,
                        DocumentLocation = fi.DocumentLocation,
                        Status = fi.Status,
                        Message = fi.Message
                    }).FirstOrDefault();
        }

        #endregion Public Methods

        #region Private Methods
        private string GetDefaultJsonDataForFormDesign(int tenantID, int formDesignVersionID)
        {
            string defaultJson = string.Empty;
            string formDesign = this._formDesignService.GetCompiledFormDesignVersion(tenantID, formDesignVersionID);
            FormDesignVersionDetail detail = JsonConvert.DeserializeObject<FormDesignVersionDetail>(formDesign);
            defaultJson = detail.GetDefaultJSONDataObject();
            return defaultJson;
        }
        private ServiceResult AddQhpFormInstance(int tenantID, int formDesignVersionID, int folderVersionID, int folderID, string formName, string addedBy, string formInstanceData)
        {
            ServiceResult result = null;

            if (this.IsFormInstanceExists(tenantID, formDesignVersionID, folderVersionID, folderID, formName))
            {
                int formInstanceID = this.GetFormInstanceIDByName(tenantID, formDesignVersionID, folderVersionID, folderID, formName);
                result = UpdateFormInstanceData(tenantID, formDesignVersionID, folderVersionID, folderID, formInstanceID, formName, addedBy, formInstanceData);
            }
            else
            {
                result = this._folderVersionService.CreateFormInstance(tenantID, folderVersionID, formDesignVersionID, 0, false, formName, addedBy);

                if (result.Result == ServiceResultStatus.Success)
                {
                    int formInstanceID = Convert.ToInt32(result.Items.Select(c => c.Messages.Select(d => d).FirstOrDefault()).FirstOrDefault());
                    result = UpdateFormInstanceData(tenantID, formDesignVersionID, folderVersionID, folderID, formInstanceID, formName, addedBy, formInstanceData);
                }
            }
            return result;
        }

        private ServiceResult UpdateFormInstanceData(int tenantID, int formDesignVersionID, int folderVersionID, int folderID, int formInstanceID, string formName, string addedBy, string formInstanceData)
        {
            ServiceResult result = null;
            result = this._folderVersionService.SaveFormInstanceData(tenantID, folderVersionID, formInstanceID, formInstanceData, addedBy);
            return result;
        }

        private bool IsFormInstanceExists(int tenantID, int formDesignVersionID, int folderVersionID, int folderID, string formName)
        {
            bool isExists = false;
            isExists = this._unitOfWork.Repository<FormInstance>()
                                            .Query()
                                            .Filter(c => c.FolderVersionID == folderVersionID && c.Name == formName && c.IsActive == true)
                                            .Get()
                                            .Any();
            return isExists;
        }

        private int GetFormInstanceIDByName(int tenantID, int formDesignVersionID, int folderVersionID, int folderID, string formName)
        {
            int formInstanceID = this._unitOfWork.Repository<FormInstance>()
                                                    .Query()
                                                    .Filter(c => c.FolderVersionID == folderVersionID && c.Name == formName)
                                                    .Get()
                                                    .Select(c => c.FormInstanceID)
                                                    .FirstOrDefault();
            return formInstanceID;
        }
        #endregion Private Methods

    }
}
