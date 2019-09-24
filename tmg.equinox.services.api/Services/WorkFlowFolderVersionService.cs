using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.services.api.Framework;
using tmg.equinox.services.api.Models;

namespace tmg.equinox.services.api.Services
{
    public class WorkFlowFolderVersionStatusChanger
    {
        private IWorkFlowStateServices _workFlowStateService;
        private IFolderVersionServices _folderVersionServices;
        private IFormDesignService _formDesignServices;
        private IMasterListService _masterListServices;
        private BaseApiController _baseController;
        private IFacetTranslatorService _translstorService;

        public WorkFlowFolderVersionStatusChanger(IWorkFlowStateServices workFlowStateService, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices,
            IMasterListService masterListServices, BaseApiController baseController, IFacetTranslatorService translstorService)
        {
            this._workFlowStateService = workFlowStateService;
            this._formDesignServices = formDesignServices;
            this._folderVersionServices = folderVersionServices;
            this._masterListServices = masterListServices;
            this._baseController = baseController;
            this._translstorService = translstorService;

        }
        private ServiceResult UpdateStatus(FolderVersionViewModel folderversiondtls)
        {
            ServiceResult wFStateName = new ServiceResult(); 
            _workFlowStateService.UpdateWorkflowState(_baseController.TenantId, folderversiondtls.FolderVersionId, (int)folderversiondtls.WFStateID, Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.APPROVED),
                                     "Update WorkFlow through API", this._baseController.CurrentUserId, this._baseController.CurrentUserName, "", "", "");
            wFStateName.Result = ServiceResultStatus.Success;
            ((IList<ServiceResultItem>)(wFStateName.Items)).Add(new ServiceResultItem() { Messages = new string[] { "WorkFlow Updated. " } });

            return wFStateName;

        }

