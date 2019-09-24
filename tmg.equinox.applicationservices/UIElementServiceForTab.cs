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
using System.Diagnostics.Contracts;

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
        public ServiceResult AddTab(string userName, int tenantId, int formDesignVersionId, string label, string helpText)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                TabUIElement tabElement = new TabUIElement();

                FormDesignVersion version = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                          .Query()
                                                          .Filter(c => c.FormDesignVersionID == formDesignVersionId)
                                                          .Get()
                                                          .FirstOrDefault();
                FormDesign form = this._unitOfWork.RepositoryAsync<FormDesign>()
                                          .Query()
                                          .Filter(c => c.FormID == version.FormDesignID)
                                          .Get()
                                          .FirstOrDefault();


                int layOutTypeID = this._unitOfWork.RepositoryAsync<LayoutType>().Query().Filter(l => l.LayoutTypeCode == "1COL").Get().Select(l => l.LayoutTypeID).FirstOrDefault(); // For 1Column grid layout

                int childCount = 0;

                tabElement.LayoutTypeID = layOutTypeID;
                tabElement.ChildCount = childCount;

                //Retrieve the UIElementType for TAB, assuming here that it's code in database will be SEC 
                tabElement.UIElementTypeID = this._unitOfWork.RepositoryAsync<UIElementType>()
                                                                    .Query()
                                                                    .Filter(c => c.UIElementTypeCode == "TAB")
                                                                    .Get()
                                                                    .Select(c => c.UIElementTypeID)
                                                                    .FirstOrDefault();
                //Retrieve the ApplicationDataType for TAB, assuming here that it's code in database will be NA 
                int uiElementDataTypeID = this._unitOfWork.RepositoryAsync<ApplicationDataType>()
                                                                    .Query()
                                                                    .Filter(c => c.ApplicationDataTypeName == "NA")
                                                                    .Get()
                                                                    .Select(c => c.ApplicationDataTypeID)
                                                                    .FirstOrDefault();
                tabElement.AddedBy = userName;
                tabElement.AddedDate = DateTime.Now;                                       //NOTE - Setting up default values here
                tabElement.Enabled = true;                                                 //Enable
                tabElement.HasCustomRule = false;                                          //HasCustomRule
                tabElement.IsActive = true;                                                //IsActive
                tabElement.IsContainer = true;                                             //IsContainer
                tabElement.Label = label;
                tabElement.GeneratedName = GetGeneratedName(label);
                tabElement.FormID = version.FormDesignID.Value;
                tabElement.RequiresValidation = false;                                     //RequiredValidation 
                tabElement.Sequence = 0;
                tabElement.Visible = true;
                tabElement.ParentUIElementID = null;
                tabElement.UIElementDataTypeID = uiElementDataTypeID;
                tabElement.UIElementName = form.Abbreviation + "TAB1";
                this._unitOfWork.RepositoryAsync<TabUIElement>().Insert(tabElement);
                this._unitOfWork.Save();

                //add FormDesignVersionUIElementMap record
                FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                map.FormDesignVersionID = formDesignVersionId;
                map.EffectiveDate = version.EffectiveDate;
                map.UIElementID = tabElement.UIElementID;
                this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                this._unitOfWork.Save();

                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
            }
            return result;
        }

        /// <summary>
        /// Updates the tab element.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="UIElementID">UIElement identifier.</param>
        /// <param name="customRule">custom Rule</param>
        /// <returns></returns>        
        public ServiceResult UpdateTab(string userName, int tenantId,int UIElementID,bool hasCustomRule,bool modifyCustomRules, string customRule)
        {
            Contract.Requires(!string.IsNullOrEmpty(userName), "Username cannot be empty");
            Contract.Requires(tenantId > 0, "Invalid tenantId");
             ServiceResult result = new ServiceResult();
            try
            {
                //TabUIElement tabElement = new TabUIElement();
                TabUIElement tabElement = this._unitOfWork.RepositoryAsync<TabUIElement>()
                                                                    .FindById(UIElementID);

                tabElement.UpdatedBy = userName;
                tabElement.UpdatedDate = DateTime.Now;
                tabElement.UIElementID = UIElementID;
                tabElement.HasCustomRule = hasCustomRule;
                if (modifyCustomRules)
                {
                    tabElement.HasCustomRule = hasCustomRule;
                    tabElement.CustomRule = customRule;
                }

                    this._unitOfWork.RepositoryAsync<UIElement>().Update(tabElement);
                    this._unitOfWork.Save();
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

        /// <summary>
        /// Gets the tab element details.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="tabElementId">Uielement identifier</param>
        /// <returns></returns>
        public TabElementModel  GetTabDetail(int tenantId, int tabElementId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            TabElementModel  viewModel = null;
            try
            {
                TabUIElement element = this._unitOfWork.RepositoryAsync<TabUIElement>()
                                                            .Query()
                                                            .Include(c => c.LayoutType)
                                                            .Filter(c => c.UIElementID == tabElementId)
                                                            .Get()
                                                            .SingleOrDefault();

                if (element != null)
                {
                    viewModel = new TabElementModel();
                    viewModel.CustomRule = element.CustomRule;
                    viewModel.HasCustomRule = element.HasCustomRule;
                }
                else
                {
                    viewModel = null;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return viewModel;
            
        }
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
