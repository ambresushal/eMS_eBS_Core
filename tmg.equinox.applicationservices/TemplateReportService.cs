using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Diagnostics.Contracts;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.repository.interfaces;
using tmg.equinox.applicationservices.viewmodels.PrintTemplate;
using tmg.equinox.repository.extensions;
using tmg.equinox.applicationservices.viewmodels.UIElement;

namespace tmg.equinox.applicationservices
{
    public partial class TemplateReportService : ITemplateReportService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Memebers

        #region Constructor
        public TemplateReportService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        #region Public Methods
        public IEnumerable<TemplateViewModel> GetDocumentTemplateList(int tenantId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            IList<TemplateViewModel> DocumentTemplateList = null;
            try
            {
                //(from fld in this._unitOfWork.Repository<Folder>().Get()
                //                  join fI in this._unitOfWork.Repository<AccountFolderMap>().Get()
                //                            on fld.FolderID equals fI.FolderID
                DocumentTemplateList = (from t in this._unitOfWork.RepositoryAsync<Template>().Get()
                                        join fd in this._unitOfWork.Repository<FormDesign>().Get()
                                        on t.FormDesignID equals fd.FormID
                                        join fdv in this._unitOfWork.Repository<FormDesignVersion>().Get()
                                        on t.FormDesignVersionID equals fdv.FormDesignVersionID
                                        select new TemplateViewModel
                                  {
                                      FormDesignID = t.FormDesignID,
                                      FormDesignName = fd.FormName,
                                      FormDesignVersionID = fdv.FormDesignVersionID,
                                      VersionName = fdv.VersionNumber,
                                      TemplateID = t.TemplateID,
                                      TemplateName = t.TemplateName,
                                      isActive = t.IsActive,
                                      Description=t.Description
                                  }).ToList();

                if (DocumentTemplateList.Count() == 0)
                    DocumentTemplateList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return DocumentTemplateList;
        }


        public ServiceResult AddDocumentTemplate(int tenantId, int formDesignId, int formDesignVersionId, string templateName, string description, string userName)
        {
            Contract.Requires(!string.IsNullOrEmpty(templateName), "Template Name cannot be empty");
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            Contract.Requires(!string.IsNullOrEmpty(userName), "User Name cannot be empty");
            ServiceResult result = new ServiceResult();
            try
            {
                Template itemToAdd = new Template();
                itemToAdd.FormDesignID = formDesignId;
                itemToAdd.FormDesignVersionID = formDesignVersionId;
                itemToAdd.TenantID = tenantId;
                itemToAdd.TemplateName = templateName;
                itemToAdd.Description = description;
                itemToAdd.AddedBy = userName;
                itemToAdd.AddedDate = DateTime.Now;
                itemToAdd.IsActive = true;
                //Call to repository method to insert record.
                this._unitOfWork.RepositoryAsync<Template>().Insert(itemToAdd);
                this._unitOfWork.Save();
                //Return success result
                result.Result = ServiceResultStatus.Success;
                //Add add Template UIElement mapping if that is not found n database , if found update taht data source list
                result = AddTemplateUIElementMapping(itemToAdd.TemplateID, tenantId, formDesignVersionId, userName);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                //Get all the exception/inner exception messages and set the return code to failure
                result = ex.ExceptionMessages();
            }
            return result;
        }

        private ServiceResult AddTemplateUIElementMapping(int templateId, int tenantId, int formDesignVersionId, string userName)
        {
            var uiElementRowModelList = GetUIElementListForDocumentTemplate(tenantId, formDesignVersionId);
            ServiceResult result = new ServiceResult();
            foreach (var uiElementMappingModel in uiElementRowModelList)
            {
                TemplateUIMap ToAddTemplateUI = new TemplateUIMap();
                try
                {
                    SetTemplateUIMapping(templateId, uiElementMappingModel.UIElementID, true, tenantId, userName, ToAddTemplateUI, "Insert");
                    this._unitOfWork.RepositoryAsync<TemplateUIMap>().Insert(ToAddTemplateUI);
                    this._unitOfWork.Save();
                    //Return success result
                    result.Result = ServiceResultStatus.Success;
                }
                catch (Exception ex)
                {
                    bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                    if (reThrow) throw ex;
                    //Get all the exception/inner exception messages and set the return code to failure
                    result = ex.ExceptionMessages();
                }

            }
            return result;
        }

        private void SetTemplateUIMapping(int templateId, int? uIElementID, bool includePDF, int tenantId, string userName, TemplateUIMap ToAddTemplateUI, String mode)
        {
            ToAddTemplateUI.TemplateID = templateId;
            ToAddTemplateUI.UIElementID = uIElementID;
            ToAddTemplateUI.IncludeInPDF = includePDF;
            ToAddTemplateUI.TenantID = tenantId;
            if (mode == "Insert")
            {
                ToAddTemplateUI.AddedBy = userName;
                ToAddTemplateUI.AddedDate = DateTime.Now;
            }
            else
            {
                ToAddTemplateUI.UpdatedBy = userName;
                ToAddTemplateUI.UpdatedDate = DateTime.Now;
            }
        }


        public IEnumerable<UIElementRowModel> GetUIElementListForDocumentTemplate(int tenantId, int formDesignVersionId)
        {
            IList<UIElementRowModel> uiElementRowModelList = null;
            try
            {
                var elementList = this._unitOfWork.RepositoryAsync<UIElement>().GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId).ToList();

                uiElementRowModelList = (from i in elementList
                                         where (i is TabUIElement || i is SectionUIElement)
                                         //.Where(p => p.ApplicationDataType.ApplicationDataTypeName == "NA")
                                         select new UIElementRowModel
                                         {
                                             UIElementID = i.UIElementID,
                                         }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return uiElementRowModelList;
        }
        public IEnumerable<TemplateViewModel> LoadTemplateDesignUIElement(int tenantId, int formDesignVersionId, int templateId)
        {
            IList<TemplateViewModel> templateUIElementList = null;
            // IList<TemplateViewModel> uIElementList = null;
            try
            {
                //get all elements
                //IEnumerable<UIElementRowModel> elementList = GetUIElementListForDocumentTemplate(tenantId, formDesignVersionId).ToList();
                var elementList = this._unitOfWork.RepositoryAsync<UIElement>().GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId).ToList();

                templateUIElementList = (from e in elementList
                                         join t in this._unitOfWork.RepositoryAsync<TemplateUIMap>()
                                         .Query()
                                         .Filter(p=>p.TemplateID==templateId)
                                         .Get()
                                         on e.UIElementID equals t.UIElementID                                        
                                       
                                         select new TemplateViewModel
                                         {
                                             TenantID = t.TenantID,
                                             TemplateUIMapID = t.TemplateUIMapID,
                                             TemplateID = t.TemplateID,
                                             Label = e.Label == null ? "[Blank]" : e.Label,
                                             level = e.ParentUIElementID.HasValue ? GetRowLevel(e.ParentUIElementID, elementList) : 0,
                                             Sequence = e.Sequence,
                                             UIElementID = t.UIElementID,
                                             parent = e.ParentUIElementID.HasValue ? e.ParentUIElementID.Value.ToString() : "0",
                                             isActive = t.IncludeInPDF,
                                             isLeaf = e.ParentUIElementID.HasValue ? IsLeafRow(e.UIElementID, elementList) : false,
                                             isExt = true,
                                             loaded = true
                                         }).ToList();

                if (templateUIElementList.Count() == 0)
                    templateUIElementList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return templateUIElementList;
        }

        public ServiceResult UpdateTemplateDesignUIElement(int tenantId, List<TemplateViewModel> templateUIElementList, int templateID, string userName)
        {
            ServiceResult result = new ServiceResult();
            try
            {

                foreach (var uiElementMappingModel in templateUIElementList)
                {
                    var updateTemplateUIElement = this._unitOfWork.RepositoryAsync<TemplateUIMap>().Query()
                                                       .Filter(e => e.TemplateUIMapID == uiElementMappingModel.TemplateUIMapID)
                                                       .Get().FirstOrDefault();

                    updateTemplateUIElement.TemplateID = templateID;
                    updateTemplateUIElement.UIElementID = uiElementMappingModel.UIElementID;
                    updateTemplateUIElement.IncludeInPDF = uiElementMappingModel.IncludeInPDF;
                    updateTemplateUIElement.TenantID = tenantId;
                    updateTemplateUIElement.UpdatedBy = userName;
                    updateTemplateUIElement.UpdatedDate = DateTime.Now;

                    //SetTemplateUIMapping(templateID, uiElementMappingModel.UIElementID, uiElementMappingModel.IncludeInPDF, tenantId, userName, ToupdateTemplateUI,"Update");
                    this._unitOfWork.RepositoryAsync<TemplateUIMap>().Update(updateTemplateUIElement);
                    this._unitOfWork.Save();

                }
                //Return success result
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                //Get all the exception/inner exception messages and set the return code to failure
                result = ex.ExceptionMessages();
            }

            return result;
        }

        public ServiceResult DeleteDocumentTemplate(int tenantId, int templateId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                List<TemplateUIMap> toDeleteTemplateUIElemnetList = this._unitOfWork.RepositoryAsync<TemplateUIMap>().Query()
                                                       .Filter(e => e.TemplateID == templateId)
                                                       .Get().ToList();


                if (toDeleteTemplateUIElemnetList != null && toDeleteTemplateUIElemnetList.Count() > 0)
                {
                    foreach (var toDeleteUIElementMapping in toDeleteTemplateUIElemnetList)
                    {
                        this._unitOfWork.RepositoryAsync<TemplateUIMap>().Delete(toDeleteUIElementMapping);
                    }
                    this._unitOfWork.Save();
                }               

                List<Template> toDeleteDocumentTemplate = this._unitOfWork.RepositoryAsync<Template>().Query()
                                                      .Filter(e => e.TemplateID == templateId)
                                                      .Get().ToList();

                if (toDeleteDocumentTemplate != null && toDeleteDocumentTemplate.Count() > 0)
                {
                    foreach (var toDeleteTemplate in toDeleteDocumentTemplate)
                    {
                        this._unitOfWork.RepositoryAsync<Template>().Delete(toDeleteTemplate);
                    }
                    this._unitOfWork.Save();
                }
                
                //Return success result
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                //Get all the exception/inner exception messages and set the return code to failure
                result = ex.ExceptionMessages();
            }
            return result;
        }
        #endregion 
        #region Private Methods      
        private int GetRowLevel(int? parentID, List<UIElement> elementList)
        {
            int level = 0;
            try
            {
                while (parentID != null)
                {
                    level++;
                    var result = from element in elementList
                                 where element.UIElementID == parentID
                                 select element;

                    parentID = result.Single().ParentUIElementID;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return level;
        }
        private bool IsLeafRow(int? elementID, List<UIElement> models)
        {
            try
            {
                foreach (UIElement element in models)
                {
                    if (element.ParentUIElementID == elementID)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }
        #endregion
    }
}
