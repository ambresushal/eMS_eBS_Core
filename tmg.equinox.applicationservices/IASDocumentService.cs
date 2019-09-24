using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using tmg.equinox;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.GlobalUpdate;
using tmg.equinox.applicationservices.viewmodels.UIElement;
//using tmg.equinox.documentmatch;
using tmg.equinox.domain.entities.Enums;
using FolderVersionState = tmg.equinox.domain.entities.Enums.FolderVersionState;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.domain.entities.Utility;
using tmg.equinox.iasbuilder;
using tmg.equinox.iasbuilder.SheetBuilder;
using tmg.equinox.iasexcelbuilder;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
//using tmg.equinox.documentmatch.RulesEngine;
using Newtonsoft.Json.Linq;

namespace tmg.equinox.applicationservices
{
    public class IASDocumentService : IIASDocumentService
    {
        #region Private Memebers

        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private Dictionary<int, ExpandoObject> sourceInstances = new Dictionary<int, ExpandoObject>();
        
        #endregion Private Members

        #region Public Properties

        public ExpandoObject sourceDataObj;
        public ExpandoObjectConverter converter;
        public List<UIElement> formElementList;

        public static int ValidationErrorFlag = 1;
        public static int RuleErrorFlag = 2;
        public static int DataSourceErrorFlag = 3;

        public enum TargetPropertyTypes
        {
            Enabled = 1,
            RunValidation = 2,
            Value = 3,
            Visible = 4,
            IsRequired = 5,
            Error = 6
        }

        public enum OperatorTypes
        {
            Equals = 1,
            GreaterThan = 2,
            LessThan = 3,
            Contains = 4,
            NotEquals = 5,
            GreaterThanOrEqualTo = 6,
            LessThanOrEqualTo = 7,
            IsNull = 8
        }

        #endregion Public Properties

        #region Constructor

        public IASDocumentService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        #endregion Constructor

        #region Public Methods

