using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities;
using tmg.equinox.services.api.Framework;

namespace tmg.equinox.services.api.Services
{
    public class AddFolderVersionService
    {
        private IFolderVersionServices _folderVersionService;
        private IWorkFlowStateServices _workFlowStateService;
        private IConsumerAccountService _consumerAccountService;
        private BaseApiController _baseController;

        public AddFolderVersionService(IFolderVersionServices folderVersionService, IWorkFlowStateServices workFlowStateService, IConsumerAccountService consumerAccountService, BaseApiController baseController)
        {
            this._folderVersionService = folderVersionService;
            this._workFlowStateService = workFlowStateService;
            this._consumerAccountService = consumerAccountService;
            this._baseController = baseController;
        }

        public ServiceResult AddFolderVersion(int folderId, DateTime effectiveDate, string category, string categoryId, int tenantId, int currentUserId, string currentUserName)
        {
            ServiceResult result = new ServiceResult();
            int folderVersionStateId = 0;
            int catId = 0;
            FolderVersionViewModel folderVersiondetails = _folderVersionService.GetLatestFolderVersion(tenantId, folderId);
            if (folderVersiondetails == null)
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Please check the FolderId entered.There are no details associated to it. " } });
                return result;
            } 
            folderVersionStateId = _workFlowStateService.GetFolderVersionState(folderVersiondetails.FolderVersionId);
            if (folderVersionStateId != (int)tmg.equinox.domain.entities.Enums.FolderVersionState.RELEASED)
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "FolderVersion is not in Released State. " } });
                return result;
            } 
            if (folderVersiondetails.EffectiveDate > effectiveDate)
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Effective Date should be greater or equal to major version." } });
                return result;
            } 
            catId = _consumerAccountService.GetFolderCategoryId(category); 
            result = _folderVersionService.CreateNewMinorVersion(tenantId, folderId, folderVersiondetails.FolderVersionId,
                                            folderVersiondetails.FolderVersionNumber, "Create Minor Folder Version", effectiveDate, true, currentUserId, 0, catId, categoryId, currentUserName);
            if (result.Result == ServiceResultStatus.Success)
            {  
                var items = result.Items.ToList();
                List<FormInstanceViewModel> formList = _folderVersionService.GetFormInstanceDataOfFolderVersion(tenantId, Convert.ToInt32(items[0].Messages[0]));

                List<JToken> formInstanceJSONHashList = new List<JToken>();

                foreach (var forminstance in formList)
                {
                    if (forminstance.FormDesignID == GlobalVariables.PRODUCTFORMDESIGNID)
                    {
                        
                        JObject obj = JObject.Parse("{'FormInstanceId':'','ProductJSONHash':'','CurrentJSONHash':''}");
                        obj["FormInstanceId"] = Convert.ToInt32(forminstance.FormInstanceID);
                   
                        formInstanceJSONHashList.Add(obj);
                    }
                }

                if (formInstanceJSONHashList.Count != 0)
                {
                    _folderVersionService.SaveFormInstancesProductAndCurrentJSONHash(tenantId, JsonConvert.SerializeObject(formInstanceJSONHashList));
                }

                //Add applicable Team 
                List<int> applicableTeamsIDList = _folderVersionService.GetApplicableTeamID();
                _workFlowStateService.AddApplicableTeams(applicableTeamsIDList, folderId, folderVersiondetails.FolderVersionId, currentUserName);
            }
            return result;

        }
    }
}