        public ServiceResult UpdateWorkFlowState(int folderVersionId)
        {
            ServiceResult wFStateName = new ServiceResult();
            string folderVersionWorkflowStateName = string.Empty;
            int folderVersionStateId = 0;
            string folderVersionStateName = string.Empty;

            FolderVersionViewModel folderversiondtls = _folderVersionServices.GetFolderVersionById(_baseController.TenantId, folderVersionId);
            ////check for translation
            string folderType = "";
            Dictionary<int, ProductState> productStateList = _translstorService.GetProductStateList(folderVersionId, "Account");
            bool isQueuedForTranslation = productStateList.Values.Any(e => e.IsProductInTranslation);
            bool isQueuedForTransmission = productStateList.Values.Any(e => e.IsProductInTransmission);

            if (isQueuedForTranslation)
            {
                wFStateName.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(wFStateName.Items)).Add(new ServiceResultItem() { Messages = new string[] { "WorkFlow cannot be updated FolderVersion is queued for Translation. " } });
                return wFStateName;
            }
            if (isQueuedForTransmission)
            {
                wFStateName.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(wFStateName.Items)).Add(new ServiceResultItem() { Messages = new string[] { "WorkFlow cannot be updated FolderVersion is queued for Transmission. " } });
                return wFStateName;
            }
            //

            if (folderversiondtls.IsPortfolio)
            {
                wFStateName = UpdateStatus(folderversiondtls);
                return wFStateName;
            }

            folderVersionStateId = _workFlowStateService.GetFolderVersionState(folderVersionId);
            if (folderVersionStateId == (int)tmg.equinox.domain.entities.Enums.FolderVersionState.RELEASED)
            {
                wFStateName.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(wFStateName.Items)).Add(new ServiceResultItem() { Messages = new string[] { "FolderVersion is in Released state hence can not be update workflow state." } });
                return wFStateName;
            }
            if (folderVersionStateId == (int)tmg.equinox.domain.entities.Enums.FolderVersionState.CANCELLED)
            {
                wFStateName.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(wFStateName.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Folder Version is in Cancelled state hence can not be update workflow state." } });
                return wFStateName;
            }
            if (folderVersionStateId != (int)tmg.equinox.domain.entities.Enums.FolderVersionState.INPROGRESS)
            {
                wFStateName.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(wFStateName.Items)).Add(new ServiceResultItem() { Messages = new string[] { "FolderVersion is not InProgress State. " } });
                return wFStateName;
            }

            folderVersionWorkflowStateName = _workFlowStateService.GetFolderVersionWorkFlowName(_baseController.TenantId, folderVersionId).Replace(" ", "");
            if (folderVersionWorkflowStateName == WorkFlowStateType.ReadytoAssign.ToString())
            {
                var formIntanceState = ValidateFormInstanceReadytoAssign(folderVersionId, folderversiondtls.FolderId);
                if (formIntanceState.Result == ServiceResultStatus.Failure)
                    return formIntanceState;
            }
            else
            {
                var formIntanceResult = ValidateFormInstance(folderVersionId);

                if (formIntanceResult.Result == ServiceResultStatus.Success)
                { 
                    if (folderVersionWorkflowStateName == WorkFlowStateType.ProductValidation.ToString())
                    { 
                        IEnumerable<KeyValue> actionResponsesForQueue = new List<KeyValue> { new KeyValue { Key = (int)ApprovalStatus.QueueForTranslator, Value = "Queue for Product Translator" } }; 
                        var productValidation = AddProducttoTranslateQueue(folderVersionId, actionResponsesForQueue);
                        if (productValidation.Result == ServiceResultStatus.Failure)
                        {
                            ((IList<ServiceResultItem>)(wFStateName.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Error in translation.please check. " } });
                            return productValidation;
                        }
                    }
                    if(folderVersionWorkflowStateName == WorkFlowStateType.ProductReview.ToString())
                    {
                        IEnumerable<KeyValue> actionResponsesForQueue = new List<KeyValue> { new KeyValue { Key = (int)ApprovalStatus.QueueForTransmissionToTest, Value = "Queue for Transmission To Test" } };
                        var productValidation = AddProducttoTranslateQueue(folderVersionId, actionResponsesForQueue);
                        if (productValidation.Result == ServiceResultStatus.Failure)
                        {
                            ((IList<ServiceResultItem>)(wFStateName.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Error in transmission to Test.please check. " } });
                            return productValidation;
                        }
                    }
                    if(folderVersionWorkflowStateName == WorkFlowStateType.VendorandQUALTesting.ToString())
                    {
                        IEnumerable<KeyValue> actionResponsesForQueue = new List<KeyValue> { new KeyValue { Key = (int)ApprovalStatus.QueueForTransmissionToProd, Value = "Queue for Transmission To Prod" } };
                        var productValidation = AddProducttoTranslateQueue(folderVersionId, actionResponsesForQueue);
                        if (productValidation.Result == ServiceResultStatus.Failure)
                        {
                            ((IList<ServiceResultItem>)(wFStateName.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Error in transmission to Prod.please check. " } });
                            return productValidation;
                        }
                    }
                }
                if (formIntanceResult.Result == ServiceResultStatus.Failure)
                    return formIntanceResult;
            }
            wFStateName = UpdateStatus(folderversiondtls);
            return wFStateName;
        }
        private ServiceResult AddProducttoTransmission()
        {
            ServiceResult wFStateName = new ServiceResult();
            wFStateName.Result = ServiceResultStatus.Success;
            return wFStateName;
        }

        private ServiceResult AddProducttoTranslateQueue(int folderVersionId, IEnumerable<KeyValue> actionResponsesForQueue)
        {
            ServiceResult wFStateName = new ServiceResult();
            wFStateName.Result = ServiceResultStatus.Success;
            throw new NotImplementedException();
            //FolderVersionViewModel folderversiondtls = _folderVersionServices.GetFolderVersionById(_baseController.TenantId, folderVersionId);
            //List<TranslatorModel> translatorModelLst = new List<TranslatorModel>();

            //WorkflowModel updateWorkflowData = new WorkflowModel();
            //updateWorkflowData.TenantID = _baseController.TenantId;
            //updateWorkflowData.UserID = _baseController.CurrentUserId;
            //updateWorkflowData.FolderVersionID = folderVersionId; 
            //updateWorkflowData.ApprovalStatusID = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.APPROVED);
            //updateWorkflowData.Commenttext = "Update for facet translator";
            //updateWorkflowData.WorkflowStateID = (int)folderversiondtls.WFStateID;

            //IList<TranslatorProductRowModel> productList = _translstorService.GetProductList(folderVersionId); 
            //var rowListlen = productList.Count;
            //foreach (var product in productList)
            //{
            //    TranslatorModel translatorModel = new TranslatorModel();
            //    translatorModel.Id = product.FormInstanceID;
            //    translatorModel.Plugin = "Facets";
            //    translatorModel.Version = "5.3";
            //    translatorModel.Product = product.Product;
            //    translatorModel.FolderVersionNumber = product.FolderVersionNumber;
            //    translatorModel.FormInstanceName = product.FormInstanceName;
            //    translatorModel.FolderName = product.FolderName;
            //    translatorModel.FormDesignID = (int)product.FormDesignID;
            //    translatorModel.FormDesignVersionID = (int)product.FormDesignVersionID;
            //    translatorModel.ConsortiumID = product.ConsortiumID == null ? 0 : (int)product.ConsortiumID;
            //    translatorModel.AccountID = (int)product.AccountID;
            //    translatorModel.FormInstanceID = (int)product.FormInstanceID;
            //    translatorModel.CurrentUser = _baseController.CurrentUserName;
            //    translatorModel.FolderID = folderversiondtls.FolderId;
            //    translatorModel.AccountName = product.AccountName;
            //    translatorModel.EffectiveDate = product.EffectiveDate;
            //    translatorModel.ConsortiumName = product.ConsortiumName;
            //    translatorModel.IsTranslated = false;
            //    translatorModel.FolderVersionID = folderVersionId;
            //    translatorModel.IsRetro = folderversiondtls.VersionTypeID == 2 ? true : false;
            //    translatorModel.IsShell = product.IsShell;
            //    translatorModel.ServiceRuleMove = 1;
            //    translatorModelLst.Add(translatorModel);
            //}
            //wFStateName = _translstorService.AddProducttoTranslate(translatorModelLst, false, _baseController.CurrentUserName, _baseController.TenantId, folderVersionId, updateWorkflowData, actionResponsesForQueue);

            return wFStateName; 
        }

        
        private ServiceResult ValidateFormInstance(int folderVersionId)
        {
            ServiceResult wFStateName = new ServiceResult();
            wFStateName.Result = ServiceResultStatus.Success;

            List<FormInstanceValidationErrorsViewModel> formIntancelst = null;
            var folderVersionValidator = new FolderVersionValidator(_folderVersionServices, _formDesignServices, _masterListServices);
            List<FormInstanceValidationErrorsViewModel> formInstanceList = folderVersionValidator.validate(_baseController.TenantId, folderVersionId,
                                                                                                    _baseController.CurrentUserId, _baseController.CurrentUserName);
            if (formInstanceList.Count() != 0)
            {
                formIntancelst = formInstanceList.Where(e => e.ErrorList != "[]" && e.ErrorList.Any()).ToList();
            }
            if (formIntancelst.Count() > 0)
            {
                wFStateName.Result = ServiceResultStatus.Failure;
                var Items = new List<ServiceResultItem>();
                var sections = new List<SectionResult>();
                foreach (var fromInstance in formIntancelst)
                {
                    var jarray = JArray.Parse(fromInstance.ErrorList);
                    foreach (var obj in jarray.Children())
                    {
                        var section = obj.SelectToken("SectionID");
                        var prop = section as JValue;
                        var sect = new SectionResult();
                        sect.SectionID = prop.Value.ToString();
                        section = obj.SelectToken("Section");
                        prop = section as JValue;
                        sect.SectionName = prop.Value.ToString();

                        foreach (var erro in obj.SelectToken("ErrorRows").Children())
                        {
                            var errorRow = new Models.ErrorRow();
                            var description = erro.SelectToken("Description");
                            var descriptionProp = description as JValue;
                            errorRow.Description = descriptionProp.Value.ToString();
                            sect.ErrorRows.Add(errorRow);
                        }
                        sections.Add(sect);
                    }
                }
                var serviceResultItem = new ServiceResultItem();
                serviceResultItem.Status = ServiceResultStatus.Failure;
                serviceResultItem.Messages = new string[1];
                serviceResultItem.Messages[0] = JsonConvert.SerializeObject(sections);
                wFStateName.Result = ServiceResultStatus.Failure;
                var items = new List<ServiceResultItem>();
                items.Add(serviceResultItem);
                wFStateName.Items = items;
            }
            return wFStateName;
        }
    }

}