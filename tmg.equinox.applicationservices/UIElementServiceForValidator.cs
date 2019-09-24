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
        #region Validator Method
        public ValidatorModel GetValidator(int tenantId, int formDesignVersionId, int uiElementId)
        {
            ValidatorModel viewModel = null;
            try
            {
                Validator element = this._unitOfWork.RepositoryAsync<Validator>()
                                                            .Query()
                                                            .Include(c => c.UIElement)
                                                            .Include(c => c.UIElement.FormDesignVersionUIElementMaps)
                                                            .Filter(c => c.UIElementID == uiElementId && c.UIElement.FormDesignVersionUIElementMaps.FirstOrDefault().FormDesignVersionID == formDesignVersionId)
                                                            .Get()
                                                            .FirstOrDefault();

                if (element != null)
                {
                    viewModel = new ValidatorModel();

                    viewModel.ValidatorId = element.ValidatorID;
                    viewModel.UIElementId = element.UIElementID;
                    viewModel.Label = element.UIElement.Label;
                    viewModel.IsRequired = element.IsRequired.HasValue ? element.IsRequired.Value : false;
                    viewModel.LibraryRegexId = element.LibraryRegexID.HasValue ? element.LibraryRegexID.Value : 0;
                    viewModel.Regex = element.Regex;
                    viewModel.Message = element.Message;
                    viewModel.TenantId = tenantId;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return viewModel;
        }

        public ServiceResult AddValidator(string userName, int tenantId, int formDesignVersionId, int uiElementId, int libraryRegexId, bool isLibraryRegex, bool isRequired, string regex, string CustomRegexMessage)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                Validator validatorElement = new Validator
                {
                    UIElementID = uiElementId,
                    LibraryRegexID = libraryRegexId,
                    IsLibraryRegex = isLibraryRegex,
                    IsRequired = isLibraryRegex,
                    Regex = regex,
                    Message = CustomRegexMessage,
                    AddedBy = userName,
                    AddedDate = DateTime.Now
                };

                this._unitOfWork.RepositoryAsync<Validator>().Insert(validatorElement);
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

        public ServiceResult UpdateValidator(string userName, int tenantId, int formDesignVersionId, int uiElementId, int validatorId, int libraryRegexId, bool isLibraryRegex, bool isRequired, string regex, string CustomRegexMessage)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                Validator validatorElement = this._unitOfWork.RepositoryAsync<Validator>().FindById(validatorId);
                if (validatorElement != null)
                {
                    validatorElement.UIElementID = uiElementId;
                    validatorElement.LibraryRegexID = libraryRegexId;
                    validatorElement.IsLibraryRegex = isLibraryRegex;
                    validatorElement.IsRequired = isRequired;
                    validatorElement.Regex = regex;
                    validatorElement.Message = CustomRegexMessage;
                    validatorElement.UpdatedBy = userName;
                    validatorElement.UpdatedDate = DateTime.Now;

                    this._unitOfWork.RepositoryAsync<Validator>().Update(validatorElement);
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
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
        #endregion Validator Method
        #endregion Public Methods

        #region Private Methods
        private void AddValidator(string userName, int uiElementID, bool isRequired, Nullable<bool> isLibraryRegex, Nullable<int> LibraryRegexID, string regex, string CustomRegexMessage, bool isClone, bool maskFlag = false)
        {
            if (isClone)
            {
                Validator validator = new Validator();
                validator.AddedBy = userName;
                validator.AddedDate = DateTime.Now;
                validator.IsActive = true;
                validator.IsRequired = isRequired;
                validator.IsLibraryRegex = isLibraryRegex;
                validator.LibraryRegexID = LibraryRegexID;
                validator.Regex = SetRegex(regex);
                validator.Message = CustomRegexMessage;
                validator.MaskFlag = maskFlag;
                validator.UIElementID = uiElementID;
                this._unitOfWork.RepositoryAsync<Validator>().Insert(validator);
            }
            else
            {
                Validator validator = this._unitOfWork.RepositoryAsync<Validator>()
                                                                .Query()
                                                                .Filter(c => c.UIElementID == uiElementID)
                                                                .Get()
                                                                .FirstOrDefault();
                if (validator == null)
                {
                    Validator newVal = new Validator();
                    newVal.AddedBy = userName;
                    newVal.AddedDate = DateTime.Now;
                    newVal.IsActive = true;
                    newVal.IsRequired = isRequired;
                    newVal.IsLibraryRegex = LibraryRegexID != null && LibraryRegexID > 0 ? true : isLibraryRegex;
                    newVal.LibraryRegexID = LibraryRegexID;
                    newVal.Regex = SetRegex(regex);
                    newVal.Message = CustomRegexMessage;
                    newVal.MaskFlag = maskFlag;
                    newVal.UIElementID = uiElementID;
                    this._unitOfWork.RepositoryAsync<Validator>().Insert(newVal);
                }
                else
                {
                    validator.IsRequired = isRequired;
                    validator.IsLibraryRegex = LibraryRegexID != null && LibraryRegexID > 0 ? true : isLibraryRegex;
                    validator.LibraryRegexID = LibraryRegexID;
                    validator.Regex = SetRegex(regex);
                    validator.MaskFlag = maskFlag;
                    validator.Message = CustomRegexMessage;
                    validator.UpdatedBy = userName;
                    validator.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<Validator>().Update(validator);
                }
            }
        }

        /// <summary>
        /// Added method for checking if value of Regex isNullOrEmpty, used .Trim() to remove white spaces
        /// </summary>
        /// <param name="regex"></param>
        /// <returns></returns>
        private string SetRegex(string regex)
        {
            string returnRegex = string.Empty;
            returnRegex = string.IsNullOrEmpty(regex) ? null : regex.Trim();
            return returnRegex;
        }
        private void AddValidator(string userName, int uiElementID, Validator validator)
        {
            Validator val = new Validator();
            val.AddedBy = userName;
            val.AddedDate = DateTime.Now;
            val.IsActive = true;
            val.UIElementID = uiElementID;
            val.IsRequired = validator.IsRequired;
            val.LibraryRegexID = validator.LibraryRegexID;
            val.Regex = validator.Regex;
            val.Message = validator.Message;
            this._unitOfWork.RepositoryAsync<Validator>().Insert(val);
        }

        private void DeleteValidator(int uiElementID)
        {
            Validator val = this._unitOfWork.RepositoryAsync<Validator>()
                                                            .Query()
                                                            .Filter(c => c.UIElementID == uiElementID)
                                                            .Get()
                                                            .FirstOrDefault();
            if (val != null)
            {
                this._unitOfWork.RepositoryAsync<Validator>().Delete(val.ValidatorID);
            }
        }
        #endregion Private Methods
    }
}
