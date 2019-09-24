using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Reporting;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public partial class FolderVersionReportService : IFolderVersionReportService
    {
        #region Private Memebers

        private IUnitOfWork _unitOfWork { get; set; }

        #endregion Private Members

        #region Constructor

        public FolderVersionReportService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        #region ServicesMethod
        /// <summary>
        /// Gets the folder Name form which the form instances will be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="IsPortfolio"></param>
        /// <returns></returns>
        public IEnumerable<ReportingViewModel> GetFolderList(int tenantId, bool isPortfolio)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");

            IList<ReportingViewModel> folderList = null;
            try
            {
                if (isPortfolio)
                {
                    folderList = (from fld in this._unitOfWork.Repository<Folder>().Get()
                                  where (fld.TenantID == tenantId && fld.IsPortfolio == isPortfolio)
                                  select new ReportingViewModel
                                  {
                                      FolderId = fld.FolderID,
                                      FolderName = fld.Name
                                  }).OrderBy(ord => ord.FolderName).ToList();
                }
                else
                {
                    folderList = (from fld in this._unitOfWork.Repository<Folder>().Get()
                                  join fI in this._unitOfWork.Repository<AccountFolderMap>().Get()
                                            on fld.FolderID equals fI.FolderID
                                  join ac in this._unitOfWork.Repository<Account>().Get()
                                     on fI.AccountID equals ac.AccountID
                                  where (fld.TenantID == tenantId && ac.IsActive == true)
                                  select new ReportingViewModel
                                  {
                                      FolderId = fld.FolderID,
                                      FolderName = fld.Name,
                                      AccountName = ac.AccountName

                                  }).OrderBy(ord => ord.FolderName).ToList();
                }
                if (folderList.Count() == 0)
                    folderList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folderList;

        }

        /// <summary>
        /// Gets the folder version form which the form instances will be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="IsPortfolio"></param>
        /// <param name="FolderId">The folder identifier.</param>
        /// <returns></returns>
        public IEnumerable<ReportingViewModel> GetFolderVersionList(int tenantId, int folderId, bool isPortfolio)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");

            IList<ReportingViewModel> folderList = null;
            try
            {

                folderList = (from fld in this._unitOfWork.Repository<Folder>().Get()
                              //left join on folder version
                              join fldvrsn in this._unitOfWork.Repository<FolderVersion>().Get()
                              on fld.FolderID equals fldvrsn.FolderID
                              where (fld.TenantID == tenantId && fld.IsPortfolio == isPortfolio && fld.FolderID == folderId)
                              select new ReportingViewModel
                              {
                                  FolderVersionId = fldvrsn.FolderVersionID,
                                  FolderVersionNumber = (fldvrsn.FolderVersionNumber) + "_" + (fldvrsn.FolderVersionState.FolderVersionStateID == 3 ? "Major" : "Minor")
                              }).OrderBy(ord => ord.FolderVersionNumber).ToList();


                if (folderList.Count() == 0)
                    folderList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folderList;

        }

        /// <summary>
        /// Gets the folder version form which the form instances will be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        ///   <param name="sourceFolderVersionId">The folder version identifier.</param>
        ///   <param name="targetFolderVersionId">The folder version identifier.</param>
        /// <returns></returns>
        public IEnumerable<ReportingViewModel> GetSourceFormInstanceList(int tenantId, int sourceFolderVersionId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            IList<ReportingViewModel> formInstanceList = null;
            try
            {
                //genrate list first form based on folderVersionId
                formInstanceList = (from c in this._unitOfWork.Repository<FormInstance>()
                                              .Query()
                                              .Include(c => c.FormDesign)
                                              .Filter(c => c.TenantID == tenantId && c.FolderVersionID == sourceFolderVersionId && c.IsActive == true)
                                              .Get()
                                    select new ReportingViewModel
                                    {
                                        FormDesignId = c.FormDesignID,
                                        FormInstanceId = c.FormInstanceID,
                                        FormDesignVersionId = c.FormDesignVersionID,
                                        SourceForm = c.Name != null ? c.Name : c.FormDesign.FormName

                                    }).OrderBy(ord => ord.SourceForm).ToList();
                // genrate list of Second form based on folderVersionId              

                if (formInstanceList.Count() == 0)
                {
                    formInstanceList.Add(new ReportingViewModel() { FormDesignId = 0, TargetForm = "NA", SourceForm = "NA" });

                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return formInstanceList;

        }

        //get list of folder version
        /// <summary>
        /// Gets the list of  from name based on folder version identifer .
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>       
        ///   <param name="folderTwoVersionId">The folder version identifier.</param>
        /// <returns></returns>
        public IEnumerable<ReportingViewModel> GetTargetFormInstanceList(int tenantId, int folderVersionId)
        {
            Contract.Requires(folderVersionId > 0, "Invalid Folder Version Id ");
            Contract.Requires(tenantId > 0, "Invalid tenantId");


            IList<ReportingViewModel> formInstanceList = null;

            try
            {

                formInstanceList = (from c in this._unitOfWork.Repository<FormInstance>()
                                              .Query()
                                              .Include(c => c.FormDesign)
                                              .Filter(c => c.TenantID == tenantId && c.FolderVersionID == folderVersionId && c.IsActive == true)
                                              .Get()
                                    select new ReportingViewModel
                                    {

                                        FormDesignId = c.FormDesignID,
                                        FormInstanceId = c.FormInstanceID,
                                        TargetForm = c.Name != null ? c.Name : c.FormDesign.FormName

                                    }).OrderBy(ord => ord.TargetForm).ToList();



                if (formInstanceList.Count() == 0)
                {
                    formInstanceList = null;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return formInstanceList;
        }

        public IEnumerable<ReportingViewModel> GetUIElementList(int tenantId, int formInstanceId)
        {
            IList<ReportingViewModel> uiElementList = null;          
            try
            {
             var uiElement = (from fdElement in this._unitOfWork.Repository<FormDesignVersionUIElementMap>()
                                 .Query().Include(xy => xy.UIElement).Get()                                  
                                  join fI in this._unitOfWork.Repository<FormInstance>().Get()
                                        on fdElement.FormDesignVersionID equals fI.FormDesignVersionID                                                   
                                   join rd in this._unitOfWork.Repository<RadioButtonUIElement>().Get()
                                       on fdElement.UIElement.UIElementID equals rd.UIElementID
                                        into radioElement
                                 from uilist in radioElement.DefaultIfEmpty()
                                 where (fI.TenantID == tenantId && fI.FormInstanceID == formInstanceId && fI.IsActive == true)
                                  select new ReportingViewModel
                                  {
                                      GeneratedName = fdElement.UIElement.GeneratedName,
                                      UILabelName = fdElement.UIElement.Label,
                                      RadioOptionLabelYes = uilist.OptionLabel,
                                      RadioOptionLabelNo = uilist.OptionLabelNo,
                                      UIElementID = fdElement.UIElement.UIElementID,
                                      Visable=fdElement.UIElement.Visible,
                                      PropertyRule = fdElement.UIElement.PropertyRuleMaps.Select(x => x.TargetProperty.TargetPropertyName).FirstOrDefault(),
                                      UIElementName=fdElement.UIElement.UIElementName
                                  });             
             if (uiElement != null)
                {
                    uiElementList = uiElement.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return uiElementList;
        }

        public IEnumerable<ReportingViewModel> GetDataSourceList(int tenantId, int formInstanceId)
        {
            IList<ReportingViewModel> dataSourceList = null;
            try
            {
                dataSourceList = (from fI in this._unitOfWork.Repository<FormInstance>().Get()
                                  //left join on folder version
                                  join dS in this._unitOfWork.Repository<DataSource>().Get()
                            on fI.FormDesignID equals dS.FormDesignID
                                  where (fI.TenantID == tenantId && fI.FormInstanceID == formInstanceId && fI.IsActive == true)
                                  select new ReportingViewModel
                                  {
                                      DataSourceName = dS.DataSourceName,
                                      DataSourceId = dS.DataSourceID
                                  }).ToList();

                if (dataSourceList.Count() == 0)
                {
                    dataSourceList = null;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return dataSourceList;
        }
   
        public string GetFolderAccountName(int tenantId, int sourceFolderId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");

            IList<ReportingViewModel> folderList = null;
            string folderAccountName = "";
            try
            {

                folderList = (from acc in this._unitOfWork.Repository<Account>().Get()
                              //left join on folder version
                              join afm in this._unitOfWork.Repository<AccountFolderMap>().Get()
                              on acc.AccountID equals afm.AccountID
                              where (acc.TenantID == tenantId && afm.FolderID == sourceFolderId)
                              select new ReportingViewModel
                              {
                                  AccountName = acc.AccountName
                              }).ToList();


                if (folderList.Count() == 0)
                {
                    folderAccountName = "";
                }
                else
                { folderAccountName = folderList[0].AccountName; }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }


            return folderAccountName;
        }
        #endregion ServicesMethod

    }


}