        public List<IASElementExportDataModel> GetGlobalUpdatesDataList(int GlobalUpdateID)
        {
            List<IASElementExportDataModel> globalUpdatesDataList = null;

            try
            {
                var globalUpdatesData = (from j in this._unitOfWork.Repository<IASElementExport>()
                                                            .Query()
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID)
                                                            .Get()
                                         join rd in this._unitOfWork.Repository<RadioButtonUIElement>().Get()
                                         on j.UIElement.UIElementID equals rd.UIElementID
                                         into radioElement
                                         from radiolist in radioElement.DefaultIfEmpty()
                                         select new IASElementExportDataModel
                                         {
                                             FormDesignID = j.IASFolderMap.FormInstance.FormDesignID,
                                             AcceptChange = j.AcceptChange,
                                             FormInstanceID = j.IASFolderMap.FormInstanceID,
                                             Label = j.ElementHeaderName,
                                             NewValue = j.NewValue,
                                             OldValue = j.OldValue,
                                             IASFolderMapID = j.IASFolderMapID,
                                             UIElementID = j.UIElementID,
                                             UIElementTypeID = j.UIElementTypeID,
                                             OptionLabel = (radiolist.OptionLabel != null) ? radiolist.OptionLabel.ToString() : null,
                                             OptionLabelNo = (radiolist.OptionLabelNo != null) ? radiolist.OptionLabelNo.ToString() : null,
                                             ItemData = null
                                         });
                if (globalUpdatesData != null)
                {
                    globalUpdatesDataList = globalUpdatesData.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return globalUpdatesDataList;
        }

        public List<FormDesignElementValueVeiwModel> GetGlobalUpdatesDesignDocumentList(int GlobalUpdateID)
        {
            List<FormDesignElementValueVeiwModel> globalUpdatesDesignDocumentsList = null;

            try
            {
                var globalUpdatesData = (from j in this._unitOfWork.Repository<FormDesignElementValue>()
                                                            .Query()
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID)
                                                            .Get()

                                         select new FormDesignElementValueVeiwModel
                                         {
                                             FormDesignID = j.FormDesignID,
                                             FormDesignName = j.FormDesign.DisplayText
                                         }).Distinct();
                if (globalUpdatesData != null)
                {
                    globalUpdatesDesignDocumentsList = globalUpdatesData.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return globalUpdatesDesignDocumentsList;
        }

        public ServiceResult ExportIASExcelTemplate(int GlobalUpdateID, string GlobalUpdateName, DateTime GlobalUpdateEffectiveDateFrom, DateTime GlobalUpdateEffectiveDateTo, string folderPath, out string filePath)
        {
            ServiceResult result = new ServiceResult();
            filePath = string.Empty;
            try
            {
                IASBuilder builder = new IASBuilder();
                IASSheet sheet = new IASSheet();

                List<IASFolderDataModel> DocumentInfo = GetGlobalUpdatesFolderDataList(GlobalUpdateID);
                List<FormDesignElementValueVeiwModel> ElementSelectionInfo = GetFormDesignVersionUIElements(GlobalUpdateID);
                List<IASElementExportDataModel> ElementUpdateInfo = GetGlobalUpdatesDataList(GlobalUpdateID);
                List<FormDesignElementValueVeiwModel> designdocumentsInfo = GetGlobalUpdatesDesignDocumentList(GlobalUpdateID);

                //Add ItemData values as expressions string in ElementSelectionInfo
                foreach (var ElementSelection in ElementSelectionInfo)
                {
                    string resultDescription = string.Empty;
                    int FormDesignVersionID = Convert.ToInt32(ElementSelection.FormDesignVersionID);
                    formElementList = GetUIElemenstFromFormDesignVersion(FormDesignVersionID);
                    IEnumerable<GuRuleRowModel> uiElementRules = GetRulesForUIElement(1, FormDesignVersionID, ElementSelection.UIElementID, GlobalUpdateID);
                    if (uiElementRules != null)
                    {
                        List<GuRuleRowModel> rulesFilter = uiElementRules.ToList();
                        foreach (var rule in rulesFilter)
                        {
                            if (rule.RootExpression == null)
                            {
                                resultDescription = "";
                            }
                            else
                            {
                                resultDescription = ProcessNode(rule.RootExpression);
                            }
                        }
                    }
                    if (resultDescription != "")
                    {
                        ElementSelection.ItemData = resultDescription;
                    }
                    else
                    {
                        ElementSelection.ItemData = null;
                    }
                }

                sheet.Config = builder.BuildConfig(GlobalUpdateID, DocumentInfo, ElementUpdateInfo, ElementSelectionInfo);
                sheet.DocumentInstance = builder.BuildDocumentInstance(GlobalUpdateID, sheet.Config, designdocumentsInfo);

                IASExcelBuilder excelBuilder = new IASExcelBuilder();
                filePath = excelBuilder.ExportToExcel(GlobalUpdateID, GlobalUpdateName, GlobalUpdateEffectiveDateFrom, GlobalUpdateEffectiveDateTo, folderPath, sheet.DocumentInstance);
                //IAS Download is Complete
                UpdateGlobalUpdateIASStatus(GlobalUpdateID, (int)GlobalUpdateIASStatus.IASDownloadComplete);
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

        public ServiceResult ValidateIASExcelTemplate(int GlobalUpdateID, string GlobalUpdateName, DateTime GlobalUpdateEffectiveDateFrom, DateTime GlobalUpdateEffectiveDateTo, string importPath, string addedBy, string folderPath, out string filePath)
        {
            ServiceResult serviceResult = new ServiceResult();
            filePath = string.Empty;
            try
            {
                IASBuilder builder = new IASBuilder();
                IASSheet sheet = new IASSheet();
                IASExcelBuilder excelBuilder = new IASExcelBuilder();

                List<IASFolderDataModel> DocumentInfo = GetGlobalUpdatesFolderDataList(GlobalUpdateID);
                //List<FormDesignElementValueVeiwModel> ElementSelectionInfo = GetFormDesignVersionUIElements(GlobalUpdateID);
                List<IASElementExportDataModel> ElementUpdateInfo = GetGlobalUpdatesDataList(GlobalUpdateID);
                List<FormDesignElementValueVeiwModel> designdocumentsInfo = GetGlobalUpdatesDesignDocumentList(GlobalUpdateID);

                //Delete existing data in GU.ErrorLog table before validating new IAS file.
                DeleteErrorLog(GlobalUpdateID);
                foreach (var designdocument in designdocumentsInfo)
                {
                    DataTable dt = excelBuilder.getDataTableFromExcel(importPath, designdocument.FormDesignName);
                    if (dt != null)
                    {
                        CheckIASElementImportDataErrorLog(GlobalUpdateID, designdocument.FormDesignID, dt, addedBy);
                    }
                }

                List<ErrorLogViewModel> ErrorLogInfo = GetGlobalUpdatesErrorLogList(GlobalUpdateID);
                if (ErrorLogInfo.Count > 0)
                {
                    string csv = builder.BuildErrorLogRow(GlobalUpdateID, ErrorLogInfo);
                    filePath = excelBuilder.ExportToErrorLogExcel(GlobalUpdateID, GlobalUpdateName, GlobalUpdateEffectiveDateFrom, GlobalUpdateEffectiveDateTo, folderPath, csv);
                    //Error Log Download is Complete
                    UpdateGlobalUpdateIASStatus(GlobalUpdateID, (int)GlobalUpdateIASStatus.ErrorLogDownloadComplete);
                }
                else
                {
                    DeleteIASElementImportLog(GlobalUpdateID);
                    foreach (var designdocument in designdocumentsInfo)
                    {
                        DataTable dt = excelBuilder.getDataTableFromExcel(importPath, designdocument.FormDesignName);
                        if (dt != null)
                        {
                            serviceResult = IASElementImportData(GlobalUpdateID, designdocument.FormDesignID, dt, addedBy);
                        }
                    }
                    UpdateGlobalUpdateIASStatus(GlobalUpdateID, (int)GlobalUpdateIASStatus.Complete);
                }

                serviceResult.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                serviceResult = ex.ExceptionMessages();
                serviceResult.Result = ServiceResultStatus.Failure;

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return serviceResult;
        }

        private void CheckIASElementImportDataErrorLog(int GlobalUpdateID, int FormDesignID, DataTable dt, string addedBy)
        {
            try
            {
                //Code optimization changes to check validations columnwise instead of earlier rowwise implementation.
                int totalColumn = dt.Columns.Count;
                int checkCount = 0;
                for (int c = 10; c < totalColumn; c++)
                {
                    checkCount++;
                    if (checkCount % 6 == 0)
                    {
                        foreach (DataRow insertRow in dt.Rows)
                        {
                            int FormInstanceID = Convert.ToInt32(Convert.ToString(insertRow["FormInstanceID"]));
                            int IASElementExportID = 0;
                            int newValueColumn = c - 2;
                            int acceptChangeColumn = c - 1;
                            string elementFullName = Convert.ToString(insertRow[c]);
                            string newValue = Convert.ToString(insertRow[newValueColumn]);
                            string acceptChange = Convert.ToString(insertRow[acceptChangeColumn]);
                            if (acceptChange == GlobalUpdateConstants.YES)
                            {
                                //check for Validations, Custom Validation like Custom Regex, TextBox MaxLength criteria, Date format & Min/Max criteria etc.
                                IASElementExportID = CheckIASElementImportDataValidations(GlobalUpdateID, FormDesignID, insertRow, elementFullName, newValue, addedBy);
                                //check for Rules Source & Target.
                                IASElementExportID = CheckIASElementImportDataRules(GlobalUpdateID, FormDesignID, insertRow, IASElementExportID, elementFullName, newValue, addedBy);
                                //check for Dropdown Items & DataSources.
                                IASElementExportID = CheckIASElementImportDataDatasources(GlobalUpdateID, FormDesignID, insertRow, IASElementExportID, elementFullName, newValue, addedBy);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
        }

        private int CheckIASElementImportDataValidations(int GlobalUpdateID, int FormDesignID, DataRow insertRow, string elementFullName, string newValue, string addedBy)
        {
            int IASElementExportID = 0;
            try
            {
                List<FormDesignElementValueVeiwModel> ElementSelectionInfo = GetFormDesignVersionElementSelection(GlobalUpdateID, FormDesignID);
                string AccountName = Convert.ToString(insertRow["Account or Portfolio"]);
                int FolderID = Convert.ToInt32(Convert.ToString(insertRow["FolderID"]));
                string FolderName = Convert.ToString(insertRow["Folder Name"]);
                DateTime EffectiveDate = Convert.ToDateTime(Convert.ToString(insertRow["Folder Effective Date"]));
                string FormName = Convert.ToString(insertRow["Document Name"]);
                string Owner = Convert.ToString(insertRow["Owner"]);
                int FormInstanceID = Convert.ToInt32(Convert.ToString(insertRow["FormInstanceID"]));
                int FolderVersionID = Convert.ToInt32(Convert.ToString(insertRow["FolderVersionID"]));
                int IASFolderMapID = GetIASFolderMapID(GlobalUpdateID, FormInstanceID);

                int FormDesignVersionIDValue = (from formInstance in this._unitOfWork.RepositoryAsync<FormInstance>().Query()
                                                  .Filter(fil => fil.FormInstanceID == FormInstanceID)
                                                  .Get()
                                                select formInstance.FormDesignVersionID).FirstOrDefault();

                StringBuilder sb = new StringBuilder();
                FormDesignElementValueVeiwModel ElementSelection = ElementSelectionInfo.Where(cr => cr.ElementFullPath == elementFullName).FirstOrDefault();
                //list of validations
                Validator validator = (from v in this._unitOfWork.RepositoryAsync<Validator>().Query()
                                            .Get()
                                       join fd in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Query()
                                            .Get()
                                       on v.UIElementID equals fd.UIElementID
                                       where fd.FormDesignVersionID == FormDesignVersionIDValue
                                             && v.UIElementID == ElementSelection.UIElementID
                                             && v.IsActive == true
                                       select v).FirstOrDefault();
                if (validator != null)
                {
                    string RegexValue = string.Empty;
                    string ValidationMessageValue = string.Empty;
                    string LibraryRegexNameValue = string.Empty;
                    int DataTypeValue = 0;
                    string IsRequiredValue = string.Empty;

                    if (!(GlobalUpdateConstants.equalsList.Contains(newValue)))
                    {
                        if (validator.Regex != null)
                        {
                            RegexValue = Convert.ToString(validator.Regex);
                        }
                        else
                        {
                            RegexValue = (from rl in this._unitOfWork.RepositoryAsync<RegexLibrary>()
                                                    .Query()
                                                    .Get()
                                          where rl.LibraryRegexID == validator.LibraryRegexID
                                          select rl.RegexValue).FirstOrDefault();
                        }

                        ValidationMessageValue = Convert.ToString(validator.Message);

                        DataTypeValue = (from dt in this._unitOfWork.RepositoryAsync<UIElement>()
                                                .Query()
                                                .Get()
                                         where dt.UIElementID == validator.UIElementID
                                         select dt.UIElementDataTypeID).FirstOrDefault();

                        LibraryRegexNameValue = (from rl in this._unitOfWork.RepositoryAsync<RegexLibrary>()
                                                            .Query()
                                                            .Get()
                                                 where rl.LibraryRegexID == validator.LibraryRegexID
                                                 select rl.LibraryRegexName).FirstOrDefault();

                        //Regex & Custom Regex check
                        if (!(GlobalUpdateConstants.equalsList.Contains(RegexValue)))
                        {
                            Regex regex = new Regex(@RegexValue);
                            if (!(regex.IsMatch(newValue)))
                            {
                                if (!(GlobalUpdateConstants.equalsList.Contains(LibraryRegexNameValue)))
                                {
                                    sb.Append(String.Format(ValidationMessageData.regexMsg, newValue, LibraryRegexNameValue));
                                }
                                else if (!(GlobalUpdateConstants.equalsList.Contains(ValidationMessageValue)))
                                {
                                    sb.Append(String.Format(ValidationMessageData.regexMsg, newValue, ValidationMessageValue));
                                }
                            }
                        }
                        //
                        //DataType check
                        if (DataTypeValue == 1)
                        {
                            Regex regex = new Regex(@"^\d+$");
                            if (!(regex.IsMatch(newValue)))
                            {
                                sb.Append(String.Format(ValidationMessageData.invalidIntMsg, newValue));
                            }
                        }
                        //
                    }
                    IsRequiredValue = Convert.ToString(validator.IsRequired);
                    //IsRequired check
                    if (GlobalUpdateConstants.trueList.Contains(IsRequiredValue))
                    {
                        if (GlobalUpdateConstants.equalsList.Contains(newValue) || newValue == GlobalUpdateConstants.NA)
                        {
                            sb.Append(String.Format(ValidationMessageData.requiredMsg));
                        }
                    }
                    //
                }

                if (!(GlobalUpdateConstants.equalsList.Contains(newValue)))
                {
                    var element = (from i in this._unitOfWork.Repository<UIElement>()
                                                .Query()
                                                .Filter(c => c.UIElementID == ElementSelection.UIElementID)
                                                .Get()
                                   select i).FirstOrDefault();
                    //MaxLength check
                    if (element is TextBoxUIElement)
                    {
                        int MaxLengthValue = ((TextBoxUIElement)element).MaxLength;
                        if (newValue.Length > MaxLengthValue)
                        {
                            sb.Append(String.Format(ValidationMessageData.invalidMaxLengthMsg, newValue, MaxLengthValue));
                        }
                    }
                    //
                    //Date Min/Max & format check
                    if (ElementSelection.UIElementTypeID == ElementTypes.CALENDARID)
                    {
                        Regex regex = new Regex(@"^(((0?[1-9]|1[012])\/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])\/(29|30)|(0?[13578]|1[02])\/31)\/(19|[2-9]\d)\d{2}|0?2\/29\/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00)))$");
                        if (!(regex.IsMatch(newValue)))
                        {
                            sb.Append(String.Format(ValidationMessageData.invalidDateMsg, newValue));
                        }
                        else if (Convert.ToDateTime(newValue) < ((CalendarUIElement)element).MinDate || Convert.ToDateTime(newValue) > ((CalendarUIElement)element).MaxDate)
                        {
                            string minDate = Convert.ToDateTime(((CalendarUIElement)element).MinDate).ToShortDateString();
                            string maxDate = Convert.ToDateTime(((CalendarUIElement)element).MaxDate).ToShortDateString();
                            sb.Append(String.Format(ValidationMessageData.invalidDateRangeMsg, newValue, minDate, maxDate));
                        }
                    }
                    //
                }
                if (sb.Length > 0)
                {
                    IASElementExportID = GetIASElementExportID(IASFolderMapID, ElementSelection.UIElementID);
                    //Save Validation error data to GU.ErrorLog table
                    SaveErrorLogData(GlobalUpdateID, IASElementExportID, sb, addedBy, ValidationErrorFlag);
                    return IASElementExportID;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return IASElementExportID;
        }

        private int CheckIASElementImportDataRules(int GlobalUpdateID, int FormDesignID, DataRow insertRow, int IASElementExportID, string elementFullName, string newValue, string addedBy)
        {
            try
            {
                List<FormDesignElementValueVeiwModel> ElementSelectionInfo = GetFormDesignVersionElementSelection(GlobalUpdateID, FormDesignID);
                string AccountName = Convert.ToString(insertRow["Account or Portfolio"]);
                int FolderID = Convert.ToInt32(Convert.ToString(insertRow["FolderID"]));
                string FolderName = Convert.ToString(insertRow["Folder Name"]);
                DateTime EffectiveDate = Convert.ToDateTime(Convert.ToString(insertRow["Folder Effective Date"]));
                string FormName = Convert.ToString(insertRow["Document Name"]);
                string Owner = Convert.ToString(insertRow["Owner"]);
                int FormInstanceID = Convert.ToInt32(Convert.ToString(insertRow["FormInstanceID"]));
                int FolderVersionID = Convert.ToInt32(Convert.ToString(insertRow["FolderVersionID"]));
                int IASFolderMapID = GetIASFolderMapID(GlobalUpdateID, FormInstanceID);
                int FormDesignVersionID = (from formInstance in this._unitOfWork.RepositoryAsync<FormInstance>().Query()
                                                  .Filter(fil => fil.FormInstanceID == FormInstanceID)
                                                  .Get()
                                                select formInstance.FormDesignVersionID).FirstOrDefault();
                FormDesignElementValueVeiwModel ElementSelection = ElementSelectionInfo.Where(cr => cr.ElementFullPath == elementFullName).FirstOrDefault();
                formElementList = GetUIElemenstFromFormDesignVersion(FormDesignVersionID);
                //Code to check rules present in current FormDesignVersionID
                List<RuleDesign> rules = new List<RuleDesign>();
                rules = (from r in this._unitOfWork.RepositoryAsync<PropertyRuleMap>()
                                                             .Query()
                                                             .Include(c => c.Rule)
                                                             .Include(d => d.Rule.Expressions)
                                                            .Get()
                             join rfd in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                     .Query()
                                                     .Get()
                             on r.UIElementID equals rfd.UIElementID
                             where rfd.FormDesignVersionID == FormDesignVersionID
                             select new RuleDesign
                             {
                                 RuleID = r.RuleID,
                                 SuccessValue = r.Rule.ResultSuccess,
                                 FailureValue = r.Rule.ResultFailure,
                                 UIELementID = r.UIElementID,
                                 TargetPropertyTypeId = r.TargetPropertyID,
                                 IsResultFailureElement = r.Rule.IsResultFailureElement,
                                 IsResultSuccessElement = r.Rule.IsResultSuccessElement,
                                 Message = r.Rule.Message,
                                 Expressions = (from e in r.Rule.Expressions
                                                select new ExpressionDesign
                                                {
                                                    ExpressionId = e.ExpressionID,
                                                    LeftOperand = e.LeftOperand,
                                                    RightOperand = e.RightOperand == null ? "" : e.RightOperand,
                                                    IsRightOperandElement = e.IsRightOperandElement,
                                                    OperatorTypeId = e.OperatorTypeID,
                                                    LogicalOperatorTypeId = e.LogicalOperatorTypeID,
                                                    ExpressionTypeId = e.ExpressionTypeID,
                                                    ParentExpressionId = e.ParentExpressionID
                                                })
                             }).ToList();

                StringBuilder sb = new StringBuilder();
                if (ElementSelection != null && rules.Count > 0)
                {
                    foreach (var rule in rules)
                    {
                        rule.IsParentRepeater = IsParentRepeater(rule.UIELementID);
                        rule.UIElementFullName = GetFullNameFromID(rule.UIELementID);
                        string ruleTypeText = GetRuleTypeText(rule.TargetPropertyTypeId);

                        //Check/Confirm if current Element value is used as Source in a Rule; in Expressions as either LeftOperand or RightOperand.
                        if (rule.Expressions != null && rule.Expressions.Count() > 0)
                        {
                            rule.Expressions = (from exp in rule.Expressions where exp != null select exp).ToList();
                        }
                        if (rule.Expressions != null && rule.Expressions.Count() > 0)
                        {
                            foreach (var exp in rule.Expressions)
                            {
                                if (exp != null)
                                {
                                    if (exp.LeftOperand == ElementSelection.UIElementName || exp.RightOperand == ElementSelection.UIElementName)
                                    {
                                        FormInstanceDataMap formInstanceDataMap = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                        .Query()
                                                                        .Include(c => c.FormInstance)
                                                                        .Filter(c => c.FormInstanceID == FormInstanceID)
                                                                        .Get().FirstOrDefault();
                                        string json = formInstanceDataMap.FormData;
                                        JObject sourceData = JObject.Parse(json);

                                        string _containerName = "";
                                        JObject rowData = null;
                                        if (rule.IsParentRepeater == true && rule.RootExpression != null)
                                        {
                                            _containerName = Convert.ToString(rule.UIElementFullName.Substring(0, (rule.UIElementFullName.LastIndexOf('.') + 1)));
                                            string fullPath = Convert.ToString(ElementSelection.ElementFullPath.Substring(0, ElementSelection.ElementFullPath.LastIndexOf('.')));
                                            var sourceValue = GetCurrentValue(json, fullPath);
                                            if (sourceValue is IList<object>)
                                            {
                                                var srList = sourceValue as System.Collections.Generic.IList<Object>;
                                                int srCount = srList.Count();
                                                if (srCount > 0)
                                                {
                                                    string jsonListDefault = JsonConvert.SerializeObject(srList[0]);
                                                    rowData = JObject.Parse(jsonListDefault);                
                                                }
                                            }
                                        }

                                        exp.LeftOperandName = GetFullNameFromName(exp.LeftOperand);
                                        if (exp.IsRightOperandElement == true)
                                        {
                                            exp.RightOperandName = GetFullNameFromName(exp.RightOperand);
                                        }
                                        string leftOperand = string.Empty;
                                        string rightOperand = string.Empty;
                                        if (exp.LeftOperand == ElementSelection.UIElementName)
                                        {
                                            leftOperand = newValue;
                                            rightOperand = exp.RightOperand;

                                            if (exp.IsRightOperandElement) { rightOperand = GetOperandValue(exp.RightOperandName, exp.RightOperand, sourceData, _containerName, rowData); }
                                        }
                                        else if (exp.RightOperand == ElementSelection.UIElementName)
                                        {
                                            leftOperand = GetOperandValue(exp.LeftOperandName, exp.LeftOperand, sourceData, _containerName, rowData);
                                            rightOperand = newValue;
                                        }
                                        bool result = this.EvaluateExpression(leftOperand, exp.OperatorTypeId, rightOperand, exp.IsRightOperandElement, exp.LeftOperand);
                                        if (rule.TargetPropertyTypeId != (int)TargetPropertyTypes.Value)
                                        {
                                            string operatorTypeSub = string.Empty;
                                            string operatorTypeName = GetOperatorTypeName(exp.OperatorTypeId);
                                            //check for Enabled, Visible, Error rule failure.
                                            if (rule.TargetPropertyTypeId != (int)TargetPropertyTypes.IsRequired && rule.TargetPropertyTypeId != (int)TargetPropertyTypes.RunValidation)
                                            {
                                                operatorTypeSub = "should be";
                                                if (!result)
                                                {
                                                    if (exp.OperatorTypeId == (int)OperatorTypes.IsNull)
                                                    {
                                                        operatorTypeName = "null";
                                                        rightOperand = "";
                                                    }
                                                    sb.Append(String.Format(ValidationMessageData.invalidRuleSourceValueMsg, newValue, ruleTypeText, rule.UIElementFullName, operatorTypeName, rightOperand, operatorTypeSub));
                                                }
                                            }
                                            //check for IsRequired, RunValidation rule failure.
                                            else if (result)
                                            {
                                                operatorTypeSub = "should not be";
                                                if (exp.OperatorTypeId == (int)OperatorTypes.IsNull)
                                                {
                                                    operatorTypeName = "null";
                                                    rightOperand = "";
                                                }
                                                sb.Append(String.Format(ValidationMessageData.invalidRuleSourceValueMsg, newValue, ruleTypeText, rule.UIElementFullName, operatorTypeName, rightOperand, operatorTypeSub));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //Check/Confirm if current Element is used as Target in a Value Rule.
                        if (rule.TargetPropertyTypeId == (int)TargetPropertyTypes.Value && rule.UIELementID == ElementSelection.UIElementID)
                        {
                            string ruleSuccessValue = rule.SuccessValue;
                            string ruleFailureValue = rule.FailureValue;
                            if (rule.IsResultSuccessElement == true)
                            {
                                rule.SuccessValueFullName = GetFullNameFromName(rule.SuccessValue);
                                ruleSuccessValue = rule.SuccessValueFullName;
                            }
                            if (rule.IsResultFailureElement == true)
                            {
                                rule.FailureValueFullName = GetFullNameFromName(rule.FailureValue);
                                ruleFailureValue = rule.FailureValueFullName;
                            }
                            sb.Append(String.Format(ValidationMessageData.invalidRuleTargetValueMsg, newValue, ruleTypeText, ruleSuccessValue, ruleFailureValue));
                        }
                        //Check/Confirm if current Element is used as Target in a Required Rule.
                        if (rule.TargetPropertyTypeId == (int)TargetPropertyTypes.IsRequired && rule.UIELementID == ElementSelection.UIElementID)
                        {
                            bool targetElementValue = false;
                            if (rule.Expressions != null && rule.Expressions.Count() > 0)
                                rule.RootExpression = GenerateHierarchicalExpression(rule.Expressions);
                            
                            if (rule.RootExpression != null)
                            {
                                targetElementValue = this.ProcessNodeElement(newValue, FormInstanceID, rule, ElementSelection, rule.RootExpression);
                            }
                            
                            if (targetElementValue)
                            {
                                if (GlobalUpdateConstants.equalsList.Contains(newValue) || newValue == GlobalUpdateConstants.NA)
                                {
                                    sb.Append(String.Format(ValidationMessageData.requiredTargetValueMsg, newValue, ruleTypeText));
                                }
                            }
                        }
                    }
                    if (sb.Length > 0)
                    {
                        if (IASElementExportID != 0)
                        {
                            //Update Rules error data to GU.ErrorLog table
                            UpdateErrorLogData(GlobalUpdateID, IASElementExportID, sb, addedBy, RuleErrorFlag);
                            return IASElementExportID;
                        }
                        else
                        {
                            IASElementExportID = GetIASElementExportID(IASFolderMapID, ElementSelection.UIElementID);
                            //Save Rules error data to GU.ErrorLog table
                            SaveErrorLogData(GlobalUpdateID, IASElementExportID, sb, addedBy, RuleErrorFlag);
                            return IASElementExportID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return IASElementExportID;
        }
        //Process list of expressions
        private bool ProcessNodeElement(string newValue, int FormInstanceID, RuleDesign rule, FormDesignElementValueVeiwModel ElementSelection, ExpressionDesign expression)
        {
            //loop through all the expression
            //Call ProcessLeaf to evaluate single expression
            bool isSuccess = expression.LogicalOperatorTypeId == (int)LogicalOperatorTypes.AND ? true : false;
            if (expression.Expressions != null && expression.Expressions.Count > 0)
            {
                for (var idx = 0; idx < expression.Expressions.Count; idx++)
                {
                    var exp = expression.Expressions[idx];
                    bool result = exp.ExpressionTypeId == (int)ExpressionTypes.NODE ? this.ProcessNodeElement(newValue, FormInstanceID, rule, ElementSelection,expression) : ProcessLeafElement(newValue, FormInstanceID, rule, ElementSelection, exp);

                    if (expression.LogicalOperatorTypeId == (int)LogicalOperatorTypes.AND)
                    {
                        if (result == false)
                        {
                            isSuccess = false;
                            break;
                        }
                    }
                    else
                    {
                        if (result == true)
                        {
                            isSuccess = true;
                            break;
                        }
                    }
                }
            }
            return isSuccess;
        }
        public enum LogicalOperatorTypes
        {
            AND = 1,
            OR = 2
        }
        public enum ExpressionTypes
        {
            NODE = 1,
            LEAF = 2
        }
        private bool ProcessLeafElement(string newValue, int FormInstanceID, RuleDesign rule, FormDesignElementValueVeiwModel ElementSelection, ExpressionDesign exp)
        {
            bool result = false;
            FormInstanceDataMap formInstanceDataMap = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                        .Query()
                                                                        .Include(c => c.FormInstance)
                                                                        .Filter(c => c.FormInstanceID == FormInstanceID)
                                                                        .Get().FirstOrDefault();
            string json = formInstanceDataMap.FormData;
            JObject sourceData = JObject.Parse(json);

            string _containerName = "";
            JObject rowData = null;
            if (rule.IsParentRepeater == true && rule.RootExpression != null)
            {
                _containerName = Convert.ToString(rule.UIElementFullName.Substring(0, (rule.UIElementFullName.LastIndexOf('.') + 1)));
                string fullPath = Convert.ToString(ElementSelection.ElementFullPath.Substring(0, ElementSelection.ElementFullPath.LastIndexOf('.')));
                var sourceValue = GetCurrentValue(json, fullPath);
                if (sourceValue is IList<object>)
                {
                    var srList = sourceValue as System.Collections.Generic.IList<Object>;
                    int srCount = srList.Count();
                    if (srCount > 0)
                    {
                        string jsonListDefault = JsonConvert.SerializeObject(srList[0]);
                        rowData = JObject.Parse(jsonListDefault);
                    }
                }
            }

            exp.LeftOperandName = GetFullNameFromName(exp.LeftOperand);
            if (exp.IsRightOperandElement == true)
            {
                exp.RightOperandName = GetFullNameFromName(exp.RightOperand);
            }
            string leftOperand = string.Empty;
            string rightOperand = string.Empty;
            leftOperand = GetOperandValue(exp.LeftOperandName, exp.LeftOperand, sourceData, _containerName, rowData);
            rightOperand = exp.RightOperand;
            if (exp.IsRightOperandElement) { rightOperand = GetOperandValue(exp.RightOperandName, exp.RightOperand, sourceData, _containerName, rowData); }
            result = this.EvaluateExpression(leftOperand, exp.OperatorTypeId, rightOperand, exp.IsRightOperandElement, exp.LeftOperand);
                                        
            return result;
        }
        private ExpressionDesign GenerateHierarchicalExpression(IEnumerable<ExpressionDesign> expressionList)
        {
            var expression = expressionList.Where(n => n != null && n.ParentExpressionId == null).FirstOrDefault();
            if (expression != null)
            {
                this.GenerateParentExpression(expressionList, ref expression);
            }
            return expression;
        }
        private void GenerateParentExpression(IEnumerable<ExpressionDesign> expressionList, ref ExpressionDesign parentExpression)
        {
            var parent = parentExpression;
            var childExpressions = from exp in expressionList where exp.ParentExpressionId == parent.ExpressionId select exp;
            if (childExpressions != null && childExpressions.Count() > 0)
            {
                parentExpression.Expressions = new List<ExpressionDesign>();
                foreach (var childExpression in childExpressions)
                {
                    ExpressionDesign child = childExpression;
                    GenerateParentExpression(expressionList, ref child);
                    parentExpression.Expressions.Add(childExpression);
                }
            }
        }
        #region Rules Methods

        private string GetRuleTypeText(int TargetPropertyTypeId)
        {
            string ruleTypeText = string.Empty;
            if (TargetPropertyTypeId == (int)TargetPropertyTypes.Enabled)
            {
                ruleTypeText = "'Enabled'";
            }
            else if (TargetPropertyTypeId == (int)TargetPropertyTypes.RunValidation)
            {
                ruleTypeText = "'Run Validation'";
            }
            else if (TargetPropertyTypeId == (int)TargetPropertyTypes.Value)
            {
                ruleTypeText = "'Value'";
            }
            else if (TargetPropertyTypeId == (int)TargetPropertyTypes.Visible)
            {
                ruleTypeText = "'Visible'";
            }
            else if (TargetPropertyTypeId == (int)TargetPropertyTypes.IsRequired)
            {
                ruleTypeText = "'Is Required'";
            }
            else if (TargetPropertyTypeId == (int)TargetPropertyTypes.Error)
            {
                ruleTypeText = "'Error'";
            }
            return ruleTypeText;
        }

        private bool IsParentRepeater(int elementID)
        {
            bool isParentRepeater = false;
            UIElement element = (from elem in this.formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            if (element != null)
            {
                if (element.UIElement2 is RepeaterUIElement)
                {
                    isParentRepeater = true;
                }
            }

            return isParentRepeater;
        }

        private string GetFullNameFromID(int elementID)
        {
            string fullName = "";
            UIElement element = (from elem in this.formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            fullName = GetFullName(element);
            return fullName;
        }

        private string GetFullNameFromName(string elementName)
        {
            string fullName = "";
            if (!String.IsNullOrEmpty(elementName))
            {
                UIElement element = (from elem in this.formElementList
                                     where elem.UIElementName == elementName
                                     select elem).FirstOrDefault();
                fullName = GetFullName(element);

            }
            return fullName;
        }

        private string GetFullName(UIElement element)
        {
            string fullName = "";
            if (element != null)
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
            return fullName;
        }

        private string GetOperandValue(string operandElementFullName, string operandElementName, JObject sourceData, string _containerName, JObject rowData)
        {
            string value = null;
            if (!string.IsNullOrEmpty(_containerName) && operandElementFullName.Contains(_containerName) && rowData != null)
            {
                string elementName = operandElementFullName.Replace(_containerName, "");
                string[] nameParts = elementName.Split('.');
                JToken dataPart = null;
                for (var idx = 0; idx < nameParts.Length; idx++)
                {
                    if (idx == 0)
                    {
                        if (rowData[nameParts[idx]] != null)
                        {
                            dataPart = rowData[nameParts[idx]];
                        }
                    }
                    else if (idx == (nameParts.Length - 1))
                    {
                        if (rowData[nameParts[idx]] != null)
                        {
                            dataPart = rowData[nameParts[idx]];
                        }
                    }
                    else
                    {
                        dataPart = dataPart[nameParts[idx]];
                    }
                }
                value = dataPart == null ? "" : (string)dataPart;
            }
            else
            {
                JToken dataPart = null;
                var nameParts = operandElementFullName.Split('.');
                for (var idx = 0; idx < nameParts.Length; idx++)
                {
                    if (idx == 0)
                    {
                        dataPart = sourceData[nameParts[idx]];
                    }
                    else
                    {
                        dataPart = dataPart[nameParts[idx]];
                    }
                }
                value = dataPart == null ? "" : (string)dataPart;
            }

            if (operandElementName != null)
            {
                if (operandElementName.IndexOf("CheckBox") > 0 || operandElementName.IndexOf("Radio") > 0)
                {
                    value = value == "true" || value == "True" || value == "Yes" || value == "yes" ? "Yes" : "No";
                }
            }

            return value;
        }

        //Evaluate single expression
        private bool EvaluateExpression(string leftOperand, int op, string rightOperand, bool isRightOperandElement, string uiElementId)
        {
            var result = false;

            if (op == (int)OperatorTypes.Equals)
            {
                //result = ExpressionHelper.Equal(leftOperand, rightOperand);
            }
            else if (op == (int)OperatorTypes.GreaterThan)
            {
                //result = ExpressionHelper.GreaterThan(leftOperand, rightOperand, uiElementId.IndexOf("Calendar") > 0);
            }
            else if (op == (int)OperatorTypes.GreaterThanOrEqualTo)
            {
                //result = ExpressionHelper.GreaterThanOrEqual(leftOperand, rightOperand, uiElementId.IndexOf("Calendar") > 0);
            }
            else if (op == (int)OperatorTypes.LessThan)
            {
               // result = ExpressionHelper.LessThan(leftOperand, rightOperand, uiElementId.IndexOf("Calendar") > 0);
            }
            else if (op == (int)OperatorTypes.LessThanOrEqualTo)
            {
               // result = ExpressionHelper.LessThanOrEqual(leftOperand, rightOperand, uiElementId.IndexOf("Calendar") > 0);
            }
            else if (op == (int)OperatorTypes.Contains)
            {
                if (uiElementId.IndexOf("DropDown") > -1)
                {
                    if (leftOperand == "Select One")
                    {
                        leftOperand = "";
                    }
                }
               // result = ExpressionHelper.Contains(leftOperand, rightOperand);
            }
            else if (op == (int)OperatorTypes.NotEquals)
            {
               // result = ExpressionHelper.NotEqual(leftOperand, rightOperand);
            }
            else if (op == (int)OperatorTypes.IsNull)
            {
               // result = ExpressionHelper.IsNull(leftOperand);
            }
            return result;
        }

        private object GetCurrentValue(string formInstanceData, string fullPath)
        {
            object getValue = null;

            try
            {
                var converter = new ExpandoObjectConverter();
                dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(formInstanceData, converter);

                string[] elements = fullPath.Split('.');
                string keyname = elements.Last();
                int count = elements.Length - 1;
                int i = -1;

                IDictionary<string, object> values = jsonObject as IDictionary<string, object>;

                foreach (var element in elements)
                {
                    i++;
                    if (values is ExpandoObject)
                    {
                        if (i == count)
                        {
                            if (values.ContainsKey(keyname))
                            {
                                getValue = values.FirstOrDefault(x => x.Key == keyname).Value;
                                return getValue;
                            }
                        }
                        else
                        {
                            values = values[element] as IDictionary<string, object>;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return getValue;
        }

        #endregion Rules Methods

        private int CheckIASElementImportDataDropdownDatasources(int GlobalUpdateID, int FormDesignID, DataRow insertRow, int IASElementExportID, string elementFullName, string newValue, string addedBy)
        {
            try
            {
                List<FormDesignElementValueVeiwModel> ElementSelectionInfo = GetFormDesignVersionElementSelection(GlobalUpdateID, FormDesignID);
                string AccountName = Convert.ToString(insertRow["Account or Portfolio"]);
                int FolderID = Convert.ToInt32(Convert.ToString(insertRow["FolderID"]));
                string FolderName = Convert.ToString(insertRow["Folder Name"]);
                DateTime EffectiveDate = Convert.ToDateTime(Convert.ToString(insertRow["Folder Effective Date"]));
                string FormName = Convert.ToString(insertRow["Document Name"]);
                string Owner = Convert.ToString(insertRow["Owner"]);
                int FormInstanceID = Convert.ToInt32(Convert.ToString(insertRow["FormInstanceID"]));
                int FolderVersionID = Convert.ToInt32(Convert.ToString(insertRow["FolderVersionID"]));
                int IASFolderMapID = GetIASFolderMapID(GlobalUpdateID, FormInstanceID);
                int TenantIDValue = 1;
                int FormDesignVersionIDValue = (from formInstance in this._unitOfWork.RepositoryAsync<FormInstance>().Query()
                                                  .Filter(fil => fil.FormInstanceID == FormInstanceID)
                                                  .Get()
                                                select formInstance.FormDesignVersionID).FirstOrDefault();

                FormDesignElementValueVeiwModel ElementSelection = ElementSelectionInfo.Where(cr => cr.ElementFullPath == elementFullName && cr.UIElementTypeID == ElementTypes.DROPDOWNID).FirstOrDefault();
                DataSourceDesign des = GetDataSources(FormDesignID, FormDesignVersionIDValue, ElementSelection);
                int FormDesignIDValue = Convert.ToInt32(des.FormDesignID);
                string SourceParentValue = Convert.ToString(des.SourceParent);
                string TargetParentValue = Convert.ToString(des.TargetParent);
                string DisplayModeValue = Convert.ToString(des.DisplayMode);
                if (des != null && ElementSelection != null && elementFullName.Contains(TargetParentValue) && DisplayModeValue == "Dropdown")
                {
                    string SourceElementValue = Convert.ToString(des.Mappings[0].SourceElement);
                    string TargetElementValue = Convert.ToString(des.Mappings[0].TargetElement);

                    StringBuilder sb = new StringBuilder();
                    if (ElementSelection.Name == TargetElementValue)
                    {
                        ExpandoObject source = GetDataSource(FormDesignIDValue, TenantIDValue, FormInstanceID, FormDesignVersionIDValue, FolderVersionID);

                        IList<Object> sourceParent = GetParentObject(source, SourceParentValue);
                        List<string> dropdownDesign = new List<string>();
                        if (sourceParent != null)
                        {
                            int sourceCount = sourceParent.Count();
                            for (int idx = 0; idx < sourceCount; idx++)
                            {
                                IDictionary<string, object> sourceDyn = sourceParent[idx] as IDictionary<string, object>;
                                if (sourceDyn.ContainsKey(SourceElementValue))
                                {
                                    string ItemValue = Convert.ToString(sourceDyn[SourceElementValue]) != null ? Convert.ToString(sourceDyn[SourceElementValue]).Trim() : null;
                                    if (!string.IsNullOrEmpty(ItemValue))
                                    {
                                        dropdownDesign.Add(ItemValue);
                                    }
                                }
                            }

                            if (dropdownDesign.Count > 0 && !dropdownDesign.Contains(newValue))
                            {
                                sb.Append(String.Format(ValidationMessageData.invalidDropdownDataSourceMsg, newValue, Convert.ToString(ElementSelection.ElementFullPath.Replace(".", " => "))));
                            }

                            if (sb.Length > 0)
                            {
                                if (IASElementExportID != 0)
                                {
                                    //Update DataSources error data to GU.ErrorLog table
                                    UpdateErrorLogData(GlobalUpdateID, IASElementExportID, sb, addedBy, DataSourceErrorFlag);
                                    return IASElementExportID;
                                }
                                else
                                {
                                    IASElementExportID = GetIASElementExportID(IASFolderMapID, ElementSelection.UIElementID);
                                    //Save DataSources error data to GU.ErrorLog table
                                    SaveErrorLogData(GlobalUpdateID, IASElementExportID, sb, addedBy, DataSourceErrorFlag);
                                    return IASElementExportID;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return IASElementExportID;
        }

        private int CheckIASElementImportDataDatasources(int GlobalUpdateID, int FormDesignID, DataRow insertRow, int IASElementExportID, string elementFullName, string newValue, string addedBy)
        {
            try
            {
                string AccountName = Convert.ToString(insertRow["Account or Portfolio"]);
                int FolderID = Convert.ToInt32(Convert.ToString(insertRow["FolderID"]));
                string FolderName = Convert.ToString(insertRow["Folder Name"]);
                DateTime EffectiveDate = Convert.ToDateTime(Convert.ToString(insertRow["Folder Effective Date"]));
                string FormName = Convert.ToString(insertRow["Document Name"]);
                string Owner = Convert.ToString(insertRow["Owner"]);
                int FormInstanceID = Convert.ToInt32(Convert.ToString(insertRow["FormInstanceID"]));
                int FolderVersionID = Convert.ToInt32(Convert.ToString(insertRow["FolderVersionID"]));
                int IASFolderMapID = GetIASFolderMapID(GlobalUpdateID, FormInstanceID);

                List<FormDesignElementValueVeiwModel> ElementSelectionInfo = GetFormDesignVersionElementSelection(GlobalUpdateID, FormDesignID);
                FormDesignElementValueVeiwModel ElementSelection = ElementSelectionInfo.Where(cr => cr.ElementFullPath == elementFullName && cr.UIElementTypeID == ElementTypes.DROPDOWNID).FirstOrDefault();
                if (ElementSelection != null)
                {
                    List<Item> dropDownItemList = GetDropDownItems(ElementSelection.UIElementID);
                    List<string> dropdownDesign = new List<string>();
                    foreach (var dropDownItem in dropDownItemList)
                    {
                        string ItemValue = Convert.ToString(dropDownItem.ItemValue) != null ? Convert.ToString(dropDownItem.ItemValue).Trim() : null;
                        if (!string.IsNullOrEmpty(ItemValue))
                        {
                            dropdownDesign.Add(ItemValue);
                        }
                    }

                    StringBuilder sb = new StringBuilder();
                    if (dropdownDesign.Count > 0 && !dropdownDesign.Contains(newValue))
                    {
                        sb.Append(String.Format(ValidationMessageData.invalidDropdownItemsMsg, newValue));
                    }
                    else
                    {
                        CheckIASElementImportDataDropdownDatasources(GlobalUpdateID, FormDesignID, insertRow, IASElementExportID, elementFullName, newValue, addedBy);
                    }

                    if (sb.Length > 0)
                    {
                        if (IASElementExportID != 0)
                        {
                            //Update DataSources error data to GU.ErrorLog table
                            UpdateErrorLogData(GlobalUpdateID, IASElementExportID, sb, addedBy, DataSourceErrorFlag);
                            return IASElementExportID;
                        }
                        else
                        {
                            IASElementExportID = GetIASElementExportID(IASFolderMapID, ElementSelection.UIElementID);
                            //Save DataSources error data to GU.ErrorLog table
                            SaveErrorLogData(GlobalUpdateID, IASElementExportID, sb, addedBy, DataSourceErrorFlag);
                            return IASElementExportID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return IASElementExportID;
        }

        #endregion Public Methods

        #region Private Methods

        private class ValidationMessageData
        {
            public static string requiredMsg = "Current New Value is required.";
            public static string regexMsg = "Current New Value - {0} does not have correct format - {1}";
            public static string invalidMaxLengthMsg = "Current New Value - {0} has exceeded its max length = {1}.";
            public static string invalidIntMsg = "Current New Value - {0} is not a valid number.";
            public static string invalidFloatMsg = "Current New Value - {0} is not a valid decimal number.";
            public static string invalidDateMsg = "Current New Value - {0} does not have a valid date format.";
            public static string invalidDateRangeMsg = "Current New Value - {0} is not within a valid date range from {1} to {2}.";

            public static string invalidDropdownDataSourceMsg = "Current New Value - {0} is not present in mapped Datasource - {1}.";
            public static string invalidDropdownItemsMsg = "Current New Value - {0} is not present in mapped Items.";

            public static string invalidRuleSourceValueMsg = "{1} Rule failed on {2} because Current New Value - {0} {5} {3} {4}.";
            public static string invalidRuleTargetValueMsg = "Current New Value - {0} is going to be changed by {1} Rule with either {2} or {3} value.";

            public static string requiredTargetValueMsg = "Current New Value - {0} is required because of {1} Rule.";

        }

        private List<FormDesignElementValueVeiwModel> GetFormDesignVersionElementSelection(int GlobalUpdateID, int FormDesignID)
        {
            List<FormDesignElementValueVeiwModel> globalUpdatesUIElementsList = null;

            try
            {
                var globalUpdatesUIElements = (from j in this._unitOfWork.Repository<FormDesignElementValue>()
                                                            .Query()
                                                            .Include(f => f.UIElement)
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID && f.FormDesignID == FormDesignID
                                                                && ((f.UIElement is TextBoxUIElement) || (f.UIElement is RadioButtonUIElement)
                                                                || (f.UIElement is DropDownUIElement) || (f.UIElement is CheckBoxUIElement)
                                                                || (f.UIElement is CalendarUIElement)))
                                                            .Get()
                                               join tb in this._unitOfWork.Repository<TextBoxUIElement>().Get()
                                                on j.UIElement.UIElementID equals tb.UIElementID
                                                into textboxElement
                                               from textboxlist in textboxElement.DefaultIfEmpty()
                                               join rd in this._unitOfWork.Repository<RadioButtonUIElement>().Get()
                                                on j.UIElement.UIElementID equals rd.UIElementID
                                                into radioElement
                                               from radiolist in radioElement.DefaultIfEmpty()
                                               join dd in this._unitOfWork.Repository<DropDownUIElement>().Get()
                                                on j.UIElement.UIElementID equals dd.UIElementID
                                                into dropdownElement
                                               from dropdownlist in dropdownElement.DefaultIfEmpty()
                                               join cb in this._unitOfWork.Repository<CheckBoxUIElement>().Get()
                                                on j.UIElement.UIElementID equals cb.UIElementID
                                                into checkboxElement
                                               from checkboxlist in checkboxElement.DefaultIfEmpty()
                                               join cal in this._unitOfWork.Repository<CalendarUIElement>().Get()
                                                on j.UIElement.UIElementID equals cal.UIElementID
                                                into calendarElement
                                               from calendarlist in calendarElement.DefaultIfEmpty()
                                               select new FormDesignElementValueVeiwModel
                                               {
                                                   FormDesignElementValueID = j.FormDesignElementValueID,
                                                   GlobalUpdateID = j.GlobalUpdateID,
                                                   FormDesignID = j.FormDesignID,
                                                   FormDesignVersionID = j.FormDesignVersionID,
                                                   UIElementID = j.UIElementID,
                                                   ElementFullPath = j.ElementFullPath,
                                                   IsUpdated = j.IsUpdated,
                                                   NewValue = j.NewValue,
                                                   ElementHeaderName = j.ElementHeaderName,
                                                   FormDesignName = j.FormDesign.DisplayText,
                                                   Name = j.UIElement.GeneratedName,
                                                   UIElementTypeID = (textboxlist.UIElementTypeID != null) ? textboxlist.UIElementTypeID : (radiolist.UIElementTypeID != null) ? radiolist.UIElementTypeID : (dropdownlist.UIElementTypeID != null) ? dropdownlist.UIElementTypeID : (checkboxlist.UIElementTypeID != null) ? checkboxlist.UIElementTypeID : (calendarlist.UIElementTypeID != null) ? calendarlist.UIElementTypeID : ElementTypes.BLANKID,
                                                   UIElementName = j.UIElement.UIElementName,
                                                   Label = j.ElementHeaderName,
                                                   OptionLabel = (radiolist.OptionLabel != null) ? radiolist.OptionLabel.ToString() : null,
                                                   OptionLabelNo = (radiolist.OptionLabelNo != null) ? radiolist.OptionLabelNo.ToString() : null
                                               });
                if (globalUpdatesUIElements != null)
                {
                    globalUpdatesUIElementsList = globalUpdatesUIElements.GroupBy(xy => xy.UIElementID).Select(xy => xy.FirstOrDefault()).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return globalUpdatesUIElementsList;
        }

        private string GetFormDesignVersionData(int FormInstanceID)
        {
            string formDesignVersionjson = string.Empty;
            try
            {
                var formDesignVersion = (from j in this._unitOfWork.Repository<FormInstance>()
                                                            .Query()
                                                            .Include(f => f.FormDesignVersion)
                                                            .Filter(f => f.FormInstanceID == FormInstanceID)
                                                            .Get()

                                           select new
                                           {
                                               FormDesignVersionData = j.FormDesignVersion.FormDesignVersionData
                                           }).FirstOrDefault();
                if (formDesignVersion != null)
                {
                    formDesignVersionjson = formDesignVersion.FormDesignVersionData;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return formDesignVersionjson;
        }

        private int GetIASFolderMapID(int GlobalUpdateID, int formInstanceId)
        {
            int iasFolderMapID = 0;
            try
            {
                var globalUpdatesFolderData = (from j in this._unitOfWork.Repository<IASFolderMap>()
                                                            .Query()
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID && f.FormInstanceID == formInstanceId)
                                                            .Get()

                                               select new IASFolderDataModel
                                               {
                                                   IASFolderMapID = j.IASFolderMapID
                                               }).FirstOrDefault();
                if (globalUpdatesFolderData != null)
                {
                    iasFolderMapID = Convert.ToInt32(globalUpdatesFolderData.IASFolderMapID);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return iasFolderMapID;
        }

        private int GetIASElementExportID(int IASFolderMapID, int UIElementID)
        {
            int iasFolderMapID = 0;
            try
            {
                var globalUpdatesFolderData = (from j in this._unitOfWork.Repository<IASElementExport>()
                                                            .Query()
                                                            .Filter(f => f.IASFolderMapID == IASFolderMapID && f.UIElementID == UIElementID)
                                                            .Get()

                                               select new IASElementExportDataModel
                                               {
                                                   IASElementExportID = j.IASElementExportID
                                               }).FirstOrDefault();
                if (globalUpdatesFolderData != null)
                {
                    iasFolderMapID = Convert.ToInt32(globalUpdatesFolderData.IASElementExportID);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return iasFolderMapID;
        }

        private void SaveErrorLogData(int GlobalUpdateID, int IASElementExportID, StringBuilder sb, string addedBy, int flag)
        {
            ErrorLog itemToAdd = new ErrorLog();

            itemToAdd.GlobalUpdateID = GlobalUpdateID;
            itemToAdd.IASElementExportID = IASElementExportID;
            if (flag == ValidationErrorFlag)
            {
                itemToAdd.ValidationErrorDescription = Convert.ToString(sb);
                itemToAdd.RuleErrorDescription = null;
                itemToAdd.DataSourceErrorDescription = null;
            }
            else if (flag == RuleErrorFlag)
            {
                itemToAdd.ValidationErrorDescription = null;
                itemToAdd.RuleErrorDescription = Convert.ToString(sb);
                itemToAdd.DataSourceErrorDescription = null;
            }
            else if (flag == DataSourceErrorFlag)
            {
                itemToAdd.ValidationErrorDescription = null;
                itemToAdd.RuleErrorDescription = null;
                itemToAdd.DataSourceErrorDescription = Convert.ToString(sb);
            }
            itemToAdd.AddedBy = addedBy;
            itemToAdd.AddedDate = DateTime.Now;
            itemToAdd.UpdatedBy = null;
            itemToAdd.UpdatedDate = null;

            this._unitOfWork.Repository<ErrorLog>().Insert(itemToAdd);
            this._unitOfWork.Save();
        }

        private void UpdateErrorLogData(int GlobalUpdateID, int IASElementExportID, StringBuilder sb, string addedBy, int flag)
        {
            var itemToUpdate = this._unitOfWork.RepositoryAsync<ErrorLog>().Query()
                                                        .Filter(e => e.GlobalUpdateID == GlobalUpdateID && e.IASElementExportID == IASElementExportID)
                                                        .Get().FirstOrDefault();

            if (itemToUpdate != null)
            {
                StringBuilder updatedsb = new StringBuilder();
                if (flag == ValidationErrorFlag)
                {
                    if (itemToUpdate.ValidationErrorDescription != null)
                    {
                        updatedsb.Append(Convert.ToString(itemToUpdate.ValidationErrorDescription));
                    }
                    updatedsb.Append(Convert.ToString(sb));
                    itemToUpdate.ValidationErrorDescription = Convert.ToString(updatedsb);
                }
                else if (flag == RuleErrorFlag)
                {
                    if (itemToUpdate.RuleErrorDescription != null)
                    {
                        updatedsb.Append(Convert.ToString(itemToUpdate.RuleErrorDescription));
                    }
                    updatedsb.Append(Convert.ToString(sb));
                    itemToUpdate.RuleErrorDescription = Convert.ToString(updatedsb);
                }
                else if (flag == DataSourceErrorFlag)
                {
                    if (itemToUpdate.DataSourceErrorDescription != null)
                    {
                        updatedsb.Append(Convert.ToString(itemToUpdate.DataSourceErrorDescription));
                    }
                    updatedsb.Append(Convert.ToString(sb));
                    itemToUpdate.DataSourceErrorDescription = Convert.ToString(updatedsb);
                }
                itemToUpdate.UpdatedDate = DateTime.Now;
                itemToUpdate.UpdatedBy = addedBy;
                this._unitOfWork.RepositoryAsync<ErrorLog>().Update(itemToUpdate);
                this._unitOfWork.Save();
            }
        }

        private List<ErrorLogViewModel> GetGlobalUpdatesErrorLogList(int GlobalUpdateID)
        {
            List<ErrorLogViewModel> globalUpdatesErrorLogList = null;

            try
            {
                var globalUpdatesErrorLog = (from j in this._unitOfWork.Repository<ErrorLog>()
                                                            .Query()
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID)
                                                            .Get()
                                             join fdEV in this._unitOfWork.Repository<FormDesignElementValue>().Get()
                                             on j.IASElementExport.UIElement.UIElementID equals fdEV.UIElementID
                                             select new ErrorLogViewModel
                                             {
                                                 GlobalUpdateID = j.GlobalUpdateID,
                                                 IASElementExportID = j.IASElementExportID,
                                                 IASFolderMapID = j.IASElementExport.IASFolderMapID,
                                                 AccountName = j.IASElementExport.IASFolderMap.AccountName,
                                                 FolderID = j.IASElementExport.IASFolderMap.FolderID,
                                                 FolderName = j.IASElementExport.IASFolderMap.FolderName,
                                                 FolderVersionID = j.IASElementExport.IASFolderMap.FolderVersionID,
                                                 FolderVersionNumber = j.IASElementExport.IASFolderMap.FolderVersionNumber,
                                                 EffectiveDate = j.IASElementExport.IASFolderMap.EffectiveDate,
                                                 FormInstanceID = j.IASElementExport.IASFolderMap.FormInstanceID,
                                                 FormName = j.IASElementExport.IASFolderMap.FormName,
                                                 Owner = j.IASElementExport.IASFolderMap.Owner,
                                                 FormDesignID = j.IASElementExport.IASFolderMap.FormInstance.FormDesignID,
                                                 Label = j.IASElementExport.ElementHeaderName,
                                                 ElementFullPath = fdEV.ElementFullPath.Replace(".", " => "),
                                                 RuleErrorDescription = j.RuleErrorDescription,
                                                 DataSourceErrorDescription = j.DataSourceErrorDescription,
                                                 ValidationErrorDescription = j.ValidationErrorDescription,
                                                 AddedBy = j.AddedBy,
                                                 AddedDate = j.AddedDate
                                             }).OrderBy(el => el.IASElementExportID);
                if (globalUpdatesErrorLog != null)
                {
                    globalUpdatesErrorLogList = globalUpdatesErrorLog.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return globalUpdatesErrorLogList;
        }

        private ServiceResult IASElementImportData(int GlobalUpdateID, int FormDesignID, DataTable dt, string addedBy)
        {
            var result = new ServiceResult();

            foreach (DataRow insertRow in dt.Rows)
            {
                string AccountName = Convert.ToString(insertRow["Account or Portfolio"]);
                int FolderID = Convert.ToInt32(Convert.ToString(insertRow["FolderID"]));
                string FolderName = Convert.ToString(insertRow["Folder Name"]);
                DateTime EffectiveDate = Convert.ToDateTime(Convert.ToString(insertRow["Folder Effective Date"]));
                string FormName = Convert.ToString(insertRow["Document Name"]);
                string Owner = Convert.ToString(insertRow["Owner"]);
                int FormInstanceID = Convert.ToInt32(Convert.ToString(insertRow["FormInstanceID"]));
                int totalColumn = dt.Columns.Count;
                int IASFolderMapID = GetIASFolderMapID(GlobalUpdateID, FormInstanceID);

                var filter = GetIASElementExportDataList(GlobalUpdateID, IASFolderMapID);
                SaveImportMatch(GlobalUpdateID, FormDesignID, FormInstanceID, IASFolderMapID, insertRow, totalColumn, addedBy, filter);
                
            }
            result.Result = ServiceResultStatus.Success;
            return result;
        }

        private List<IASElementExportDataModel> GetIASElementExportDataList(int GlobalUpdateID, int IASFolderMapID)
        {
            List<IASElementExportDataModel> iasElementExportDataList = null;

            try
            {
                var iasElementExportData = (from j in this._unitOfWork.Repository<IASElementExport>()
                                                            .Query()
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID && f.IASFolderMapID == IASFolderMapID)
                                                            .Get()
                                         
                                         select new IASElementExportDataModel
                                         {
                                             GlobalUpdateID = j.GlobalUpdateID,
                                             IASFolderMapID = j.IASFolderMapID,
                                             UIElementID = j.UIElementID,
                                             UIElementTypeID = j.UIElementTypeID,
                                             Label = j.ElementHeaderName,
                                             UIElementName = j.UIElementName,
                                             NewValue = j.NewValue,
                                             OldValue = j.OldValue,
                                             AcceptChange = j.AcceptChange
                                         });
                if (iasElementExportData != null)
                {
                    iasElementExportDataList = iasElementExportData.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return iasElementExportDataList;
        }

        private void SaveImportMatch(int GlobalUpdateID, int FormDesignID, int FormInstanceID, int IASFolderMapID, DataRow insertRow, int totalColumn, string addedBy, List<IASElementExportDataModel> IASElementExportDataList)
        {
            List<FormDesignElementValueVeiwModel> ElementSelectionInfo = GetFormDesignVersionElementSelection(GlobalUpdateID, FormDesignID);
            foreach (var IASElementExportData in IASElementExportDataList)
            {
                SaveIASElementImportData(IASElementExportData.GlobalUpdateID, IASElementExportData.IASFolderMapID, IASElementExportData.Label, IASElementExportData.OldValue, IASElementExportData.NewValue, IASElementExportData.AcceptChange, IASElementExportData.UIElementID, IASElementExportData.UIElementTypeID, IASElementExportData.UIElementName, addedBy);
            }
            int checkCount = 0;
            for (int c = 10; c < totalColumn; c++)
            {
                checkCount++;
                if (checkCount % 6 == 0)
                {
                    int newValueColumn = c - 2;
                    int acceptChangeColumn = c - 1;
                    string elementFullName = Convert.ToString(insertRow[c]);
                    string newValue = Convert.ToString(insertRow[newValueColumn]);
                    string acceptChange = Convert.ToString(insertRow[acceptChangeColumn]);
                    FormDesignElementValueVeiwModel ElementSelection = ElementSelectionInfo.Where(cr => cr.ElementFullPath == elementFullName).FirstOrDefault();
                    if (ElementSelection != null)
                    {
                        if (acceptChange == GlobalUpdateConstants.YES)
                        {
                            if (GlobalUpdateConstants.equalsList.Contains(newValue))
                            {
                                UpdateIASElementImportData(GlobalUpdateID, IASFolderMapID, ElementSelection.UIElementID, GlobalUpdateConstants.NA, true, addedBy);
                            }
                            else
                            {
                                UpdateIASElementImportData(GlobalUpdateID, IASFolderMapID, ElementSelection.UIElementID, newValue, true, addedBy);
                            }
                        }
                        else if (acceptChange == GlobalUpdateConstants.NO)
                        {
                            if (GlobalUpdateConstants.equalsList.Contains(newValue))
                            {
                                UpdateIASElementImportData(GlobalUpdateID, IASFolderMapID, ElementSelection.UIElementID, GlobalUpdateConstants.NA, false, addedBy);
                            }
                            else
                            {
                                UpdateIASElementImportData(GlobalUpdateID, IASFolderMapID, ElementSelection.UIElementID, newValue, false, addedBy);
                            }
                        }
                    }
                }
            }
        }

        private void SaveIASElementImportData(int GlobalUpdateID, int IASFolderMapID, string Label, string OldValue, string NewValue, bool AcceptChange, int UIElementID, int UIElementTypeID, string UIElementName, string addedBy)
        {
            IASElementImport itemToAdd = new IASElementImport();

            itemToAdd.GlobalUpdateID = GlobalUpdateID;
            itemToAdd.IASFolderMapID = IASFolderMapID;
            itemToAdd.ElementHeaderName = Label;
            itemToAdd.OldValue = OldValue;
            itemToAdd.NewValue = NewValue;
            itemToAdd.AcceptChange = AcceptChange;
            itemToAdd.UIElementID = UIElementID;
            itemToAdd.UIElementTypeID = UIElementTypeID;
            itemToAdd.UIElementName = UIElementName;
            itemToAdd.AddedBy = addedBy;
            itemToAdd.AddedDate = DateTime.Now;
            itemToAdd.UpdatedBy = null;
            itemToAdd.UpdatedDate = null;

            this._unitOfWork.Repository<IASElementImport>().Insert(itemToAdd);
            this._unitOfWork.Save();
        }

        private void UpdateIASElementImportData(int GlobalUpdateID, int IASFolderMapID, int UIElementID, string newValue, bool acceptChange, string addedBy)
        {
            var itemToUpdate = this._unitOfWork.RepositoryAsync<IASElementImport>().Query()
                                    .Filter(e => e.GlobalUpdateID == GlobalUpdateID
                                            && e.IASFolderMapID == IASFolderMapID
                                            && e.UIElementID == UIElementID)
                                    .Get().FirstOrDefault();

            if (itemToUpdate != null)
            {
                itemToUpdate.NewValue = newValue;
                itemToUpdate.AcceptChange = acceptChange;
                itemToUpdate.UpdatedDate = DateTime.Now;
                itemToUpdate.UpdatedBy = addedBy;
                this._unitOfWork.RepositoryAsync<IASElementImport>().Update(itemToUpdate);
                this._unitOfWork.Save();
            }
        }

        private void DeleteErrorLog(int GlobalUpdateID)
        {
            var errorLogToDelete = this._unitOfWork.RepositoryAsync<ErrorLog>()
                                            .Query()
                                            .Filter(c => c.GlobalUpdateID == GlobalUpdateID)
                                            .Get().ToList();

            if (errorLogToDelete.Any())
            {
                foreach (var item in errorLogToDelete)
                {
                    this._unitOfWork.RepositoryAsync<ErrorLog>().Delete(item);
                }
                this._unitOfWork.Save();
            }            
        }

        public void UpdateGlobalUpdateIASStatus(int GlobalUpdateID, int GlobalUpdateStatusID)
        {
            GlobalUpdate globalUpdate = (from element in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Get()
                                         where element.GlobalUpdateID == GlobalUpdateID
                                         select element).FirstOrDefault();
            globalUpdate.GlobalUpdateStatusID = GlobalUpdateStatusID;
            this._unitOfWork.RepositoryAsync<GlobalUpdate>().Update(globalUpdate, true);
            this._unitOfWork.Save();
        }

        private List<UIElement> GetUIElemenstFromFormDesignVersion(int formDesignVersionId)
        {

            List<UIElement> formElementList = (from u in this._unitOfWork.RepositoryAsync<UIElement>()
                                                                .Query()
                                                                .Get()
                                               join fd in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                       .Query()
                                                                       .Get()
                                               on u.UIElementID equals fd.UIElementID
                                               where fd.FormDesignVersionID == formDesignVersionId
                                               select u).ToList();

            return formElementList;
        }

        //Process list of expressions
        private string ProcessNode(ExpressionRowModel expression)
        {
            //loop through all the expression
            //Call ProcessLeaf to evaluate single expression
            string isSuccess = string.Empty;
            string logicalOperator = "";// expression.LogicalOperatorTypeId == (int)tmg.equinox.documentmatch.RulesEngine.RuleProcessor.LogicalOperatorTypes.AND ? "AND" : "OR";
            if (expression.Expressions != null && expression.Expressions.Count > 0)
            {
                for (var idx = 0; idx < expression.Expressions.Count; idx++)
                {
                    var exp = expression.Expressions[idx];
                    string result = exp.ExpressionTypeId.ToString() ;//== (int)tmg.equinox.documentmatch.RulesEngine.RuleProcessor.ExpressionTypes.NODE ? this.ProcessNode(exp) : this.ProcessLeaf(exp);

                    if (result != "")
                    {
                        if (isSuccess != "")
                        {
                            isSuccess = isSuccess + " " + logicalOperator;
                        }
                        isSuccess = isSuccess + " [" + result + "]";
                    }
                }
            }

            return isSuccess;
        }

        //Process an expressions
        private string ProcessLeaf(ExpressionRowModel expression)
        {
            string leftOperand = GetLabelFromName(expression.LeftOperand);
            string rightOperand = Convert.ToString(expression.RightOperand);

            if (expression.IsRightOperandElement) { rightOperand = GetLabelFromName(expression.RightOperand); }
            string operatorTypeName = GetOperatorTypeName(expression.OperatorTypeId);

            if (expression.OperatorTypeId == (int)OperatorTypes.IsNull)
            {
                return leftOperand + operatorTypeName;
            }
            else
            {
                return leftOperand + operatorTypeName + rightOperand;
            }
        }

        private string GetOperatorTypeName(int OperatorTypeId)
        {
            string operatorTypeName = string.Empty;
            if (OperatorTypeId == (int)OperatorTypes.Equals)
            {
                operatorTypeName = " = ";
            }
            else if (OperatorTypeId == (int)OperatorTypes.GreaterThan)
            {
                operatorTypeName = " > ";
            }
            else if (OperatorTypeId == (int)OperatorTypes.LessThan)
            {
                operatorTypeName = " < ";
            }
            else if (OperatorTypeId == (int)OperatorTypes.Contains)
            {
                operatorTypeName = " Contains ";
            }
            else if (OperatorTypeId == (int)OperatorTypes.NotEquals)
            {
                operatorTypeName = " != ";
            }
            else if (OperatorTypeId == (int)OperatorTypes.GreaterThanOrEqualTo)
            {
                operatorTypeName = " >= ";
            }
            else if (OperatorTypeId == (int)OperatorTypes.LessThanOrEqualTo)
            {
                operatorTypeName = " <= ";
            }
            else if (OperatorTypeId == (int)OperatorTypes.IsNull)
            {
                operatorTypeName = " IsNull ";
            }
            return operatorTypeName;
        }

        private string GetLabelFromName(string elementName)
        {
            string label = "";
            if (!String.IsNullOrEmpty(elementName))
            {
                UIElement element = (from elem in formElementList
                                     where elem.UIElementName == elementName
                                     select elem).FirstOrDefault();
                label = element.Label;
            }
            return label;
        }

        private List<IASFolderDataModel> GetGlobalUpdatesFolderDataList(int GlobalUpdateID)
        {
            List<IASFolderDataModel> globalUpdatesFolderDataList = null;

            try
            {
                var globalUpdatesFolderData = (from j in this._unitOfWork.Repository<IASFolderMap>()
                                                            .Query()
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID)
                                                            .Get()

                                               select new IASFolderDataModel
                                               {
                                                   FormDesignID = j.FormInstance.FormDesignID,
                                                   IASFolderMapID = j.IASFolderMapID,
                                                   GlobalUpdateID = j.GlobalUpdateID,
                                                   AccountName = j.AccountName,
                                                   FolderID = j.FolderID,
                                                   FolderName = j.FolderName,
                                                   FolderVersionID = j.FolderVersionID,
                                                   FolderVersionNumber = j.FolderVersionNumber,
                                                   EffectiveDate = j.EffectiveDate,
                                                   FormInstanceID = j.FormInstanceID,
                                                   FormName = j.FormName,
                                                   Owner = j.Owner
                                               }).OrderBy(el => el.FolderID);
                if (globalUpdatesFolderData != null)
                {
                    globalUpdatesFolderDataList = globalUpdatesFolderData.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return globalUpdatesFolderDataList;
        }

        private List<FormDesignElementValueVeiwModel> GetFormDesignVersionUIElements(int GlobalUpdateID)
        {
            List<FormDesignElementValueVeiwModel> globalUpdatesUIElementsList = null;

            try
            {
                var globalUpdatesUIElements = (from j in this._unitOfWork.Repository<FormDesignElementValue>()
                                                            .Query()
                                                            .Include(f => f.UIElement)
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID
                                                                && ((f.UIElement is TextBoxUIElement) || (f.UIElement is RadioButtonUIElement)
                                                                || (f.UIElement is DropDownUIElement) || (f.UIElement is CheckBoxUIElement)
                                                                || (f.UIElement is CalendarUIElement)))
                                                            .Get()
                                               join tb in this._unitOfWork.Repository<TextBoxUIElement>().Get()
                                                on j.UIElement.UIElementID equals tb.UIElementID
                                                into textboxElement
                                               from textboxlist in textboxElement.DefaultIfEmpty()
                                               join rd in this._unitOfWork.Repository<RadioButtonUIElement>().Get()
                                                on j.UIElement.UIElementID equals rd.UIElementID
                                                into radioElement
                                               from radiolist in radioElement.DefaultIfEmpty()
                                               join dd in this._unitOfWork.Repository<DropDownUIElement>().Get()
                                                on j.UIElement.UIElementID equals dd.UIElementID
                                                into dropdownElement
                                               from dropdownlist in dropdownElement.DefaultIfEmpty()
                                               join cb in this._unitOfWork.Repository<CheckBoxUIElement>().Get()
                                                on j.UIElement.UIElementID equals cb.UIElementID
                                                into checkboxElement
                                               from checkboxlist in checkboxElement.DefaultIfEmpty()
                                               join cal in this._unitOfWork.Repository<CalendarUIElement>().Get()
                                                on j.UIElement.UIElementID equals cal.UIElementID
                                                into calendarElement
                                               from calendarlist in calendarElement.DefaultIfEmpty()
                                               select new FormDesignElementValueVeiwModel
                                               {
                                                   FormDesignElementValueID = j.FormDesignElementValueID,
                                                   GlobalUpdateID = j.GlobalUpdateID,
                                                   FormDesignID = j.FormDesignID,
                                                   FormDesignVersionID = j.FormDesignVersionID,
                                                   UIElementID = j.UIElementID,
                                                   ElementFullPath = j.ElementFullPath,
                                                   IsUpdated = j.IsUpdated,
                                                   NewValue = j.NewValue,
                                                   ElementHeaderName = j.ElementHeaderName,
                                                   FormDesignName = j.FormDesign.DisplayText,
                                                   Name = j.UIElement.GeneratedName,
                                                   UIElementTypeID = (textboxlist.UIElementTypeID != null) ? textboxlist.UIElementTypeID : (radiolist.UIElementTypeID != null) ? radiolist.UIElementTypeID : (dropdownlist.UIElementTypeID != null) ? dropdownlist.UIElementTypeID : (checkboxlist.UIElementTypeID != null) ? checkboxlist.UIElementTypeID : (calendarlist.UIElementTypeID != null) ? calendarlist.UIElementTypeID : ElementTypes.BLANKID,
                                                   UIElementName = j.UIElement.UIElementName,
                                                   Label = j.ElementHeaderName,
                                                   OptionLabel = (radiolist.OptionLabel != null) ? radiolist.OptionLabel.ToString() : null,
                                                   OptionLabelNo = (radiolist.OptionLabelNo != null) ? radiolist.OptionLabelNo.ToString() : null,
                                                   ItemData = null
                                               });
                if (globalUpdatesUIElements != null)
                {
                    globalUpdatesUIElementsList = globalUpdatesUIElements.GroupBy(xy => xy.UIElementID).Select(xy => xy.FirstOrDefault()).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return globalUpdatesUIElementsList;
        }

        private IEnumerable<GuRuleRowModel> GetRulesForUIElement(int tenantId, int formDesignVersionId, int uiElementId, int globalUpdateId)
        {
            IList<GuRuleRowModel> rowModelList = null;
            //call new function
            rowModelList = GetRulesForUIElementHierarchical(tenantId, formDesignVersionId, uiElementId, globalUpdateId);
            if (rowModelList == null)
            {
                try
                {
                    //Get all the rules along with expression for a uielement
                    rowModelList = (from r in this._unitOfWork.RepositoryAsync<RuleGu>()
                                                      .Query()
                                                      .Include(c => c.ExpressionsGu)
                                                      .Include(c => c.TargetProperty)
                                                      .Get()

                                    where r.UIElementID == uiElementId && r.GlobalUpdateID == globalUpdateId

                                    select new GuRuleRowModel
                                    {
                                        Expressions = (from exp in r.ExpressionsGu
                                                       select new ExpressionRowModel
                                                       {
                                                           ExpressionId = exp.ExpressionID,
                                                           LeftOperand = exp.LeftOperand,
                                                           LogicalOperatorTypeId = exp.LogicalOperatorTypeID,
                                                           OperatorTypeId = exp.OperatorTypeID,
                                                           RightOperand = exp.RightOperand,
                                                           RuleId = exp.RuleID,
                                                           ExpressionTypeId = exp.ExpressionTypeID,
                                                           IsRightOperandElement = exp.IsRightOperandElement,
                                                           TenantId = tenantId
                                                       }).AsEnumerable(),
                                        ResultFailure = r.ResultFailure,
                                        ResultSuccess = r.ResultSuccess,
                                        IsResultFailureElement = r.IsResultFailureElement,
                                        IsResultSuccessElement = r.IsResultSuccessElement,
                                        Message = r.Message,
                                        RuleId = r.RuleID,
                                        TargetPropertyId = r.TargetPropertyID,
                                        TargetProperty = r.TargetProperty.TargetPropertyName,
                                        UIElementID = r.UIElementID
                                    }).ToList();
                    if (rowModelList.Count() == 0)
                        rowModelList = null;
                }
                catch (Exception ex)
                {
                    bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                    if (reThrow) throw ex;
                }
            }
            return rowModelList;
        }

        private IList<GuRuleRowModel> GetRulesForUIElementHierarchical(int tenantId, int formDesignVersionId, int uiElementId, int globalUpdateId)
        {

            //  int globalUpdateId = (from s in this._unitOfWork.RepositoryAsync<FormDesignElementValue>().Query().Filter(x => x.FormDesignVersionID == formDesignVersionId && x.UIElementID == uiElementId).Get()
            //                       select s.GlobalUpdateID).FirstOrDefault();

            IList<GuRuleRowModel> rowModelList = null;
            try
            {
                //Get all the rules along with expression for a uielement
                rowModelList = (from r in this._unitOfWork.RepositoryAsync<RuleGu>()
                                                           .Query()
                                                           .Include(c => c.ExpressionsGu)
                                                           .Include(c => c.TargetProperty)
                                                           .Get()
                                where r.UIElementID == uiElementId && r.GlobalUpdateID == globalUpdateId

                                select new GuRuleRowModel
                                {
                                    Expressions = (from exp in r.ExpressionsGu
                                                   select new ExpressionRowModel
                                                   {
                                                       ExpressionId = exp.ExpressionID,
                                                       LeftOperand = exp.LeftOperand,
                                                       LogicalOperatorTypeId = exp.LogicalOperatorTypeID,
                                                       OperatorTypeId = exp.OperatorTypeID,
                                                       RightOperand = exp.RightOperand,
                                                       RuleId = exp.RuleID,
                                                       TenantId = tenantId,
                                                       ParentExpressionId = exp.ParentExpressionID,
                                                       ExpressionTypeId = exp.ExpressionTypeID,
                                                       IsRightOperandElement = exp.IsRightOperandElement
                                                   }).AsEnumerable(),
                                    ResultFailure = r.ResultFailure,
                                    ResultSuccess = r.ResultSuccess,
                                    IsResultFailureElement = r.IsResultFailureElement,
                                    IsResultSuccessElement = r.IsResultSuccessElement,
                                    Message = r.Message,
                                    RuleId = r.RuleID,
                                    TargetPropertyId = r.TargetPropertyID,
                                    TargetProperty = r.TargetProperty.TargetPropertyName,
                                    UIElementID = r.UIElementID
                                }).ToList();
                if (rowModelList.Count() == 0)
                {
                    rowModelList = null;
                }
                else
                {
                    foreach (var rowModel in rowModelList)
                    {
                        if (rowModel.Expressions != null && rowModel.Expressions.Count() > 0)
                        {
                            rowModel.RootExpression = GenerateHierarchicalExpression(rowModel.Expressions.ToList());
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return rowModelList;

            //-----------------------Initial --------------------------- 
        }

        private ExpressionRowModel GenerateHierarchicalExpression(List<ExpressionRowModel> expressionList)
        {
            var expression = expressionList.Where(n => n.ParentExpressionId == null).FirstOrDefault();
            if (expression != null)
            {
                GenerateParentExpression(expressionList, ref expression);
            }
            return expression;
        }

        private void GenerateParentExpression(List<ExpressionRowModel> expressionList, ref ExpressionRowModel parentExpression)
        {
            var parent = parentExpression;
            var childExpressions = from exp in expressionList where exp.ParentExpressionId == parent.ExpressionId select exp;
            if (childExpressions != null && childExpressions.Count() > 0)
            {
                parentExpression.Expressions = new List<ExpressionRowModel>();
                foreach (var childExpression in childExpressions)
                {
                    ExpressionRowModel child = childExpression;
                    GenerateParentExpression(expressionList, ref child);
                    parentExpression.Expressions.Add(childExpression);
                }
            }
        }

        private void DeleteIASElementImportLog(int GlobalUpdateID)
        {
            var iasElementImportLogToDelete = this._unitOfWork.RepositoryAsync<IASElementImport>()
                                            .Query()
                                            .Filter(c => c.GlobalUpdateID == GlobalUpdateID)
                                            .Get().ToList();

            if (iasElementImportLogToDelete.Any())
            {
                foreach (var item in iasElementImportLogToDelete)
                {
                    this._unitOfWork.RepositoryAsync<IASElementImport>().Delete(item);
                }
                this._unitOfWork.Save();
            }
        }

        #region DataSources Methods
        
        private ExpandoObject GetDataSource(int FormDesignID, int tenantID, int formInstanceID, int formDesignVersionID, int folderVersionID)
        {
            ExpandoObject sourceJsonObject = null;
            if (sourceInstances.ContainsKey(FormDesignID))
            {
                sourceJsonObject = sourceInstances[FormDesignID];
            }
            else
            {
                int sourceFormInstanceID = GetSourceFormInstanceID(formInstanceID, formDesignVersionID, folderVersionID, FormDesignID);
                string sourceFormInstanceData = GetFormInstanceData(tenantID, sourceFormInstanceID);
                var converter = new ExpandoObjectConverter();
                sourceJsonObject = JsonConvert.DeserializeObject<ExpandoObject>(sourceFormInstanceData, converter);
                sourceInstances.Add(FormDesignID, sourceJsonObject);
            }
            return sourceJsonObject;
        }

        private int GetSourceFormInstanceID(int formInstanceID, int formDesignVersionID, int folderVersionID, int sourceFormDesignID)
        {
            //determine if source form is from the same folder
            //if so, get form instance id for the source form and return
            //if not, get the effective date of the current folder version
            //and get the latest finalized form instance id before that effective date
            int sourceFormInstanceID = 0;
            var sourceGroup = (from grpSource in this._unitOfWork.RepositoryAsync<FormDesignGroupMapping>()
                                        .Query()
                                        .Filter(c => c.FormID == sourceFormDesignID)
                                        .Get()
                               select grpSource).FirstOrDefault();

            var formGroup = (from grpSource in this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                        .Query()
                                        .Include(c => c.FormDesign.FormDesignGroupMappings)
                                        .Filter(d => d.FormDesignVersionID == formDesignVersionID)
                                        .Get()
                             select grpSource.FormDesign.FormDesignGroupMappings).FirstOrDefault().FirstOrDefault();

            if (sourceGroup != null && formGroup != null)
            {
                if (sourceGroup.FormDesignGroupID == formGroup.FormDesignGroupID)
                {
                    //same folder
                    var formInst = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                                    .Query()
                                    .Filter(c => c.FormDesignID == sourceFormDesignID && c.FolderVersionID == folderVersionID && c.IsActive == true)
                                    .Get()
                                    select frm.FormInstanceID).FirstOrDefault();
                    if (formInst != null)
                    {
                        sourceFormInstanceID = formInst;
                    }
                }
                else
                {
                    //effective date code to determine form instance from another folder
                    var form = (from frmInst in this._unitOfWork.RepositoryAsync<FormInstance>()
                               .Query()
                               .Include(c => c.FolderVersion)
                               .Filter(d => d.FormInstanceID == formInstanceID && d.IsActive == true)
                               .Get()
                                select frmInst).FirstOrDefault();
                    if (form != null)
                    {
                        //Check If Any FolderVersion is Released from given FormDesignId, If there are
                        //multiple released versions pick the latest Released Version where EffectiveDate 
                        //should be lesser than or equal to Folder Version EffectiveDate
                        //else pick the latest Minor Version.

                        var releasedWorkflowState = _unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetReleasedWorkflowState(this._unitOfWork,folderVersionID);

                        var listOfVersions = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                                             .Query()
                                             .Include(c => c.FolderVersion)
                                             .Filter(h => h.FormDesignID == sourceFormDesignID &&
                                                    h.FolderVersion.EffectiveDate <= form.FolderVersion.EffectiveDate &&
                                                    h.FolderVersion.WFStateID == releasedWorkflowState.WorkFlowVersionStateID &&
                                                    h.FolderVersion.FolderVersionStateID == (int)FolderVersionState.RELEASED &&
                                                    h.IsActive == true)
                                             .Get()
                                              select frm.FolderVersion.FolderVersionNumber).ToList();
                        var maxFolderVersionNumber = "";
                        dynamic formInst = null;
                        if (listOfVersions.Count() > 0)
                        {
                            List<int> yearList = new List<int>();
                            yearList = (from version in listOfVersions select Convert.ToInt32(version.Split('_')[0])).ToList();

                            Double maxVersion = (from a in listOfVersions.Where(r => r.Contains(yearList.Max().ToString())).ToList()
                                                 select Convert.ToDouble(a.Split('_')[1])).Max();


                            for (int i = 0; i < listOfVersions.Count; i++)
                            {
                                if (listOfVersions[i].Contains(maxVersion.ToString()) && listOfVersions[i].Contains(yearList.Max().ToString()))
                                    maxFolderVersionNumber = listOfVersions[i];
                            }

                            formInst = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                                            .Query()
                                            .Include(c => c.FolderVersion)
                                            .Filter(h => h.FormDesignID == sourceFormDesignID &&
                                                        h.FolderVersion.EffectiveDate <= form.FolderVersion.EffectiveDate &&
                                                        h.FolderVersion.WFStateID == releasedWorkflowState.WorkFlowVersionStateID &&
                                                        h.FolderVersion.FolderVersionStateID == (int)FolderVersionState.RELEASED &&
                                                        h.IsActive == true &&
                                                        h.FolderVersion.FolderVersionNumber == maxFolderVersionNumber)
                                            .Get()
                                        select frm).FirstOrDefault();
                        }

                        if (formInst != null)
                        {
                            sourceFormInstanceID = formInst.FormInstanceID;
                        }
                        else
                        {
                            var latestformInst = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                                                       .Query()
                                                       .Include(c => c.FolderVersion)
                                                       .Filter(h => h.FormDesignID == sourceFormDesignID && h.FolderVersion.EffectiveDate < form.FolderVersion.EffectiveDate && h.IsActive == true)
                                                       .OrderBy(o => o.OrderByDescending(h => h.FolderVersion.EffectiveDate))
                                                       .Get()
                                                  select frm).FirstOrDefault();

                            if (latestformInst != null)
                                sourceFormInstanceID = latestformInst.FormInstanceID;
                        }
                    }
                }
            }
            return sourceFormInstanceID;
        }

        private string GetFormInstanceData(int tenantId, int formInstanceID)
        {
            string data = "";

            try
            {
                FormInstanceDataMap formInstance = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                   .Query()
                                                   .Filter(c => c.FormInstanceID == formInstanceID)
                                                   .Get()
                                                   .FirstOrDefault();

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

        private IList<Object> GetParentObject(ExpandoObject sourceObject, string fullParentName)
        {
            IList<Object> parent = new List<Object>();
            string[] parentElements = fullParentName.Split('.');
            IDictionary<string, object> vals = sourceObject as IDictionary<string, object>;
            foreach (var parentElement in parentElements)
            {
                if (vals is ExpandoObject)
                {
                    if (vals[parentElement] is System.Collections.Generic.IList<Object>)
                    {
                        parent = vals[parentElement] as System.Collections.Generic.IList<Object>;
                        break;
                    }
                    vals = vals[parentElement] as IDictionary<string, object>;
                }
            }
            return parent;
        }

        private DataSourceDesign GetDataSources(int formDesignId, int formDesignVersionId, FormDesignElementValueVeiwModel ElementSelection)
        {
            DataSourceDesign designs = new DataSourceDesign();
            var dataSourceList = (from ds in this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                        .Query()
                                                        .Include(b => b.DataSource)
                                                        .Include(c => c.UIElement)
                                                        .Include(d => d.UIElement1)
                                                        .Include(e => e.UIElement1.DataSourceElementDisplayMode)
                                                        .Include(e => e.DataSourceMode)
                                                        .Get()
                                  where ds.FormDesignVersionID == formDesignVersionId && ds.UIElementID == ElementSelection.UIElementID
                                  select new
                                  {
                                      FormDesignID = ds.UIElement1.FormID,
                                      SourceParentUIElementID = ds.UIElement1.ParentUIElementID.HasValue ? ds.UIElement1.ParentUIElementID.Value : 0,
                                      TargetParentUIElementID = ds.UIElement.ParentUIElementID.HasValue ? ds.UIElement.ParentUIElementID.Value : 0,
                                      DisplayModeUIElement = ds.UIElement.DataSourceElementDisplayMode,
                                      DisplayMode = ds.DataSourceElementDisplayMode,
                                      DataSourceModeType = ds.DataSourceMode.DataSourceModeType,
                                      DataSourceName = ds.DataSource.DataSourceName,
                                      IsPrimary = ds.IsPrimary,
                                      SourceUIElementID = ds.MappedUIElementID,
                                      TargetUIElementID = ds.UIElementID,
                                      SourceUIElementName = ds.UIElement1.GeneratedName,
                                      TargetUIElementName = ds.UIElement.GeneratedName,
                                      Sequence = ds.UIElement.Sequence,
                                      IsKey = ds.IsKey,
                                      Filter = ds.DataSourceFilter,
                                      Operator = ds.DataSourceOperatorID,
                                      CopyMode = ds.DataCopyModeID,
                                      IncludeChild = ds.IncludeChild,
                                  }).ToList();
            if (dataSourceList != null && dataSourceList.Count() > 0)
            {
                var designGroups = from ds in dataSourceList
                                   group ds by new { ds.TargetParentUIElementID, ds.DisplayMode.DisplayMode } into grouping
                                   select grouping;
                foreach (var dsGroup in designGroups)
                {
                    if (dsGroup.Key.DisplayMode == "Dropdown")
                    {
                        var srcGroups = (from src in dsGroup
                                         group src by new { src.DataSourceName, src.TargetUIElementID } into srcgroup
                                         select srcgroup);
                        foreach (var srcGroup in srcGroups)
                        {
                            var sr = from src in srcGroup select src;
                            if (sr != null && sr.Count() > 0)
                            {
                                formElementList = GetUIElemenstFromFormDesignVersion(formDesignVersionId);
                                var items = sr.ToList();
                                DataSourceDesign design = new DataSourceDesign();
                                design.DisplayMode = items[0].DisplayMode != null ? items[0].DisplayMode.DisplayMode : "";
                                design.FormDesignID = items[0].FormDesignID == formDesignId ? GetFormDesignID(items[0].DataSourceName) : items[0].FormDesignID;
                                design.DataSourceModeType = items[0].DataSourceModeType;
                                design.MappingType = design.MappingType;
                                design.DataSourceName = GetGeneratedName(items[0].DataSourceName);
                                var sourceParentElemntID = items[0].FormDesignID == formDesignId ? GetSourceParentUIElementID(items[0].DataSourceName, items[0].SourceParentUIElementID) : items[0].SourceParentUIElementID;
                                design.SourceParent = GetGeneratedNameForDataSource(design.FormDesignID, sourceParentElemntID);
                                design.TargetParent = GetGeneratedNameForDataSource(items[0].TargetParentUIElementID, formElementList);
                                var mappings = (from item in items.OrderBy(c => c.Sequence)
                                                select new DataSourceElementMapping
                                                {
                                                    SourceElement = item.SourceUIElementName,
                                                    TargetElement = item.TargetUIElementName,
                                                    IsKey = item.IsKey,
                                                    CopyModeID = item.CopyMode.HasValue == true ? item.CopyMode.Value : 0,
                                                    OperatorID = item.Operator.HasValue == true ? item.Operator.Value : 0,
                                                    Filter = item.Filter,
                                                    IncludeChild = item.IncludeChild
                                                });
                                if (mappings != null && mappings.Count() > 0)
                                {
                                    design.Mappings = mappings.ToList();
                                }
                                else
                                {
                                    design.Mappings = new List<DataSourceElementMapping>();
                                }
                                designs = design;
                            }
                        }
                    }
                }
            }

            return designs;
        }

        private int GetFormDesignID(string dataSourceName)
        {
            var formDesignID = this._unitOfWork.RepositoryAsync<DataSource>()
                                    .Query()
                                    .Filter(fil => fil.DataSourceName == dataSourceName)
                                    .Get()
                                    .Select(sel => sel.FormDesignID)
                                    .FirstOrDefault();
            return formDesignID;
        }

        private string GetGeneratedName(string label)
        {
            string generatedName = "";
            if (!String.IsNullOrEmpty(label))
            {
                Regex regex = new Regex("[^a-zA-Z0-9]");
                generatedName = regex.Replace(label, String.Empty);
                if (generatedName.Length > 70)
                {
                    generatedName = generatedName.Substring(0, 70);
                }

                Regex checkDigits = new Regex("^[0-9]");

                //if Label starts with numeric characters, this will append a character at the beginning.
                if (checkDigits.IsMatch(label, 0))
                {
                    generatedName = "a" + generatedName;
                }
            }
            return generatedName;
        }

        private int GetSourceParentUIElementID(string dataSourceName, int defaultParentElementID)
        {
            var formDesign = this._unitOfWork.RepositoryAsync<RepeaterUIElement>()
                                                     .Query()
                                                     .Filter(fil => fil.DataSource.DataSourceName == dataSourceName)
                                                     .Get().FirstOrDefault();

            if (formDesign == null)
            {
                return defaultParentElementID;
            }
            return formDesign.UIElementID;
        }

        private string GetGeneratedNameForDataSource(int formDesignID, int elementID)
        {
            List<UIElement> formElementList = (from u in this._unitOfWork.Repository<UIElement>()
                                                        .Query()
                                                        .Get()
                                               join fd in this._unitOfWork.Repository<FormDesignVersionUIElementMap>()
                                                                       .Query()
                                                                       .Get()
                                               on u.UIElementID equals fd.UIElementID
                                               where fd.FormDesignVersion.FormDesignID == formDesignID
                                               && u.IsContainer == true
                                               select u).ToList();
            return GetGeneratedNameForDataSource(elementID, formElementList);
        }

        private string GetGeneratedNameForDataSource(int elementID, List<UIElement> formElementList)
        {
            formElementList = (from el in formElementList where el.IsContainer == true select el).ToList();
            string generatedName = "";
            UIElement element = (from elem in formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            int currentElementID = elementID;
            int parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
            generatedName = element.GeneratedName;
            while (parentUIElementID > 0)
            {
                element = (from elem in formElementList
                           where elem.UIElementID == parentUIElementID
                           select elem).FirstOrDefault();
                parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
                if (parentUIElementID > 0)
                {
                    generatedName = element.GeneratedName + "." + generatedName;
                }
            }
            return generatedName;
        }

        #endregion DataSources Methods

        private List<Item> GetDropDownItems(int UIElementID)
        {
            List<Item> dropDownItemList = null;
            try
            {
                dropDownItemList = (from i in this._unitOfWork.Repository<DropDownElementItem>()
                                                                 .Query()
                                                                 .Filter(c => c.UIElementID == UIElementID)
                                                                 .Get()
                                    orderby i.Sequence
                                    select new Item
                                    {
                                        ItemID = i.ItemID,
                                        ItemValue = i.Value
                                    }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return dropDownItemList;

        }

        #endregion Private Methods

    }
}
