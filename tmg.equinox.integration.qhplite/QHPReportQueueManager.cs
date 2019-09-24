using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.notification;
using tmg.equinox.repository.interfaces;


namespace tmg.equinox.integration.qhplite
{
    public class QHPReportQueueManager : IQHPReportQueueManager
    {
        private int _commercialAnchorDesignID = 2405;
        private int _qhpFormDesignID = 2424;
        private int _qhpFormDesignVersionID = 2418;
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private IFormDesignService _formDesignService { get; set; }
        private IFormInstanceService _formInstanceService { get; set; }
        private IQhpIntegrationService _qhpIntegrationService { get; set; }
        private IFolderVersionServices _folderVersionService { get; set; }

        private static readonly Object _lockObject = new Object();

        private static readonly ILog _logger = LogProvider.For<QHPReportQueueManager>();

        INotificationService _notificationService;


        public QHPReportQueueManager(IUnitOfWorkAsync unitOfWork, IFormInstanceService formInstanceService, IQhpIntegrationService qhpIntegrationService, IFormDesignService formDesignService, INotificationService notificationService, IFolderVersionServices folderVersionService)
        {
            _unitOfWork = unitOfWork;
            _formInstanceService = formInstanceService;
            _qhpIntegrationService = qhpIntegrationService;
            _formDesignService = formDesignService;
            _folderVersionService = folderVersionService;
            this._notificationService = notificationService;
        }
        public void Execute()
        {
            try
            {
                lock (_lockObject)
                {

                    var queuedItems = GetQHPReportingQueueList().ToList();
                    //Parallel.ForEach(queuedItems, item =>
                    //{
                    //    ExecuteQueueAsync(item);
                    //});

                    foreach (var item in queuedItems)
                        Task.Run(() => ExecuteQueueAsync(item));
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException(ex.Message, ex);
            }
        }
        private void ExecuteQueueAsync(QHPReportingQueue queue)
        {
            try
            {
                var formInstances = this._formInstanceService.GetQHPViewByAnchor(GetFormInstances(queue.QueueID), this._qhpFormDesignID, queue.OffExchangeOnly);
                var formInstanceList = formInstances.Select(s => s.FormInstanceId).ToList();
                string folderPath = ConfigurationManager.AppSettings["QhpFilePath"].ToString();
                string filePath = string.Empty;
                int folderVersionID = 0;
                try
                {
                    if (formInstances != null)
                        folderVersionID = formInstances.FirstOrDefault() == null ? 0 : formInstances.FirstOrDefault().FolderVersionId;
                }
                catch (Exception)
                {
                }
                ServiceResult result = ExportQhpExcelTemplate(folderVersionID, formInstanceList, folderPath, out filePath, queue.OffExchangeOnly);
                if (result.Result == ServiceResultStatus.Success)
                {
                    _qhpIntegrationService.UpdateQHPReportQueueStatus(queue, "Completed", filePath, null);
                    List<Paramters> paramater = new List<Paramters>();
                    paramater.Add(new Paramters { key = "user", Value = queue.AddedBy });
                    paramater.Add(new Paramters { key = "qid", Value = Convert.ToString(queue.QueueID) });
                    _notificationService.SendNotification(
                                new NotificationInfo
                                {
                                    SentTo = queue.AddedBy,
                                    MessageKey = MessageKey.TASK_QHP_COMPLETED,
                                    ParamterValues = paramater,
                                    loggedInUserName = queue.AddedBy,
                                });
                }
                else
                {
                    if (result.Items.Count() > 0)
                    {
                        ServiceResultItem resultItme = result.Items.FirstOrDefault();
                        _qhpIntegrationService.UpdateQHPReportQueueStatus(queue, "Fail", null, resultItme.Messages[0].ToString());
                    }
                    else
                        _qhpIntegrationService.UpdateQHPReportQueueStatus(queue, "Fail", null, null);
                }
            }
            catch (Exception ex)
            {
                _qhpIntegrationService.UpdateQHPReportQueueStatus(queue, "Fail", null, ex.Message);
                _logger.ErrorException(ex.Message, ex);
            }
        }
        public ServiceResult ExportQhpExcelTemplate(int folderVersionID, List<int> formInstanceList, string folderPath, out string filePath, bool offExchangeOnly)
        {
            ServiceResult result = new ServiceResult();
            filePath = string.Empty;
            if (formInstanceList.Count() > 0)
            {
                List<QhpDocumentViewModel> qhpDocumentList = new List<QhpDocumentViewModel>();

                qhpDocumentList = (from c in this._unitOfWork.Repository<FormInstance>()
                                                                .Query()
                                                                .Filter(d => formInstanceList.Contains(d.FormInstanceID))
                                                                .Get()
                                   select new QhpDocumentViewModel
                                   {
                                       DocumentID = c.FormInstanceID,
                                       DocumentName = c.Name,
                                   }).ToList();

                if (qhpDocumentList.Count() > 0)
                {
                    qhpDocumentList.ForEach(doc => { doc.DocumentData = _qhpIntegrationService.GetSourceDocumentData(doc.DocumentID); });

                    //DateTime formDesignVersionEffectiveDate = _formDesignService.GetFormDesignVersionById(formDesignVersionID).EffectiveDate.Value;
                    DateTime folderVersionEffectiveDate = _folderVersionService.GetFolderVersionEffectiveDate(1, folderVersionID);
                    filePath = CreateBenifitPackage(qhpDocumentList, folderVersionID, folderVersionEffectiveDate, folderPath, offExchangeOnly);
                    if (filePath != "")
                    {
                        result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        result.Result = ServiceResultStatus.Failure;
                        ServiceResultItem serviceResultItem = new ServiceResultItem();
                        serviceResultItem.Messages = new string[1] { "No documents were exported." };
                        IList<ServiceResultItem> serviceResultItemList = new List<ServiceResultItem>();
                        serviceResultItemList.Add(serviceResultItem);
                        result.Items = serviceResultItemList;
                    }
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    ServiceResultItem serviceResultItem = new ServiceResultItem();
                    serviceResultItem.Messages = new string[1] { "There are no documents found to export report." };
                    IList<ServiceResultItem> serviceResultItemList = new List<ServiceResultItem>();
                    serviceResultItemList.Add(serviceResultItem);
                    result.Items = serviceResultItemList;
                }
            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
                ServiceResultItem serviceResultItem = new ServiceResultItem();
                serviceResultItem.Messages = new string[1] { "There are no documents selected to export report." };
                IList<ServiceResultItem> serviceResultItemList = new List<ServiceResultItem>();
                serviceResultItemList.Add(serviceResultItem);
                result.Items = serviceResultItemList;
            }
            return result;
        }

        private async Task<Tuple<List<QhpDocumentViewModel>>> GetDocumentData(List<QhpDocumentViewModel> qhpDocumentList)
        {
            List<Task> task = new List<Task>();

            await Task.Run(() => Parallel.ForEach(qhpDocumentList, doc =>
            {
                doc.DocumentData = "";// _qhpIntegrationService.GetSourceDocumentData(doc.DocumentID);
            }));
            return new Tuple<List<QhpDocumentViewModel>>(qhpDocumentList);
        }
        
        private IQueryable<QHPReportingQueue> GetQHPReportingQueueList()
        {
            return this._unitOfWork.RepositoryAsync<QHPReportingQueue>().Get().Where(x => x.Status == "InProgress").Take(1);
        }
        private List<int> GetFormInstances(int QueueID)
        {
            return _unitOfWork.RepositoryAsync<QHPReportingQueueDetails>().Get().Where(x => x.QueueID == QueueID).Select(s => s.FormInstanceID).ToList();
        }
        private string CreateBenifitPackage(List<QhpDocumentViewModel> qhpDocumentList, int folderVersionID, DateTime folderVersionEffectiveDate, string folderPath, bool offExchangeOnly)
        {
            string filePath = "";

            qhpDocumentList = this.GetGroups(qhpDocumentList, offExchangeOnly);
            if (qhpDocumentList.Count > 0)
            {
                DocumentExporter.DocumentExporterFactory factory = new DocumentExporter.DocumentExporterFactory();
                filePath = factory.ExportQhpFileFromDocuments(folderVersionID, folderVersionEffectiveDate, folderPath, qhpDocumentList);
            }
            return filePath;
        }

        private List<QhpDocumentViewModel> GetGroups(List<QhpDocumentViewModel> qhpDocumentList, bool offExchangeOnly)
        {
            List<int> documents = qhpDocumentList.Select(s => s.DocumentID).ToList();
            var groupedInstances = _qhpIntegrationService.BuildQhpPackageGroups(documents);
            if (groupedInstances.Count > 0 && groupedInstances[0].ForminstanceID == 0)
            {
                throw new Exception("An error occurred while creating Benefit Package groups due invalid characters in the data.");
            }
            DocumentMerger.DocumentMerger merger = new DocumentMerger.DocumentMerger(qhpDocumentList);
            qhpDocumentList = merger.GroupAndMerge(groupedInstances, offExchangeOnly);

            return qhpDocumentList;
        }
    }
}
