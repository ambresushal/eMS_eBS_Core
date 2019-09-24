using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using System.Transactions;
using tmg.equinox.infrastructure.exceptionhandling;

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

        #region Public Methods
        public ServiceResult UpdateFieldSequences(string userName, int tenantId, int formDesignVersionId, int parentElementId, IDictionary<int, int> uiElementIdSequences)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var fields = this._unitOfWork.RepositoryAsync<UIElement>()
                                                            .Query()
                                                            .Filter(c => (uiElementIdSequences.Keys.Contains(c.UIElementID) && c.ParentUIElementID == parentElementId))
                                                            .Get();
                List<UIElement> fieldList = fields.ToList();

                foreach (UIElement field in fieldList)
                {
                    field.Sequence = uiElementIdSequences[field.UIElementID];
                    field.UpdatedBy = userName;
                    field.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<UIElement>().Update(field);
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
        public ServiceResult UpdateFieldCheckDuplicate(string userName, int tenantId, int formDesignVersionId, int parentElementId, IDictionary<int, bool> uiElementId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var fields = this._unitOfWork.RepositoryAsync<UIElement>()
                                                            .Query()
                                                            .Filter(c => (uiElementId.Keys.Contains(c.UIElementID) && c.ParentUIElementID == parentElementId))
                                                            .Get();
                List<UIElement> fieldList = fields.ToList();

                foreach (UIElement field in fieldList)
                {
                    field.CheckDuplicate = uiElementId[field.UIElementID]; 
                    field.UpdatedBy = userName;
                    field.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<UIElement>().Update(field);
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


        public ServiceResult CopyElement(int tenantId, int formDesignVersionId, int uiElementId, string elementType, string userName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                switch (elementType)
                {
                    case "Textbox":
                    case "Label":
                    case "Multiline TextBox":
                    case "Rich TextBox":
                        TextBoxElementModel txtModel = this.GetTextBox(tenantId, formDesignVersionId, uiElementId);
                        result = this.UpdateTextBox(userName, txtModel.TenantID, txtModel.FormDesignID, txtModel.FormDesignVersionID, txtModel.UIElementID, txtModel.Enabled, txtModel.Visible, txtModel.HasCustomRule,
                            txtModel.HelpText, txtModel.IsLabel, txtModel.IsMultiLine, txtModel.IsRequired, txtModel.UIElementDataTypeID, txtModel.DefaultValue, txtModel.Label, txtModel.MaxLength, txtModel.Sequence,
                            txtModel.SpellCheck, txtModel.AllowGlobalUpdates, txtModel.IsLibraryRegex, txtModel.Regex, txtModel.CustomRegexMessage, txtModel.LibraryRegexID, null, false, false, txtModel.CustomRule,
                            txtModel.MaskFlag, txtModel.ViewType, txtModel.IsStandard, txtModel.MDMName);
                        break;
                    case "Checkbox":
                        CheckBoxElementModel chkModel = this.GetCheckBox(tenantId, formDesignVersionId, uiElementId);
                        result = this.UpdateCheckBox(userName, chkModel.TenantID, chkModel.FormDesignID, chkModel.FormDesignVersionID, chkModel.UIElementID, chkModel.Enabled,
                            chkModel.Visible, chkModel.HasCustomRule, chkModel.HelpText, chkModel.IsRequired, chkModel.Label, chkModel.OptionLabel, chkModel.Sequence, true,
                            null, false, false, chkModel.CustomRule, chkModel.AllowGlobalUpdates, chkModel.ViewType, chkModel.IsStandard, chkModel.MDMName);
                        break;
                    case "Dropdown List":
                    case "Dropdown TextBox":
                        DropDownElementModel dropModel = this.GetDropDown(tenantId, formDesignVersionId, uiElementId);
                        result = this.UpdateDropDown(userName, dropModel.TenantID, dropModel.FormDesignID, dropModel.FormDesignVersionID, dropModel.UIElementID, dropModel.Enabled,
                                dropModel.Visible, dropModel.HasCustomRule, dropModel.HelpText, dropModel.IsRequired, dropModel.SelectedValue, dropModel.Label, dropModel.Sequence,
                                dropModel.Items, null, false, false, dropModel.CustomRule, dropModel.UIElementDataTypeID, dropModel.IsDropDownTextBox, dropModel.IsSortRequired,
                                dropModel.IsLibraryRegex, dropModel.LibraryRegexID, dropModel.AllowGlobalUpdates, dropModel.IsDropDownFilterable, dropModel.ViewType,
                                dropModel.IsMultiSelect, dropModel.IsStandard, dropModel.MDMName);
                        break;
                    case "Radio Button":
                        RadioButtonElementModel rdoModel = this.GetRadioButton(tenantId, formDesignVersionId, uiElementId);
                        result = this.UpdateRadioButton(userName, rdoModel.TenantID, rdoModel.FormDesignID, rdoModel.FormDesignVersionID, rdoModel.UIElementID, rdoModel.Enabled,
                            rdoModel.Visible, rdoModel.HasCustomRule, rdoModel.HelpText, rdoModel.IsRequired, rdoModel.Label, rdoModel.OptionLabel, rdoModel.OptionLabelNo,
                            rdoModel.IsYesNo, rdoModel.Sequence, rdoModel.DefaultValue, null, false, false, rdoModel.CustomRule, rdoModel.AllowGlobalUpdates, rdoModel.ViewType,
                            rdoModel.IsStandard, rdoModel.MDMName);
                        break;
                    case "Calendar":
                        CalendarElementModel calModel = this.GetCalendar(tenantId, formDesignVersionId, uiElementId);
                        result = this.UpdateCalendar(userName, calModel.TenantID, calModel.FormDesignID, calModel.FormDesignVersionID, calModel.UIElementID, calModel.Enabled,
                                calModel.Visible, calModel.HasCustomRule, calModel.HelpText, calModel.IsRequired, calModel.Label, calModel.Sequence, calModel.DefaultDate,
                                calModel.MinDate, calModel.MaxDate, null, false, false, calModel.CustomRule, calModel.AllowGlobalUpdates, calModel.ViewType, calModel.IsStandard,
                                calModel.MDMName);
                        break;
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

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
