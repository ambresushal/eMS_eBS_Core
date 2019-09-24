using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.ruleinterpreter.model;
using Newtonsoft.Json;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.applicationservices.viewmodels.DocumentRule;
using Newtonsoft.Json.Linq;
using tmg.equinox.infrastructure.util;
using System.Data.SqlClient;
using tmg.equinox.applicationservices.FormDesignDetail;
using tmg.equinox.ruleinterpreter.rulecompiler;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.ruleinterpreter.RuleCompiler;

namespace tmg.equinox.applicationservices
{
    public partial class UIElementService : IUIElementService
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Private Methods

        private int ChangeDocumentRules(string userName, int uiElementId, string uiElementType, int formDesignId, int formDesignVersionId, IEnumerable<DocumentRuleModel> nRules)
        {
            int docId = 0;
            try
            {
                //get the current rules
                IEnumerable<DocumentRuleModel> cRules = GetDocumentRule(uiElementId, formDesignId, formDesignVersionId);
                if ((nRules == null || nRules.Count() == 0) && (cRules == null || cRules.Count() == 0))
                {
                    return docId;
                }

                //for each rule, find matching rule - update if not new/ else create new

                if (cRules == null || cRules.Count() == 0)
                {
                    cRules = new List<DocumentRuleModel>();
                    foreach (DocumentRuleModel nwRule in nRules)
                    {
                        docId = AddDocumentRule(userName, uiElementId, uiElementType, nwRule);
                    }
                }
                else
                {
                    foreach (DocumentRuleModel nwRule in nRules)
                    {
                        var currentRule = (from c in cRules where c.DocumentRuleID == nwRule.DocumentRuleID select c).FirstOrDefault();
                        if (currentRule != null)
                        {
                            docId = UpdateDocumentRule(userName, nwRule);
                        }
                        else
                        {
                            docId = AddDocumentRule(userName, uiElementId, uiElementType, nwRule);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return docId;
        }

        private int AddDocumentRule(string userName, int uiElementID, string uiElementType, DocumentRuleModel model)
        {
            int docId = new int();
            try
            {
                DocumentRuleTargetType drTargetType = this._unitOfWork.RepositoryAsync<DocumentRuleTargetType>()
                                                          .Query()
                                                          .Filter(f => f.DisplayText == uiElementType)
                                                          .Get().FirstOrDefault();
                string path = GetUIElementFullPath(uiElementID, model.FormDesignVersionID);

                DocumentRule arule = new DocumentRule();
                arule.DisplayText = model.DisplayText;
                arule.Description = model.Description;
                arule.AddedBy = userName;
                arule.AddedDate = DateTime.Now;
                arule.DocumentRuleTypeID = model.DocumentRuleTypeID;
                arule.RuleJSON = model.RuleJSON;
                arule.FormDesignID = model.FormDesignID;
                arule.FormDesignVersionID = model.FormDesignVersionID;
                arule.TargetUIElementID = uiElementID;
                arule.TargetElementPath = path;
                arule.DocumentRuleTargetTypeID = drTargetType.DocumentRuleTargetTypeID;
                arule.IsActive = true;

                Documentrule documentRule = DocumentRuleSerializer.Deserialize(arule.RuleJSON);
                DocumentRuleCompiler ruleCompiler = new DocumentRuleCompiler(arule.DocumentRuleTypeID, documentRule);
                CompiledDocumentRule compiledRule = ruleCompiler.CompileRule();
                var compiledJson = DocumentRuleSerializer.Serialize(compiledRule);
                arule.CompiledRuleJSON = compiledJson;

                this._unitOfWork.RepositoryAsync<DocumentRule>().Insert(arule);
                this._unitOfWork.Save();
                docId = arule.DocumentRuleID;
                AddDocumentRuleEvents(docId);
            }
            catch (Exception ex)
            {
            }
            return docId;
        }

        private void DeleteDRule(int DocumentRuleID)
        {

            if (DocumentRuleID > 0)
            {
                DeleteDocumentRuleEvents(DocumentRuleID);
                this._unitOfWork.RepositoryAsync<DocumentRule>().Delete(DocumentRuleID);
                this._unitOfWork.Save();
            }

        }

        private int UpdateDocumentRule(string userName, DocumentRuleModel urule)
        {
            int docId = new int();
            try
            {
                DocumentRule ruleToUpdate = this._unitOfWork.RepositoryAsync<DocumentRule>().FindById(urule.DocumentRuleID);
                ruleToUpdate.UpdatedBy = userName;
                ruleToUpdate.UpdatedDate = DateTime.Now;
                ruleToUpdate.DocumentRuleTypeID = urule.DocumentRuleTypeID;
                ruleToUpdate.Description = urule.Description;
                ruleToUpdate.DisplayText = urule.Description;
                ruleToUpdate.RuleJSON = urule.RuleJSON;
                ruleToUpdate.CompiledRuleJSON = urule.CompiledRuleJSON;

                this._unitOfWork.RepositoryAsync<DocumentRule>().Update(ruleToUpdate);
                this._unitOfWork.Save();
                docId = ruleToUpdate.DocumentRuleID;
            }
            catch (Exception ex)
            {
            }
            return docId;
        }

        private string GetEventType(int DocumentRuleID)
        {
            string eventType = "";
            try
            {
                var eventTypeList = (from em in this._unitOfWork.RepositoryAsync<DocumentRuleEventMap>().Query().Filter(c => c.DocumentRuleID == DocumentRuleID).Get()
                                     join et in this._unitOfWork.RepositoryAsync<DocumentRuleEventType>().Query().Get() on em.DocumentRuleEventTypeID equals et.DocumentRuleEventTypeID
                                     select et.DisplayText).ToList();
                if (eventTypeList.Count == 0)
                {
                    return eventType;
                }
                else if (eventTypeList.Count == 1 && eventTypeList[0] == "selectdialog")
                {
                    eventType = "Exclude";
                }
                else
                {
                    foreach (var e in eventTypeList)
                    {
                        if (e != "selectdialog")
                        {
                            eventType += e + ",";
                        }
                    }
                    eventType = eventType.Remove(eventType.LastIndexOf(","));
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return eventType;
        }

        public string GetRuleType(int DocumentRuleTypeID)
        {
            string ruleType = "";
            try
            {
                ruleType = (from c in this._unitOfWork.RepositoryAsync<DocumentRuleType>().Get()
                            where c.DocumentRuleTypeID == DocumentRuleTypeID
                            select c.DisplayText).ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return ruleType;
        }

        private void AddDocumentRuleEvents(int documentId)
        {
            try
            {
                DocumentRuleEventMap arule = new DocumentRuleEventMap();
                arule.DocumentRuleID = documentId;
                for (int i = 1; i < 6; i++)
                {
                    arule.DocumentRuleEventTypeID = i;
                    this._unitOfWork.RepositoryAsync<DocumentRuleEventMap>().Insert(arule);
                    this._unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void DeleteDocumentRuleEvents(int DocumentRuleID)
        {
            var getEvents = this._unitOfWork.RepositoryAsync<DocumentRuleEventMap>()
                                                          .Query()
                                                          .Get()
                                                          .Where(f => f.DocumentRuleID == DocumentRuleID)
                                                          .ToList();
            if (getEvents.Count > 0)
            {
                foreach (var i in getEvents)
                {
                    this._unitOfWork.RepositoryAsync<DocumentRuleEventMap>().Delete(i.DocumentRuleEventMapID);
                    this._unitOfWork.Save();
                }

            }

        }
        #endregion Private Methods

        #region Public Methods

        public IEnumerable<DocumentRuleModel> GetDocumentRule(int uiElementId, int formDesignId, int formDesignVersionId)
        {
            Contract.Requires(uiElementId > 0, "Invalid uiElementId");
            ServiceResult result = new ServiceResult();
            IList<DocumentRuleModel> rowModelList = null;
            try
            {
                List<DocumentRule> DocRule = this._unitOfWork.RepositoryAsync<DocumentRule>()
                                                                .Query()
                                                                .Filter(f => f.FormDesignVersionID == formDesignVersionId
                                                                && f.TargetUIElementID == uiElementId
                                                                && f.FormDesignID == formDesignId)
                                                                .Get().ToList();

                var elementModels = from dr in DocRule
                                    select new DocumentRuleModel
                                    {
                                        DocumentRuleID = dr.DocumentRuleID,
                                        DocumentRuleTypeID = dr.DocumentRuleTypeID,
                                        Description = dr.Description,
                                        RuleJSON = dr.RuleJSON
                                    };
                rowModelList = elementModels.ToList();
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return rowModelList;
        }

        public List<ExpressionRuleGeneratorModel.Sources> UpdateFormDesignIdInSourceData(List<ExpressionRuleGeneratorModel.Sources> sourceData, DateTime effecttiveDate)
        {
            List<ExpressionRuleGeneratorModel.Sources> updatedSourceData = new List<ExpressionRuleGeneratorModel.Sources>();
            try
            {
                updatedSourceData = (from sdata in sourceData
                                     join formDesign in _unitOfWork.RepositoryAsync<FormDesign>().Get()
                                     on sdata.sourceelement.Substring(0, sdata.sourceelement.IndexOf('[')) equals formDesign.FormName
                                     select new ExpressionRuleGeneratorModel.Sources
                                     {
                                         sourcename = sdata.sourcename,
                                         sourceelement = sdata.sourceelement,
                                         sourceelementtype = sdata.sourceelementtype,
                                         sourcedocumentfilter = sdata.sourcedocumentfilter,
                                         sourceformdesignid = formDesign.FormID,
                                         sourceelementlabel = sdata.sourceelementlabel
                                     }).ToList();

                if (updatedSourceData != null && updatedSourceData.Count > 0)
                {
                    foreach (ExpressionRuleGeneratorModel.Sources source in updatedSourceData)
                    {
                        if (source.sourceelementlabel == "")
                        {
                            int formDesignVersionId = GetFormDesignVersionID(source.sourceformdesignid, effecttiveDate);
                            IEnumerable<ElementListViewModel> uiElementList = GetUIElementListForExpressionRuleBuilder(1, formDesignVersionId);
                            string formDesignName = source.sourceelement.Substring(0, source.sourceelement.IndexOf('['));
                            string JSONPath = source.sourceelement.Replace(formDesignName + "[", "").Replace("]", "");
                            ElementListViewModel element = uiElementList.Where(x => x.ElementJSONPath == JSONPath).FirstOrDefault();
                            if (element != null)
                            {
                                source.sourceelementlabel = element.ElementFullPath;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return updatedSourceData;
        }
        public CompiledDocumentRule GetCompiledRuleJSON(int targetElementID, string targetElementPath)
        {
            CompiledDocumentRule compiledDocumentRule = null;
            DocumentRuleModel documentRuleModel = null;
            try
            {
                if (!String.IsNullOrEmpty(targetElementPath))
                {
                    documentRuleModel = (from rule in this._unitOfWork.RepositoryAsync<DocumentRule>()
                                                                  .Query()
                                                                  .Get()
                                         join ruleEventMap in this._unitOfWork.RepositoryAsync<DocumentRuleEventMap>()
                                                                 .Query()
                                                                 .Get()
                                         on rule.DocumentRuleID equals ruleEventMap.DocumentRuleID
                                         where rule.TargetUIElementID == targetElementID && rule.TargetElementPath == targetElementPath && ruleEventMap.DocumentRuleEventTypeID == (int)(RuleEventType.SELECTDIALOG)
                                         select new DocumentRuleModel
                                         {
                                             CompiledRuleJSON = rule.CompiledRuleJSON
                                         }).FirstOrDefault();
                    if (null != documentRuleModel && !String.IsNullOrEmpty(documentRuleModel.CompiledRuleJSON))
                    {
                        compiledDocumentRule = JsonConvert.DeserializeObject<CompiledDocumentRule>(documentRuleModel.CompiledRuleJSON);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return compiledDocumentRule;
        }
        public CompiledDocumentRule GetCompiledRuleJSON(int targetElementID, int formDesignVersionId, bool uIElementNameNeeded)
        {
            CompiledDocumentRule compiledDocumentRule = null;
            DocumentRuleModel documentRuleModel = null;
            try
            {
                if (targetElementID > 0)
                {
                    if (uIElementNameNeeded)
                    {
                        string uiElementName = this._unitOfWork.RepositoryAsync<UIElement>().Get().Where(s => s.UIElementID == targetElementID).Select(s => s.UIElementName).FirstOrDefault();

                        if (!string.IsNullOrEmpty(uiElementName))
                        {
                            int uielementid = (from element in this._unitOfWork.RepositoryAsync<UIElement>()
                                               .Get()
                                               join map in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                               .Get()
                                               on element.UIElementID equals map.UIElementID
                                               where element.UIElementName == uiElementName && map.FormDesignVersionID == formDesignVersionId
                                               select element.UIElementID).FirstOrDefault();

                            if (uielementid > 0)
                                targetElementID = uielementid;
                        }
                    }

                    documentRuleModel = (from rule in this._unitOfWork.RepositoryAsync<DocumentRule>()
                                                                  .Query()
                                                                  .Get()
                                         join ruleEventMap in this._unitOfWork.RepositoryAsync<DocumentRuleEventMap>()
                                                                 .Query()
                                                                 .Get()
                                         on rule.DocumentRuleID equals ruleEventMap.DocumentRuleID
                                         where rule.TargetUIElementID == targetElementID && ruleEventMap.DocumentRuleEventTypeID == (int)(RuleEventType.SELECTDIALOG)
                                         && rule.FormDesignVersionID == formDesignVersionId
                                         select new DocumentRuleModel
                                         {
                                             CompiledRuleJSON = rule.CompiledRuleJSON
                                         }).FirstOrDefault();
                    if (null != documentRuleModel && !String.IsNullOrEmpty(documentRuleModel.CompiledRuleJSON))
                    {
                        compiledDocumentRule = JsonConvert.DeserializeObject<CompiledDocumentRule>(documentRuleModel.CompiledRuleJSON);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return compiledDocumentRule;
        }


        public CompiledDocumentRule GetCompiledRuleJSONForDSExpRule(int targetElementID, int formDesignVersionId)
        {
            CompiledDocumentRule compiledDocumentRule = null;

            List<DocumentRule> targetElementRules = (this._unitOfWork.RepositoryAsync<DocumentRule>()
                                                          .Query()
                                                          .Filter(x => x.TargetUIElementID == targetElementID
                                                                 && x.FormDesignVersionID == formDesignVersionId)
                                                          .Get()
                                                          .Select(sel => sel)
                                                          ).ToList();

            if (targetElementRules != null && targetElementRules.Count > 1)
            {

                foreach (DocumentRule targetDocumentRule in targetElementRules)
                {
                    List<DocumentRuleEventMap> eventMap = (this._unitOfWork.RepositoryAsync<DocumentRuleEventMap>()
                                                              .Query()
                                                              .Filter(x => x.DocumentRuleID == targetDocumentRule.DocumentRuleID)
                                                              .Get()
                                                              .Select(sel => sel)
                                                              ).ToList();

                    if (eventMap != null && eventMap.Count == 1 & eventMap.First().DocumentRuleEventTypeID == (int)(RuleEventType.SELECTDIALOG) && targetDocumentRule.CompiledRuleJSON != null)
                    {
                        compiledDocumentRule = JsonConvert.DeserializeObject<CompiledDocumentRule>(targetDocumentRule.CompiledRuleJSON);
                        break;
                    }
                }
            }
            else if (targetElementRules != null && targetElementRules.Count == 1)
            {
                compiledDocumentRule = JsonConvert.DeserializeObject<CompiledDocumentRule>(targetElementRules.FirstOrDefault().CompiledRuleJSON);
            }
            return compiledDocumentRule;
        }


        public IEnumerable<DocumentRuleTypeModel> GetDocumentRuleType()
        {
            ServiceResult result = new ServiceResult();
            IEnumerable<DocumentRuleTypeModel> ruleTypeList = null;
            try
            {
                ruleTypeList = (from c in this._unitOfWork.RepositoryAsync<DocumentRuleType>()
                                                                               .Query()
                                                                               .Get()
                                select new DocumentRuleTypeModel
                                {
                                    DocumentRuleTypeID = c.DocumentRuleTypeID,
                                    DisplayText = c.DisplayText
                                }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return ruleTypeList;
        }

        public ServiceResult SaveDocumentRule(string userName, int uiElementId, string uiElementType, int formDesignId, int formDesignVersionId, IEnumerable<DocumentRuleModel> nRules)
        {
            ServiceResult result = new ServiceResult();
            int docId = new int();
            try
            {
                docId = ChangeDocumentRules(userName, uiElementId, uiElementType, formDesignId, formDesignVersionId, nRules);
                result.Result = ServiceResultStatus.Success;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { docId.ToString() } });
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult DeleteDocumentRule(int dRuleId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                DeleteDRule(dRuleId);
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public string GetUIElementFullPath(int uielementID, int formDesignVersionID)
        {
            string fullName = "";
            try
            {
                IList<UIElement> formElementList = (from u in this._unitOfWork.RepositoryAsync<UIElement>()
                                                                .Query()
                                                                .Get()
                                                    join fd in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                            .Query()
                                                                            .Get()
                                                    on u.UIElementID equals fd.UIElementID
                                                    where fd.FormDesignVersionID == formDesignVersionID
                                                    select u).ToList();

                UIElement element = (from elem in formElementList
                                     where elem.UIElementID == uielementID
                                     select elem).FirstOrDefault();
                if (element != null)
                {
                    fullName = GetFullPath(element, formElementList);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return fullName;
        }

        public string GetFullPath(UIElement element, IEnumerable<UIElement> formElementList)
        {
            string fullName = "";
            try
            {
                int currentElementID = element.UIElementID;
                int parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
                fullName = element.GeneratedName;
                while (parentUIElementID > 0)
                {
                    element = (from elem in formElementList
                               where elem.UIElementID == parentUIElementID
                               select elem).FirstOrDefault();
                    parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
                    if (parentUIElementID > 0)
                    {
                        fullName = element.GeneratedName + "." + fullName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return fullName;
        }

        public IEnumerable<DocumentRuleModel> GetAllDocumentRule(int formDesignVersionId)
        {
            ServiceResult result = new ServiceResult();
            IList<DocumentRuleModel> rowModelList = null;
            try
            {
                List<DocumentRule> DocRule = this._unitOfWork.RepositoryAsync<DocumentRule>()
                                                                .Query()
                                                                .Filter(f => f.FormDesignVersionID == formDesignVersionId)
                                                                .Get().ToList();

                var elementModels = from dr in DocRule
                                    select new DocumentRuleModel
                                    {
                                        DocumentRuleID = dr.DocumentRuleID,
                                        DocumentRuleTypeID = dr.DocumentRuleTypeID,
                                        Description = dr.Description,
                                        RuleJSON = dr.RuleJSON,
                                        CompiledRuleJSON = dr.CompiledRuleJSON,
                                        IsActive = dr.IsActive,
                                        FormDesignVersionID = dr.FormDesignVersionID,
                                        TargetUIElementID = dr.TargetUIElementID
                                    };
                rowModelList = elementModels.ToList();
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return rowModelList;
        }

        public IEnumerable<DocumentRuleExtModel> GetAllExpressionRulesForImpactList(int formDesignVersionId)
        {
            ServiceResult result = new ServiceResult();
            IList<DocumentRuleExtModel> rowModelList = null;
            try
            {
                var sectionElements = (from ele in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                       join sec in this._unitOfWork.RepositoryAsync<SectionUIElement>().Get() on ele.UIElementID equals sec.UIElementID
                                       join design in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on ele.FormID equals design.FormID
                                       join designVersion in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get() on design.FormID equals designVersion.FormDesignID
                                       where designVersion.FormDesignVersionID == formDesignVersionId
                                       select ele).ToList();

                var altLabelSectionElements = (from section in sectionElements
                                               join altLabel in this._unitOfWork.RepositoryAsync<AlternateUIElementLabel>().Get()
                                               on section.UIElementID equals altLabel.UIElementID
                                               select altLabel).ToList();

                foreach (var item in altLabelSectionElements)
                {
                    sectionElements.Find(x => x.UIElementID == item.UIElementID).Label = item.AlternateLabel;
                }

                rowModelList = (from rule in this._unitOfWork.RepositoryAsync<DocumentRule>().Get()
                                join element in this._unitOfWork.RepositoryAsync<UIElement>().Get() on rule.TargetUIElementID equals element.UIElementID
                                where rule.FormDesignVersionID == formDesignVersionId
                                select new DocumentRuleExtModel
                                {
                                    TargetUIElementID = rule.TargetUIElementID,
                                    TargetElementPath = rule.TargetElementPath,
                                    TargetUIElementLabel = element.Label,
                                    TargetUIElementName = element.UIElementName,
                                    DocumentRuleID = rule.DocumentRuleID,
                                    DocumentRuleTypeID = rule.DocumentRuleTypeID,
                                    Description = rule.Description,
                                    RuleJSON = rule.RuleJSON,
                                    CompiledRuleJSON = rule.CompiledRuleJSON
                                }).ToList();

                var pbpViewAltLabelRow = this._unitOfWork.Repository<AlternateUIElementLabel>()
                                                                  .Query()
                                                                  .Filter(c => c.FormDesignVersionID == formDesignVersionId)
                                                                  .Get().ToList();

                foreach (var item in pbpViewAltLabelRow)
                {
                    var row = rowModelList.Where(x => x.TargetUIElementID == item.UIElementID).FirstOrDefault();
                    if (row != null)
                    {
                        row.TargetUIElementLabel = item.AlternateLabel;
                    }
                }

                foreach (var row in rowModelList)
                {
                    string[] path = row.TargetElementPath.Split('.');
                    if (path.Length > 1)
                    {
                        int count = 0;
                        foreach (string secName in path)
                        {
                            if (secName != path[path.Length - 1])
                            {
                                var sectionElement = sectionElements.Where(s => s.GeneratedName == secName).FirstOrDefault();
                                if (sectionElement != null)
                                {
                                    if (count == 0)
                                    {
                                        row.TargetSectionElementName = sectionElement.UIElementName;
                                        row.TargetSectionGeneratedName = secName;
                                    }
                                    var sectionName = sectionElement.Label;
                                    row.TargetElementPathLabel += !string.IsNullOrEmpty(sectionName) ? sectionName + " => " : "";
                                    count = count = 1;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(row.TargetElementPathLabel))
                        {
                            row.TargetElementPathLabel = row.TargetElementPathLabel.Substring(0, row.TargetElementPathLabel.Length - 4);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return rowModelList;
        }

        public ServiceResult UpdateCompileJSONRule(string userName, Dictionary<int, string> data)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                foreach (KeyValuePair<int, string> dr in data)
                {

                    DocumentRule ruleToUpdate = this._unitOfWork.RepositoryAsync<DocumentRule>().FindById(dr.Key);
                    ruleToUpdate.UpdatedBy = userName;
                    ruleToUpdate.UpdatedDate = DateTime.Now;
                    ruleToUpdate.CompiledRuleJSON = dr.Value.ToString();

                    this._unitOfWork.RepositoryAsync<DocumentRule>().Update(ruleToUpdate);
                }
                this._unitOfWork.Save();
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public List<DocumentRuleData> GetAllDocumentRuleForTree(int formDesignVersionId, IEnumerable<DocumentRuleModel> documentRules)
        {
            ServiceResult result = new ServiceResult();
            List<DocumentRuleData> rowModelList = null;
            try
            {
                var elementModels = (from dr in documentRules
                                     select new DocumentRuleData
                                     {
                                         DocumentId = dr.DocumentRuleID,
                                         DocumentType = GetRuleType(dr.DocumentRuleTypeID),
                                         EventType = GetEventType(dr.DocumentRuleID),
                                         CompileJson = dr.CompiledRuleJSON
                                     }).Where(g => g.EventType != "Exclude");

                rowModelList = elementModels.ToList();
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return rowModelList;
        }

        public string GetCompiledPBPViewImpacts()
        {
            string compiledImpactList = string.Empty;
            compiledImpactList = (from design in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                  join dv in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get() on design.FormID equals dv.FormDesignID
                                  where design.FormName == "PBPView"
                                  orderby dv.FormDesignVersionID descending
                                  select dv.PBPViewImpacts).FirstOrDefault();

            return compiledImpactList;
        }

        public ServiceResult UpdatePBPViewImpacts(string userName, int formDesignVersionId, string data)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    ICompressionBase handler = CompressionFactory.GetCompressionFactory(CompressionType.JSON, null, "", "", "");
                    data = handler.Compress(data).ToString();
                }

                SqlParameter paramTenantID = new SqlParameter("@TenantID", 1);
                SqlParameter paramFormDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                SqlParameter paramJsonData = new SqlParameter("@JsonData", data);
                SqlParameter paramCurrentUserName = new SqlParameter("@CurrentUserName", userName);
                SqlParameter paramUpdateFieldType = new SqlParameter("@UpdateFieldType", "PBPViewImpacts");

                FormDesignVersion frmDesignVersion = this._unitOfWork.Repository<FormDesignVersion>().ExecuteSql("exec [dbo].[uspSaveCompiledFormDesignVersionData] @TenantID,@FormDesignVersionID,@JsonData,@CurrentUserName,@UpdateFieldType", paramTenantID, paramFormDesignVersionID, paramJsonData, paramCurrentUserName, paramUpdateFieldType).FirstOrDefault();

                if (frmDesignVersion != null)
                {
                    result.Result = ServiceResultStatus.Success;
                    return result;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    return result;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult UpdateRuleTreeJSON(string userName, int formDesignVersionId, string data)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    ICompressionBase handler = CompressionFactory.GetCompressionFactory(CompressionType.JSON, null, "", "", "");
                    data = handler.Compress(data).ToString();
                }

                SqlParameter paramTenantID = new SqlParameter("@TenantID", 1);
                SqlParameter paramFormDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                SqlParameter paramJsonData = new SqlParameter("@JsonData", data);
                SqlParameter paramCurrentUserName = new SqlParameter("@CurrentUserName", userName);
                SqlParameter paramUpdateFieldType = new SqlParameter("@UpdateFieldType", "RuleExecutionTressJSON");

                FormDesignVersion frmDesignVersion = this._unitOfWork.Repository<FormDesignVersion>().ExecuteSql("exec [dbo].[uspSaveCompiledFormDesignVersionData] @TenantID,@FormDesignVersionID,@JsonData,@CurrentUserName,@UpdateFieldType", paramTenantID, paramFormDesignVersionID, paramJsonData, paramCurrentUserName, paramUpdateFieldType).FirstOrDefault();

                if (frmDesignVersion != null)
                {
                    result.Result = ServiceResultStatus.Success;
                    return result;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    return result;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public List<DocumentRuleData> GetAllDocumentRuleForEventTree(int formDesignVersionId, IEnumerable<DocumentRuleModel> documentRules)
        {
            ServiceResult result = new ServiceResult();
            List<DocumentRuleData> rowModelList = null;
            try
            {
                //var latestFormDesignVersionList = from fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                //                                  join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get().Where(c => c.IsActive == true)
                //                                  on fdv.FormDesignID equals fd.FormID
                //                                  where fdv.FormDesignVersionID
                //group fdv by fdv.FormDesignID into g
                //select new { FormDesignVersionId = g.Max(s => s.FormDesignVersionID) };

                var docList = (from dr in documentRules
                               where dr.IsActive == true && dr.CompiledRuleJSON != null

                               select new DocumentRuleData
                               {
                                   DocumentId = dr.DocumentRuleID,
                                   FormDesignVersionId = dr.FormDesignVersionID,
                                   DocumentType = GetRuleType(dr.DocumentRuleTypeID),
                                   EventType = GetEventType(dr.DocumentRuleID),
                                   CompileJson = dr.CompiledRuleJSON
                               }).Where(g => g.EventType != "Exclude");


                rowModelList = docList.ToList();
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return rowModelList;
        }

        public string GetFormName(int formDesignVersionId)
        {
            string formName = "";
            try
            {
                formName = (from fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                            join fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get() on fd.FormID equals fdv.FormDesignID
                            where fdv.FormDesignVersionID == formDesignVersionId
                            && fd.IsActive == true
                            select fd.FormName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return formName;
        }
        public string GetDocumentRule(int ruleId)
        {
            string docuementRuleJSON = string.Empty;
            DocumentRuleModel documentRule = (from rule in this._unitOfWork.Repository<DocumentRule>()
                                             .Query().Filter(whr => whr.DocumentRuleID == ruleId).Get()
                                              select new DocumentRuleModel
                                              {
                                                  CompiledRuleJSON = rule.CompiledRuleJSON,
                                                  DocumentRuleTypeID = rule.DocumentRuleTypeID,
                                                  RuleJSON = rule.RuleJSON
                                              }).FirstOrDefault();
            if (documentRule != null) { docuementRuleJSON = documentRule.CompiledRuleJSON; }
            return docuementRuleJSON;
        }

        public List<string> ViewList(int formDesignVersionId)
        {
            List<string> getView = new List<string>();
            FormDesignVersion fdv = this._unitOfWork.RepositoryAsync<FormDesignVersion>().FindById(formDesignVersionId);

            var list = (from f in this._unitOfWork.Repository<FormDesign>().Get()
                        join d in this._unitOfWork.Repository<FormDesignMapping>().Get()
                        on f.FormID equals d.AnchorDesignID
                        where (f.IsActive && f.FormID == fdv.FormDesignID)
                        select d.TargetDesignID);

            foreach (var l in list)
            {
                string g = (from s in this._unitOfWork.Repository<FormDesign>().Get()
                            where (s.FormID == l)
                            select s.FormName).FirstOrDefault();

                if (g != null)
                {
                    getView.Add(g);
                }

            }



            return getView;
        }


        public ServiceResult UpdateEventTreeJSON(string userName, int formDesignVersionId, string data)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    ICompressionBase handler = CompressionFactory.GetCompressionFactory(CompressionType.JSON, null, "", "", "");
                    data = handler.Compress(data).ToString();
                }

                SqlParameter paramTenantID = new SqlParameter("@TenantID", 1);
                SqlParameter paramFormDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                SqlParameter paramJsonData = new SqlParameter("@JsonData", data);
                SqlParameter paramCurrentUserName = new SqlParameter("@CurrentUserName", userName);
                SqlParameter paramUpdateFieldType = new SqlParameter("@UpdateFieldType", "RuleEventMapJSON");

                FormDesignVersion frmDesignVersion = this._unitOfWork.Repository<FormDesignVersion>().ExecuteSql("exec [dbo].[uspSaveCompiledFormDesignVersionData] @TenantID,@FormDesignVersionID,@JsonData,@CurrentUserName,@UpdateFieldType", paramTenantID, paramFormDesignVersionID, paramJsonData, paramCurrentUserName, paramUpdateFieldType).FirstOrDefault();

                if (frmDesignVersion != null)
                {
                    result.Result = ServiceResultStatus.Success;
                    return result;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    return result;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public List<SourceDesignDetails> GetParentDesignDetails(int folderVersionId, int anchorDesigId, int formInstanceId)
        {

            var anchorDesignDetails = (from designMappings in this._unitOfWork.RepositoryAsync<FormDesignMapping>().Query().Filter(m => m.AnchorDesignID == anchorDesigId).Get() //this._unitOfWork.RepositoryAsync<FormDesignMapping>().Query().Filter(x=>x.AnchorDesignID==anchorDesigId).Get()
                                       join formdesign in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                       on designMappings.TargetDesignID equals formdesign.FormID
                                       join formInstance in this._unitOfWork.RepositoryAsync<FormInstance>().Query().Filter(whr => whr.FolderVersionID == folderVersionId && whr.AnchorDocumentID == formInstanceId).Get()
                                       on designMappings.TargetDesignID equals formInstance.FormDesignID
                                       join formDesignVersion in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                                       on formInstance.FormDesignVersionID equals formDesignVersion.FormDesignVersionID
                                       select new SourceDesignDetails
                                       {
                                           FormName = formdesign.FormName,
                                           FormDesignVersionId = formInstance.FormDesignVersionID,
                                           FormDesignId = formInstance.FormDesignID,
                                           FormInstanceId = formInstance.FormInstanceID,
                                           RuleEventTree = formDesignVersion.RuleEventMapJSON
                                       }
                                    ).ToList();

            return anchorDesignDetails;
        }

        public List<ConfigRulesTesterData> GetFormDesignVersionUIElementsTestData(int tenantId, int formDesignVersionId, int elementId)
        {
            return this._unitOfWork.RepositoryAsync<ConfigRulesTesterData>().Get().Where(x => x.FormDesignVersionId == formDesignVersionId && x.UIElementId == elementId).ToList();
        }
        public ServiceResult SaveConfigurationRuleTesterData(int tenantId, string currentUserName, string designRulesTesterData)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                List<JToken> sources = JToken.Parse(designRulesTesterData).ToList();
                foreach (JToken source in sources)
                {
                    int designRuleTesterId = (int)source["designRuleTesterId"];
                    ConfigRulesTesterData configRulesTesterData = this._unitOfWork.RepositoryAsync<ConfigRulesTesterData>().Get().Where(x => x.RuleTersterId == designRuleTesterId).FirstOrDefault();

                    if (configRulesTesterData != null)
                    {
                        configRulesTesterData.FormDesignVersionId = (int)source["formDesignVersionId"];
                        configRulesTesterData.UIElementId = (int)source["UIElementId"];
                        configRulesTesterData.RuleId = (int)source["ruleId"];
                        configRulesTesterData.TestData = source["testDataJson"].ToString();
                        configRulesTesterData.IsActive = ((string)source["isActive"]) == "1" ? true : false;
                        configRulesTesterData.UpdatedDate = DateTime.Now;
                        configRulesTesterData.UpdatedBy = currentUserName;

                        this._unitOfWork.RepositoryAsync<ConfigRulesTesterData>().Update(configRulesTesterData);
                        this._unitOfWork.Save();
                    }
                    else
                    {
                        ConfigRulesTesterData _configRulesTesterData = new ConfigRulesTesterData();
                        _configRulesTesterData.FormDesignVersionId = (int)source["formDesignVersionId"];
                        _configRulesTesterData.UIElementId = (int)source["UIElementId"];
                        _configRulesTesterData.RuleId = (int)source["ruleId"];
                        _configRulesTesterData.TestData = source["testDataJson"].ToString();
                        _configRulesTesterData.IsActive = ((string)source["isActive"]) == "1" ? true : false;
                        _configRulesTesterData.AddedDate = DateTime.Now;
                        _configRulesTesterData.AddedBy = currentUserName;

                        this._unitOfWork.RepositoryAsync<ConfigRulesTesterData>().Insert(_configRulesTesterData);
                        this._unitOfWork.Save();

                        IList<ServiceResultItem> item = new List<ServiceResultItem>();
                        item.Add(new ServiceResultItem { Messages = new string[] { _configRulesTesterData.RuleTersterId.ToString() } });
                        result.Items = item;
                    }
                }
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        #endregion Public Methods



    }
}
