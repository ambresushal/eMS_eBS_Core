using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.Qhp;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.FormInstanceProcessor;

namespace tmg.equinox.web.extensions
{
    public class QHPExportPreProcessor
    {
        private int _currentUserId;
        private string _currentUserName;
        private IFolderVersionServices _folderVersionService;
        private IFormDesignService _formDesignService;
        private IFormInstanceDataServices _formInstanceDataService;
        private IReportingDataService _reportingDataService;
        private IMasterListService _masterListService;
        private IUIElementService _uiElementService;
        private IFormInstanceService _formInstanceService;
        private IQhpIntegrationService _qhpIntegrationService;
        public QHPExportPreProcessor(int? currentUserId, string currentUserName, IQhpIntegrationService qhpIntegrationService, IFolderVersionServices folderVersionService, IFormDesignService formDesignService, IFormInstanceDataServices formInstanceDataService, IReportingDataService reportingDataService, IMasterListService masterListService, IUIElementService uiElementService, IFormInstanceService formInstanceService)
        {
            _currentUserId = currentUserId ?? 0;
            _currentUserName = currentUserName;
            this._qhpIntegrationService = qhpIntegrationService;
            this._folderVersionService = folderVersionService;
            this._formDesignService = formDesignService;
            this._formInstanceDataService = formInstanceDataService;
            _reportingDataService = reportingDataService;
            _masterListService = masterListService;
            _uiElementService = uiElementService;
            _formInstanceService = formInstanceService;
        }

        
        public void ProcessRulesAndSaveSections(List<int> formInstanceList, QHPReportingQueue queue = null)
        {
            List<QHPFormInstanceViewModel> qhpFormInstances = _qhpIntegrationService.GetFormInstances(formInstanceList);
            
            if (qhpFormInstances.Count() > 0)
            {
                FormDesignVersionDetail detail = null;
                int tenantId = 1;
                foreach (var formInstance in qhpFormInstances)
                {
                    if (detail == null)
                    {
                        FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formInstance.FormDesignVersionID, _formDesignService);
                        detail = formDesignVersionMgr.GetFormDesignVersion(true);
                    }
                    bool isReleased = _folderVersionService.IsFolderVersionReleased(formInstance.FolderVersionId);
                    if (!isReleased)
                    {
                        FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(detail.TenantID, _currentUserId, _formInstanceDataService, _currentUserName, _folderVersionService, _reportingDataService, _masterListService);
                        IDataProcessor dataProcessor = new FormInstanceDataProcessor(_folderVersionService, formInstance.FormInstanceID, formInstance.FolderVersionId, formInstance.FormDesignVersionID, isReleased, _currentUserId, formDataInstanceManager, _formInstanceDataService, _uiElementService, detail, _currentUserName, _formDesignService, _masterListService, _formInstanceService);
                        string sectionName = "";
                        foreach (var sec in detail.Sections)
                        {
                            sectionName = sec.FullName;
                            formDataInstanceManager.RemoveSectionsData(formInstance.FormInstanceID, sectionName, _currentUserId);
                            dataProcessor.RunProcessorOnSectionLoad(sectionName, false);
                            string sectionData = detail.JSONData;
                            formDataInstanceManager.SetCacheData(formInstance.FormInstanceID, sectionName, sectionData);
                        }

                        formDataInstanceManager.SaveSectionsData(formInstance.FormInstanceID, false, _folderVersionService, _formDesignService, detail, sectionName);
                    }
                };
                if (queue != null)
                {
                    _qhpIntegrationService.UpdateQHPReportQueueStatus(queue, "InProgress", null, null);
                }
            }
        }
    }
}
