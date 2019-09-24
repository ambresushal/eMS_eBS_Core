using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using System.Diagnostics.Contracts;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Xml;
using Newtonsoft.Json;
using tmg.equinox.applicationservices.viewmodels.Collateral;
using System.Transactions;
using System.Data.SqlClient;
using System.Data;
using tmg.equinox.domain.entities.Utility;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.repository.extensions;
using Newtonsoft.Json.Linq;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using System.Linq.Expressions;
using System.Configuration;

namespace tmg.equinox.applicationservices
{
    public class CollateralService : ICollateralService
    {
        #region Private Memebers

        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private string UserName { get; set; }

        #endregion Private Members

        #region Constructor

        public CollateralService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        /// <summary>
        /// Gets the Form Design List for the specified TenantID.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public IEnumerable<FormDesignRowModel> GetFormDesignList(int tenantId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            IList<FormDesignRowModel> formDesignList = null;
            try
            {
                formDesignList = (from c in this._unitOfWork.RepositoryAsync<FormDesign>()
                                                                        .Query()
                                                                        .Filter(c => c.TenantID == tenantId && c.IsActive == true)
                                                                        .Get()
                                  select new FormDesignRowModel
                                  {
                                      FormDesignId = c.FormID,
                                      FormDesignName = c.FormName,
                                      DisplayText = c.DisplayText,
                                      TenantID = c.TenantID,
                                      DocumentDesignTypeID = c.DocumentDesignTypeID,
                                      DocumentLocationID = c.DocumentLocationID,
                                      IsAliasDesignMasterList = c.IsAliasDesignMasterList,
                                      UsesAliasDesignMasterList = c.UsesAliasDesignMasterList
                                  }).ToList();

                if (formDesignList.Count() == 0)
                    formDesignList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formDesignList;
        }

        /// <summary>
        /// Gets the Form Design Version List for the specified TenantID and FormDesignID.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignId"></param>
        /// <returns></returns>
        public IEnumerable<FormDesignVersionRowModel> GetFormDesignVersionList(int tenantId, int formDesignId)
        {
            IList<FormDesignVersionRowModel> formDesignVersionList = null;
            try
            {
                formDesignVersionList = (from c in this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                                 .Query()
                                                                                 .Filter(c => c.FormDesignID == formDesignId /*&& c.IsActive == true*/)
                                                                                 .OrderBy(c => c.OrderByDescending(d => d.AddedDate))
                                                                                 .Get()
                                         select new FormDesignVersionRowModel
                                         {
                                             FormDesignVersionId = c.FormDesignVersionID,
                                             FormDesignId = c.FormDesignID,
                                             Version = c.VersionNumber,
                                             StatusId = c.StatusID,
                                             StatusText = c.Status.Status1,
                                             EffectiveDate = c.EffectiveDate,
                                             //AddedBy = c.AddedBy,
                                             //AddedDate = c.AddedDate,
                                             //UpdatedBy = c.UpdatedBy,
                                             //UpdatedDate = c.UpdatedDate
                                         }).ToList();
                if (formDesignVersionList.Count() == 0)
                    formDesignVersionList = null;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formDesignVersionList;
        }

        /// <summary>
        /// Outputs the equivalent XML for the JSON of the specified Document Design Version.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public XmlDocument GetFormDesignVersionXML(int tenantId, int formDesignVersionId, ref string FileName)
        {
            XmlDocument doc = null;

            var formData = (from des in _unitOfWork.RepositoryAsync<FormDesign>().Get()
                            join desver in _unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                              on des.FormID equals desver.FormDesignID
                            join ins in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                              on desver.FormDesignVersionID equals ins.FormDesignVersionID 
                            join insdata in _unitOfWork.RepositoryAsync<FormInstanceDataMap>().Get()
                              on ins.FormInstanceID equals insdata.FormInstanceID
                            where desver.FormDesignID == formDesignVersionId && desver.TenantID == tenantId
                            && ins.FormDesignID==formDesignVersionId
                            orderby ins.FormInstanceID descending
                            select new
                            {
                                desver.FormDesignVersionID,
                                desver.FormDesignID,
                                insdata.FormInstanceID,
                                insdata.FormData,
                                des.FormName,
                                desver.VersionNumber
                            }).FirstOrDefault();

            if (formData != null)
            {
                doc = (XmlDocument)JsonConvert.DeserializeXmlNode(formData.FormData, "root");
                FileName = formData.FormName + "_" + formData.VersionNumber + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xml";
            }
            return doc;
        }

        /// <summary>
        /// Gets the Form Designs and it's Version Info.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReportTemplateModel> GetDocumentVersionInfo(int ReportTemplateVersionID)//, GridPagingRequest gridPagingRequest)
        {
            List<ReportTemplateModel> DocDesigns = null;
            int count = 0;

            try
            {
                //SearchCriteria criteria = new SearchCriteria();
                //criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                DocDesigns = (from des in _unitOfWork.RepositoryAsync<FormDesign>().Get()
                              join existingRptDocMap in (_unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                                                                    .Where(i => i.TemplateReportVersionID == ReportTemplateVersionID))
                              on des.FormID equals existingRptDocMap.FormDesignID into result
                              from res in result.DefaultIfEmpty()
                              where des.DocumentDesignTypeID == 11 //Collatral Designs
                              select new ReportTemplateModel
                              {
                                  TenantID = des.TenantID,
                                  DocumentDesignId = des.FormID,
                                  DocumentDesignName = des.FormName,
                                  DataSourceName = res.DataSourceName,
                                  SelectedVersion = new DocumentVersion() { VersionId = res != null ? res.FormDesignVersionID : 0 },
                                  TemplateReportFormDesignVersionMapID = res.TemplateReportFormDesignVersionMapID
                              }).Distinct().ToList();
                // .ApplySearchCriteria(criteria)
                //.ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                //.GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);

                if (DocDesigns.Any())
                {
                    //var mappedDocs = from desVer in _unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                    //                 join rptDocMap in (_unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                    //                                             .Where(i => i.TemplateReportVersionID == ReportTemplateVersionID))
                    //                 on desVer.FormDesignVersionID equals rptDocMap.FormDesignVersionID
                    //                 select new ReportTemplateModel
                    //                 {
                    //                     DocumentDesignId = rptDocMap.FormDesignID,
                    //                     DataSourceName = rptDocMap.DataSourceName,
                    //                     SelectedVersion = new DocumentVersion()
                    //                     {
                    //                         VersionId = rptDocMap.FormDesignVersionID,
                    //                         VersionNo = desVer.VersionNumber
                    //                     }
                    //                 };

                    foreach (var d in DocDesigns)
                    {
                        ReportTemplateModel obj = d ?? new ReportTemplateModel();

                        #region Adding Versions List specific to each of the Form Designs
                        if (d != null)
                        {
                            var dd = (from frv in _unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                                      where (frv.FormDesignID ?? 0) == d.DocumentDesignId
                                      select new DocumentVersion
                                      {
                                          VersionId = frv.FormDesignVersionID,
                                          VersionNo = frv.VersionNumber,
                                          EffctiveDate = frv.EffectiveDate
                                      }).ToList();

                            if (dd.Any())
                            {
                                d.DocumentVersions = dd;
                            }
                        }
                        #endregion

                        //if (mappedDocs.Any())
                        //{
                        //    var matched = mappedDocs.Where(i => i.DocumentDesignId == d.DocumentDesignId).FirstOrDefault();
                        //    if (matched != null)
                        //    {
                        //        d.DataSourceName = matched.DataSourceName;
                        //        d.SelectedVersion = matched.SelectedVersion;
                        //        d.IsSelected = "true";
                        //    }
                        //}                    

                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return DocDesigns; // new GridPagingResponse<ReportTemplateModel>(gridPagingRequest.page, count, gridPagingRequest.rows, DocDesigns);
        }

        /// <summary>
        /// Updates Report Template VErsion Location
        /// </summary>
        /// <param name="tenantid"></param>
        /// <param name="location"></param>
        /// <param name="locationid"></param>
        /// <param name="locationDesc"></param>
        /// <returns></returns>
        public ServiceResult SaveReportLocation(int tenantid, string location, int TemplateReportVersionID, ref int locationid, string locationName = "InFolder")
        {
            Contract.Requires(!string.IsNullOrEmpty(location), "Location can not be empty");
            Contract.Requires(!string.IsNullOrEmpty(locationName), "Location Description can not be empty");

            ServiceResult result = new ServiceResult();
            try
            {
                var DoesReportVersionExist = _unitOfWork.RepositoryAsync<TemplateReportLocation>().Get().Where(i => i.TemplateReportVersionID == TemplateReportVersionID).FirstOrDefault();

                if (DoesReportVersionExist != null)
                {
                    if (!string.IsNullOrEmpty(location) || locationName != DoesReportVersionExist.LocationName)
                    {
                        DoesReportVersionExist.LocationName = locationName;
                        if (!string.IsNullOrEmpty(location))
                            DoesReportVersionExist.LocationDescription = location;
                        DoesReportVersionExist.UpdatedDate = DateTime.Now;
                        DoesReportVersionExist.UpdatedBy = UserName;

                        _unitOfWork.RepositoryAsync<TemplateReportLocation>().Update(DoesReportVersionExist);
                        _unitOfWork.Save();
                    }

                    locationid = DoesReportVersionExist.LocationID;
                }
                else
                {
                    TemplateReportLocation reportLocation = new TemplateReportLocation();
                    reportLocation.TenantID = tenantid;
                    reportLocation.LocationName = locationName;
                    reportLocation.LocationDescription = location;
                    reportLocation.TemplateReportVersionID = TemplateReportVersionID;
                    reportLocation.AddedBy = UserName;
                    reportLocation.AddedDate = DateTime.Now;
                    reportLocation.IsActive = true;

                    this._unitOfWork.RepositoryAsync<TemplateReportLocation>().Insert(reportLocation);
                    this._unitOfWork.Save();
                    locationid = reportLocation.LocationID;
                }

                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public int GetLatestVersionNumber(int reportTemplateID)
        {
            int reportVersionNo = _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                                        .Where(i => i.TemplateReportID == reportTemplateID)
                                                        .Select(i => i.VersionNumber).Count();

            return reportVersionNo + 1;
        }

        /// <summary>
        /// Updates Report Template Version
        /// </summary>
        /// <param name="tenantid"></param>
        /// <param name="username"></param>
        /// <param name="path"></param>
        /// <param name="reportName"></param>
        /// <param name="reportDesc"></param>
        /// <param name="EffDate"></param>
        /// <param name="CanDate"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ServiceResult UpdateReportTemplateVersion(int tenantid, string username, string path, int reportVersionTemplateID, string reportName, string templateDocMappings, string templateProperties, string Parameters)
        {
            #region Variable Declarations
            List<ReportTemplateModel> docMappings = new List<ReportTemplateModel>();
            ReportPropertiesViewModel properties = new ReportPropertiesViewModel();
            List<ParameterViewModel> parameters = new List<ParameterViewModel>();
            int reportFormatTypeID = 1;
            int LocationID = 0;
            int reportTemplateID = 0;
            string FileName = string.Empty;
            UserName = username;
            if (!string.IsNullOrEmpty(path))
                FileName = path.Split(new char[] { '\\' })[path.Split(new char[] { '\\' }).Length - 1];
            if (!string.IsNullOrEmpty(templateDocMappings))
                docMappings = JsonConvert.DeserializeObject<List<ReportTemplateModel>>(templateDocMappings);
            if (!string.IsNullOrEmpty(templateProperties))
                properties = JsonConvert.DeserializeObject<ReportPropertiesViewModel>(templateProperties);
            if (!string.IsNullOrEmpty(Parameters))
                parameters = JsonConvert.DeserializeObject<List<ParameterViewModel>>(Parameters);
            #endregion

            ServiceResult result = new ServiceResult();
            try
            {
                using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
                {
                    # region Update Report Template Version
                    TemplateReportVersion reportVersion = _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                                              .Where(i => i.TemplateReportVersionID == reportVersionTemplateID)
                                                              .Select(i => i).FirstOrDefault();

                    if (reportVersion != null)
                    {
                        reportTemplateID = reportVersion.TemplateReportID;
                        reportVersion.Description = properties.ReportDescription;
                        reportVersion.HelpText = properties.HelpText;
                        reportVersion.IsVisible = properties.Visible;

                        if (!string.IsNullOrEmpty(FileName))                    // Overwrite the Path and Template Name only if there was a File Uploaded against the existing one.
                        {
                            reportVersion.Location = path;
                            reportVersion.TemplateFileName = FileName;
                        }
                        reportVersion.ReportFormatTypeID = reportFormatTypeID;
                        reportVersion.TenantID = tenantid;
                        reportVersion.UpdatedBy = UserName;
                        reportVersion.UpdatedDate = DateTime.Now;

                        this._unitOfWork.RepositoryAsync<TemplateReportVersion>().Update(reportVersion);
                        this._unitOfWork.Save();

                        result.Result = ServiceResultStatus.Success;
                    }
                    #endregion

                    #region Update Report Version Location.
                    if (result.Result == ServiceResultStatus.Success)
                        result = SaveReportLocation(tenantid, path, reportVersionTemplateID, ref LocationID, properties.Location);
                    #endregion

                    #region Update Report Version Document Mapping. ** Instead of Deleting the existing records and Inserting new ones, Updating the existing ones, saves Identity creation operation in Database + ghost record management..ok..that's it
                    if (result.Result == ServiceResultStatus.Success)
                    {
                        result.Result = ServiceResultStatus.Failure;
                        if (docMappings.Any())
                        {
                            //There will be only one mapping always
                            #region  Updating Existing document Mappings for the Report Version.
                            //int updatedItems = 0;
                            //int indexesToBePersisted = docMappings.Count() - 1;
                            var existingMapping = _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get().Where(i => i.TemplateReportVersionID == reportVersionTemplateID).FirstOrDefault();

                            if (existingMapping != null)
                            {
                                existingMapping.FormDesignID = docMappings[0].DocumentDesignId;
                                existingMapping.FormDesignVersionID = docMappings[0].FormDesignVersionID;
                                existingMapping.DataSourceName = docMappings[0].DataSourceName;
                                existingMapping.UpdatedBy = UserName;
                                existingMapping.UpdatedDate = DateTime.Now;

                                _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Update(existingMapping);
                                _unitOfWork.Save();
                            }
                            else
                            {
                                TemplateReportFormDesignVersionMap reportMap = new domain.entities.Models.TemplateReportFormDesignVersionMap();
                                reportMap.TemplateReportID = reportTemplateID;
                                reportMap.TemplateReportVersionID = reportVersionTemplateID;
                                reportMap.FormDesignID = docMappings[0].DocumentDesignId;
                                reportMap.FormDesignVersionID = docMappings[0].FormDesignVersionID;
                                reportMap.DataSourceName = docMappings[0].DataSourceName;
                                reportMap.TenantID = tenantid;
                                reportMap.AddedBy = UserName;
                                reportMap.AddedDate = DateTime.Now;
                                reportMap.IsActive = true;

                                this._unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Insert(reportMap);
                                this._unitOfWork.Save();
                            }
                            #endregion

                            //#region  Updating Existing document Mappings for the Report Version.
                            //int updatedItems = 0;
                            //int indexesToBePersisted = docMappings.Count() - 1;
                            //var Existing_ReportDocument_Mappings = _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get().Where(i => i.TemplateReportVersionID == reportVersionTemplateID);

                            //foreach (var item in Existing_ReportDocument_Mappings)
                            //{
                            //    if (indexesToBePersisted >= updatedItems)
                            //    {
                            //        int toBeUpdatedAt = indexesToBePersisted - updatedItems;
                            //        item.FormDesignID = docMappings[toBeUpdatedAt].DocumentDesignId;
                            //        item.FormDesignVersionID = docMappings[toBeUpdatedAt].SelectedVersion.VersionId;
                            //        item.DataSourceName = docMappings[toBeUpdatedAt].DataSourceName;
                            //        item.TenantID = tenantid;
                            //        item.UpdatedBy = UserName;
                            //        item.UpdatedDate = DateTime.Now;

                            //        _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Update(item);
                            //        _unitOfWork.Save();

                            //        updatedItems++;
                            //        docMappings.RemoveAt(toBeUpdatedAt);
                            //    }
                            //    else
                            //    {
                            //        _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Delete(item);
                            //        _unitOfWork.Save();
                            //    }
                            //}
                            //#endregion

                            //#region Add the left over New Document Mappings for the Report Version.
                            //foreach (var d in docMappings)
                            //{
                            //    var versionInfo = d.DVersion.Split(new char[] { '-' });

                            //    TemplateReportFormDesignVersionMap reportMap = new domain.entities.Models.TemplateReportFormDesignVersionMap();
                            //    reportMap.TemplateReportID = reportTemplateID;
                            //    reportMap.TemplateReportVersionID = reportVersionTemplateID;
                            //    reportMap.FormDesignID = d.DocumentDesignId;
                            //    reportMap.FormDesignVersionID = d.SelectedVersion.VersionId;
                            //    reportMap.DataSourceName = d.DataSourceName;
                            //    reportMap.TenantID = tenantid;
                            //    reportMap.AddedBy = UserName;
                            //    reportMap.AddedDate = DateTime.Now;
                            //    reportMap.IsActive = true;

                            //    this._unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Insert(reportMap);
                            //    this._unitOfWork.Save();
                            //}
                            //#endregion
                        }
                        result.Result = ServiceResultStatus.Success;
                    }
                    #endregion

                    #region Update Report Template Version Parameters
                    if (result.Result == ServiceResultStatus.Success)
                    {
                        if (parameters.Count() > 0)
                            result = UpdateReportPropertiesParameters(parameters, properties);
                    }
                    #endregion

                    if (result.Result == ServiceResultStatus.Success)
                        scope.Complete();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return result;
        }

        public GridPagingResponse<ReportDesignViewModel> GetReportNames(int tenantId, GridPagingRequest gridPagingRequest)
        {
            List<ReportDesignViewModel> data = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                data = (from report in _unitOfWork.RepositoryAsync<TemplateReport>().Get().Where(t => t.IsActive == true)
                        select new ReportDesignViewModel
                        {
                            ReportId = report.TemplateReportID,
                            ReportName = report.TemplateReportName
                        }).ApplySearchCriteria(criteria)
                          .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                          .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return new GridPagingResponse<ReportDesignViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, data);
        }

        /// <summary>
        /// Get the Report Name List for which Report Version and the Template File Exists.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public IEnumerable<ReportDesignViewModel> GetReportNamesForGeneration(int tenantId, string ReportLocation)
        {
            IEnumerable<ReportDesignViewModel> data = (from report in _unitOfWork.RepositoryAsync<TemplateReport>().Get()
                                                       join reportversion in _unitOfWork.Repository<TemplateReportVersion>().Get()
                                                        on report.TemplateReportID equals reportversion.TemplateReportID
                                                       join reportLocation in _unitOfWork.RepositoryAsync<TemplateReportLocation>().Get()
                                                        on reportversion.TemplateReportVersionID equals reportLocation.TemplateReportVersionID
                                                       join reportDocMap in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                                                        on reportversion.TemplateReportVersionID equals reportDocMap.TemplateReportVersionID
                                                       where !string.IsNullOrEmpty(reportversion.Location)
                                                        && (reportLocation.LocationName == ReportLocation || reportLocation.LocationName == "All")
                                                        && report.IsActive == true
                                                       select new ReportDesignViewModel
                                                       {
                                                           ReportId = report.TemplateReportID,
                                                           ReportName = report.TemplateReportName,
                                                           //ReportTemplateLocation = reportversion.Location,
                                                           //ReportTemplateVersionID = reportversion.TemplateReportVersionID,
                                                           //VersionNumber = reportversion.VersionNumber
                                                       }).Distinct().ToList();

            return data ?? new List<ReportDesignViewModel>();
        }

        /// <summary>
        /// Get the Report Template Version List
        /// </summary>
        /// <param name="TenantID"></param>
        /// <param name="ReportTemplateID"></param>
        /// <returns></returns>
        public GridPagingResponse<ReportTemplateVersionModel> GetReportTemplateVersionList(int TenantID, int ReportTemplateID, GridPagingRequest gridPagingRequest)
        {
            List<ReportTemplateVersionModel> reportTemplateVersionList = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                reportTemplateVersionList = (from rep in _unitOfWork.RepositoryAsync<TemplateReport>().Get()
                                             join repVer in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                             on rep.TemplateReportID equals repVer.TemplateReportID
                                             where rep.TemplateReportID == ReportTemplateID && rep.TenantID == TenantID
                                             && repVer.IsActive == true
                                             && rep.IsActive == true
                                             orderby repVer.TemplateReportVersionID descending
                                             select new ReportTemplateVersionModel
                                             {
                                                 ReportTemplateVersionID = repVer.TemplateReportVersionID,
                                                 EffectiveDate = repVer.EffectiveDate,
                                                 Status = (repVer.IsReleased ?? false) ? "Finalized" : "In Progress",
                                                 VersionNumber = repVer.VersionNumber,
                                                 TemplateLocation = repVer.Location,
                                                 TemplateName = repVer.TemplateFileName
                                             }).ToList().ApplySearchCriteria(criteria)
                                               .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                                               .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return new GridPagingResponse<ReportTemplateVersionModel>(gridPagingRequest.page, count, gridPagingRequest.rows, reportTemplateVersionList);
        }

        /// <summary>
        /// Add a Report Template
        /// </summary>
        /// <param name="TenantID"></param>
        /// <param name="ReportTemplateID"></param>
        /// <param name="ReportName"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public ServiceResult AddReportTemplate(int TenantID, int ReportTemplateID, string ReportName, string username)
        {
            string ReportType = "Folder";
            ServiceResult result = new ServiceResult();

            //reportExists = _unitOfWork.RepositoryAsync<TemplateReport>().Get().Where(i => i.TemplateReportName == reportName).FirstOrDefault();

            result.Result = ServiceResultStatus.Failure;

            TemplateReport report = new TemplateReport();
            //report.LocationID = LocationID;
            report.TemplateReportName = ReportName;
            report.TenantID = TenantID;
            //report.IsVisible = true;
            //report.HelpText = helpText;
            //report.IsRelease = true;
            report.ReportType = ReportType;
            report.AddedBy = username;
            //report.location = path;
            //report.TemplateFileName = FileName;
            report.AddedDate = DateTime.Now;
            report.IsActive = true;
            this._unitOfWork.RepositoryAsync<TemplateReport>().Insert(report);
            this._unitOfWork.Save();

            //reportID = report.TemplateReportID;
            result.Result = ServiceResultStatus.Success;
            List<ServiceResultItem> TemplateReportID = new List<ServiceResultItem>() { new ServiceResultItem() { Messages = new string[] { report.TemplateReportID.ToString() } } };
            result.Items = TemplateReportID;

            return result;
        }

        /// <summary>
        /// Update the specified Report Template
        /// </summary>
        /// <param name="TenantID"></param>
        /// <param name="ReportTemplateID"></param>
        /// <param name="ReportName"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public ServiceResult UpdateReportTemplate(int TenantID, int ReportTemplateID, string ReportName, string username)
        {
            string ReportType = "Folder";
            ServiceResult result = new ServiceResult();

            var ExistingRecord = _unitOfWork.RepositoryAsync<TemplateReport>().Get().Where(i => i.TemplateReportID == ReportTemplateID && i.IsActive == true).FirstOrDefault();

            if (ExistingRecord != null)
            {
                ExistingRecord.TemplateReportName = ReportName;
                ExistingRecord.UpdatedBy = username;
                ExistingRecord.UpdatedDate = DateTime.Now;
            }

            this._unitOfWork.RepositoryAsync<TemplateReport>().Update(ExistingRecord);
            this._unitOfWork.Save();

            //result.Result = ServiceResultStatus.Failure;

            //TemplateReport report = new TemplateReport();
            ////report.LocationID = LocationID;
            //report.TemplateReportName = ReportName;
            //report.TenantID = TenantID;
            ////report.IsVisible = true;
            ////report.HelpText = helpText;
            ////report.IsRelease = true;
            //report.ReportType = ReportType;
            //report.AddedBy = username;
            ////report.location = path;
            ////report.TemplateFileName = FileName;
            //report.AddedDate = DateTime.Now;

            //this._unitOfWork.RepositoryAsync<TemplateReport>().Insert(report);
            //this._unitOfWork.Save();

            ////reportID = report.TemplateReportID;


            result.Result = ServiceResultStatus.Success;

            return result;
        }

        public ServiceResult DeleteReportTemplate(int TenantID, int ReportTemplateID, string ReportName, string username)
        {
            string ReportType = "Default";
            ServiceResult result = new ServiceResult();

            //reportExists = _unitOfWork.RepositoryAsync<TemplateReport>().Get().Where(i => i.TemplateReportName == reportName).FirstOrDefault();

            result.Result = ServiceResultStatus.Failure;

            TemplateReport report = new TemplateReport();
            //report.LocationID = LocationID;
            report.TemplateReportName = ReportName;
            report.TenantID = TenantID;
            //report.IsVisible = true;
            //report.HelpText = helpText;
            //report.IsRelease = true;
            report.ReportType = ReportType;
            report.AddedBy = username;
            //report.location = path;
            //report.TemplateFileName = FileName;
            report.AddedDate = DateTime.Now;

            this._unitOfWork.RepositoryAsync<TemplateReport>().Insert(report);
            this._unitOfWork.Save();

            //reportID = report.TemplateReportID;
            result.Result = ServiceResultStatus.Success;

            return result;


        }

        /// <summary>
        /// Add a Report Template Version
        /// </summary>
        /// <param name="TenantID"></param>
        /// <param name="ReportTemplateID"></param>
        /// <param name="EffectiveDate"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public ServiceResult AddReportTemplateVersion(int TenantID, int ReportTemplateID, string EffectiveDate, string username, int reportTemplateVersionID)
        {
            Contract.Requires(!string.IsNullOrEmpty(EffectiveDate), "Effective Date is required Input.");
            ServiceResult result = new ServiceResult();

            try
            {
                DateTime dt;
                DateTime.TryParse(EffectiveDate, out dt);
                DateTime CancelDate = new DateTime(2099, 01, 01);
                int reportVersionNo = 0;

                /* Update Cancel Date of the Previous Version to be the day before the EffectiveDate input here. */
                TemplateReportVersion updatePreviousVersion = _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                                      .Where(i => i.TemplateReportID == ReportTemplateID)
                                                      .OrderByDescending(i => i.EffectiveDate)
                                                      .FirstOrDefault();

                if (updatePreviousVersion != null)
                {
                    updatePreviousVersion.CancelDate = dt.AddDays(-1);
                    _unitOfWork.RepositoryAsync<TemplateReportVersion>().Update(updatePreviousVersion);
                    _unitOfWork.Save();
                }


                /* Add new Entry */
                TemplateReportVersion version = new TemplateReportVersion();

                reportVersionNo = _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                        .Where(i => i.TemplateReportID == ReportTemplateID && i.IsActive == true)
                                        .Select(i => i.VersionNumber).Count();

                reportVersionNo = reportVersionNo + 1;

                version.TemplateReportID = ReportTemplateID;
                version.VersionNumber = reportVersionNo + ".0";
                version.AddedDate = DateTime.Now;
                version.EffectiveDate = dt;
                version.CancelDate = CancelDate;
                version.AddedBy = username;
                version.ReportFormatTypeID = 1;
                version.IsActive = true;

                //set TemplateReportVersion properties from previous version in case of copy
                if (reportTemplateVersionID > 0)
                {
                    TemplateReportVersion sourceRptVersion = this._unitOfWork.RepositoryAsync<TemplateReportVersion>()
                                                            .Query()
                                                            .Filter(c => c.TemplateReportVersionID == reportTemplateVersionID)
                                                            .Get().FirstOrDefault();

                    version.Description = sourceRptVersion.Description;
                    version.HelpText = sourceRptVersion.HelpText;
                    version.Location = sourceRptVersion.Location;
                    version.TemplateFileName = sourceRptVersion.TemplateFileName;
                    version.IsVisible = sourceRptVersion.IsVisible;
                }

                _unitOfWork.RepositoryAsync<TemplateReportVersion>().Insert(version);
                _unitOfWork.Save();

                int templateReportVersionID = _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                       .Where(i => i.TemplateReportID == ReportTemplateID)
                                       .Select(i => i.TemplateReportVersionID).Max();

                TemplateReportRoleAccessPermission newItemToAdd = new TemplateReportRoleAccessPermission();
                newItemToAdd.RoleID = (int)UserRoleEnum.SuperUser;  //  Super User  24
                newItemToAdd.TemplateReportVersionID = templateReportVersionID;
                newItemToAdd.TenantID = 1;
                newItemToAdd.AddedBy = username;
                newItemToAdd.AddedDate = DateTime.Now;
                newItemToAdd.IsActive = true;

                this._unitOfWork.RepositoryAsync<TemplateReportRoleAccessPermission>().Insert(newItemToAdd);
                this._unitOfWork.Save();

                if (reportTemplateVersionID == 0)
                {
                    result.Result = ServiceResultStatus.Success;
                }
                else //If copy option is selected then insert associated details from previous version
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(AppSettings.TransactionTimeOutPeriod)))
                    {
                        //Copy TemplateReportFormDesignVersionMap from previous version to new version
                        IList<TemplateReportFormDesignVersionMap> formDesignVersionMapList = null;
                        formDesignVersionMapList = this._unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>()
                                                                .Query()
                                                                .Filter(c => c.TemplateReportVersionID == reportTemplateVersionID)
                                                                .Get()
                                                                .ToList();

                        if (formDesignVersionMapList != null && formDesignVersionMapList.Count > 0)
                        {
                            foreach (TemplateReportFormDesignVersionMap map in formDesignVersionMapList)
                            {
                                TemplateReportFormDesignVersionMap templateReportFormDesignVersionMap = new TemplateReportFormDesignVersionMap
                                {
                                    TemplateReportID = map.TemplateReportID,
                                    TemplateReportVersionID = newItemToAdd.TemplateReportVersionID,
                                    FormDesignID = map.FormDesignID,
                                    FormDesignVersionID = map.FormDesignVersionID,
                                    DataSourceName = map.DataSourceName,
                                    TenantID = map.TenantID,
                                    IsActive = map.IsActive,
                                    AddedBy = username,
                                    AddedDate = DateTime.Now
                                };
                                this._unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Insert(templateReportFormDesignVersionMap);
                            }
                        }

                        //Copy TemplateReportLocation from previous version to new version
                        IList<TemplateReportLocation> templateReportLocationList = null;
                        templateReportLocationList = this._unitOfWork.RepositoryAsync<TemplateReportLocation>()
                                                                .Query()
                                                                .Filter(c => c.TemplateReportVersionID == reportTemplateVersionID)
                                                                .Get()
                                                                .ToList();

                        if (templateReportLocationList != null && templateReportLocationList.Count > 0)
                        {
                            foreach (TemplateReportLocation reportLoc in templateReportLocationList)
                            {
                                TemplateReportLocation templateReportLocation = new TemplateReportLocation
                                {
                                    LocationName = reportLoc.LocationName,
                                    LocationDescription = reportLoc.LocationDescription,
                                    TemplateReportVersionID = newItemToAdd.TemplateReportVersionID,
                                    AddedBy = username,
                                    AddedDate = DateTime.Now,
                                    TenantID = reportLoc.TenantID,
                                    IsActive = reportLoc.IsActive
                                };
                                this._unitOfWork.RepositoryAsync<TemplateReportLocation>().Insert(templateReportLocation);
                            }
                        }

                        //Copy TemplateReportLocation from previous version to new version
                        IList<TemplateReportVersionParameter> templateReportVersionParameterList = null;
                        templateReportVersionParameterList = this._unitOfWork.RepositoryAsync<TemplateReportVersionParameter>()
                                                                .Query()
                                                                .Filter(c => c.TemplateReportVersionID == reportTemplateVersionID)
                                                                .Get()
                                                                .ToList();

                        if (templateReportVersionParameterList != null && templateReportVersionParameterList.Count > 0)
                        {
                            foreach (TemplateReportVersionParameter reportVersionParameter in templateReportVersionParameterList)
                            {
                                TemplateReportVersionParameter templateReportVersionParameter = new TemplateReportVersionParameter
                                {
                                    ParameterTypeID = reportVersionParameter.ParameterTypeID,
                                    TemplateReportVersionID = newItemToAdd.TemplateReportVersionID,
                                    AddedBy = username,
                                    AddedDate = DateTime.Now,
                                    TenantID = reportVersionParameter.TenantID,
                                    IsActive = reportVersionParameter.IsActive
                                };
                                this._unitOfWork.RepositoryAsync<TemplateReportVersionParameter>().Insert(templateReportVersionParameter);
                            }
                        }

                        //Copy TemplateReportRoleAccessPermission from previous version to new version
                        IList<TemplateReportRoleAccessPermission> templateReportRoleAccessPermissionList = null;
                        templateReportRoleAccessPermissionList = this._unitOfWork.RepositoryAsync<TemplateReportRoleAccessPermission>()
                                                                .Query()
                                                                .Filter(c => c.TemplateReportVersionID == reportTemplateVersionID)
                                                                .Get()
                                                                .ToList();

                        if (templateReportRoleAccessPermissionList != null && templateReportRoleAccessPermissionList.Count > 0)
                        {
                            foreach (TemplateReportRoleAccessPermission accessPermission in templateReportRoleAccessPermissionList)
                            {
                                TemplateReportRoleAccessPermission templateReportRoleAccessPermission = new TemplateReportRoleAccessPermission
                                {
                                    RoleID = accessPermission.RoleID,
                                    TemplateReportVersionID = newItemToAdd.TemplateReportVersionID,
                                    AddedBy = username,
                                    AddedDate = DateTime.Now,
                                    TenantID = accessPermission.TenantID,
                                    IsActive = accessPermission.IsActive
                                };
                                this._unitOfWork.RepositoryAsync<TemplateReportRoleAccessPermission>().Insert(templateReportRoleAccessPermission);
                            }
                        }

                        this._unitOfWork.Save();
                        result.Result = ServiceResultStatus.Success;
                        scope.Complete();
                    }
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


        /// <summary>
        /// Add a Report Template Version
        /// </summary>
        /// <param name="TenantID"></param>
        /// <param name="ReportTemplateID"></param>
        /// <param name="EffectiveDate"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public ServiceResult UpdateReportTemplateVersion(int TenantID, int ReportTemplateVersionID, string EffectiveDate, string username)
        {
            Contract.Requires(!string.IsNullOrEmpty(EffectiveDate), "Effective Date is required Input.");
            ServiceResult result = new ServiceResult();

            try
            {
                DateTime dt;
                DateTime.TryParse(EffectiveDate, out dt);
                DateTime CancelDate = new DateTime(2099, 01, 01);


                /* Update Cancel Date of the Previous Version to be the day before the EffectiveDate input here. */
                //TemplateReportVersion updatePreviousVersion = _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                //                                      .Where(i => i.TemplateReportVersionID == ReportTemplateVersionID)
                //                                      .OrderByDescending(i => i.EffectiveDate)
                //                                      .FirstOrDefault();

                //if (updatePreviousVersion != null)
                //{
                //    updatePreviousVersion.CancelDate = dt.AddDays(-1);
                //    _unitOfWork.RepositoryAsync<TemplateReportVersion>().Update(updatePreviousVersion);
                //    _unitOfWork.Save();
                //}


                /* Add new Entry */
                TemplateReportVersion itemToUpdate = _unitOfWork.RepositoryAsync<TemplateReportVersion>().FindById(ReportTemplateVersionID);

                if (itemToUpdate != null)
                {
                    itemToUpdate.EffectiveDate = dt;
                    itemToUpdate.UpdatedBy = username;
                    itemToUpdate.UpdatedDate = DateTime.Now;

                    _unitOfWork.RepositoryAsync<TemplateReportVersion>().Update(itemToUpdate);
                    _unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
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


        public ServiceResult DeleteReportTemplateVersion(int TenantId, int ReportTemplateVersionID)
        {
            ServiceResult result = new ServiceResult();

            try
            {
                SqlParameter paramReportTemplateVersionID = new SqlParameter("@TemplateReportVersionID", ReportTemplateVersionID);
                var res = this._unitOfWork.Repository<TemplateReportVersion>().ExecuteUpdateSql("EXEC [dbo].[usp_DeleteTemplateReportVersion] @TemplateReportVersionID", paramReportTemplateVersionID);
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

        public List<ReportDocumentModel> GetDocumentsforReportGeneration(int ReportTemplateVersionID, int AccountID, int FolderID, int FolderVersionID, Array formInstanceIDList)
        {
            List<int> list = new List<int>();
            int valid = 0;
            foreach (string s in formInstanceIDList)
            {
                if (int.TryParse(s, out valid))
                    list.Add(Convert.ToInt32(valid));
                valid = 0;
            }
            return GetDocumentsforReportGeneration(ReportTemplateVersionID, AccountID, FolderID, FolderVersionID, false).Where(g => list.Contains(g.FormInstanceID)).ToList();
        }

        private bool ValidateReportEffectiveDateToBeGreaterThanFolderVersionOne(int FolderVersionID, DateTime? EffectiveDate)
        {
            return (_unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(i => i.FolderVersionID == FolderVersionID && i.EffectiveDate <= EffectiveDate).Any());
        }

        /// <summary>
        /// Select the Report Template for Report generation based on the specified ReportID and criteria defined on EffectiveDate and mapped Documents.
        /// </summary>
        /// <param name="ReportTemplateID"></param>
        /// <param name="FolderVersionID"></param>
        /// <param name="formInstanceIDList"></param>
        /// <param name="TemplateReportVersionID"></param>
        /// <param name="reportTemplateLocation"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetReportTemplateFromReportID(int ReportTemplateID, int FolderVersionID, IEnumerable<string> formInstanceIDList, ref int TemplateReportVersionID, ref string reportTemplateLocation, ref string status, string folderVersionEffDt)
        {
            //List<string> result = new List<string>(); 
            int templateReportVersionID = 0;
            int FormInstanceID = 0;
            Dictionary<string, int> ReportData = new Dictionary<string, int>();
            TemplateReportVersion SelectedrptVrsn = new TemplateReportVersion();

            DateTime? effDt = Convert.ToDateTime(folderVersionEffDt);

            int templateRptVersionID = 0, finalizedVersionsCnt = 0;
            bool? isFinalized = null;

            var incomingFormDesignIDs = _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                  .Where(i => formInstanceIDList.ToList().Contains(i.FormInstanceID.ToString()))
                                                  .Select(i => i.FormDesignID);

            //Get number of finalized versions
            finalizedVersionsCnt = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                    where rptvrsn.TemplateReportID == ReportTemplateID
                                         && rptvrsn.IsActive == true
                                         && rptvrsn.IsReleased == true
                                    select rptvrsn).Count();

            //Set a flag to true if finalized versions exists
            if (finalizedVersionsCnt > 0)
                isFinalized = true;

            templateRptVersionID = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                    where rptvrsn.TemplateReportID == ReportTemplateID
                                         && rptvrsn.EffectiveDate < effDt
                                         && rptvrsn.IsReleased == isFinalized
                                         && rptvrsn.IsActive == true
                                    orderby (rptvrsn.VersionNumber) descending
                                    select rptvrsn.TemplateReportVersionID).FirstOrDefault();

            SelectedrptVrsn = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get() //_unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(i => i.FolderVersionID == FolderVersionID).SelectMany(s => _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get().Where(k => k.TemplateReportID == ReportTemplateID && k.EffectiveDate >= s.EffectiveDate));
                               join rptDocMap in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                                on rptvrsn.TemplateReportVersionID equals rptDocMap.TemplateReportVersionID
                               where rptvrsn.TemplateReportID == ReportTemplateID
                                 && !string.IsNullOrEmpty(rptvrsn.TemplateFileName)
                                 && !rptvrsn.TemplateReportFormDesignVersionMaps.Select(i => i.FormDesignID).ToList().Except(incomingFormDesignIDs).Any()
                                 && rptvrsn.TemplateReportVersionID == templateRptVersionID  // TemplateReportVersionID of report version selected as per folder version effective date
                               orderby rptvrsn.EffectiveDate descending
                               select rptvrsn).FirstOrDefault();

            if (SelectedrptVrsn != null)
            {
                if (string.IsNullOrEmpty(SelectedrptVrsn.Location))
                    status = "Report Template File not present.";
                else
                {
                    TemplateReportVersionID = SelectedrptVrsn.TemplateReportVersionID;
                    templateReportVersionID = TemplateReportVersionID;
                    reportTemplateLocation = SelectedrptVrsn.Location;

                    foreach (var instance in formInstanceIDList)
                    {
                        string dataSourceName = string.Empty;
                        if (Int32.TryParse(instance, out FormInstanceID))
                        {
                            dataSourceName = (from ins in _unitOfWork.RepositoryAsync<FormInstance>().Get()//.Where(i => instance.Contains(i.FormInstanceID.ToString()));
                                              join rptmap in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                                                on ins.FormDesignID equals rptmap.FormDesignID
                                              where rptmap.TemplateReportVersionID == SelectedrptVrsn.TemplateReportVersionID
                                                && ins.FormInstanceID == FormInstanceID
                                              select rptmap.DataSourceName).FirstOrDefault();

                            if (!string.IsNullOrEmpty(dataSourceName))
                                ReportData.Add(dataSourceName, FormInstanceID);
                        }
                    }
                }
            }
            else
                status = "Report Template with specified Effective Date or with the selected Forms is not present.";

            return ReportData;
        }


        /// <summary>
        /// Gets the Accounts, related Folders, related FolderVersions and related Documents, applicable for a Report based on the Document Mapping for the Report.
        /// </summary>
        /// <param name="ReportName"></param>
        /// <param name="AccountID"></param>
        /// <param name="FolderVersionID"></param>
        /// <param name="fetchPermittedAccountsFolders"></param>
        /// <returns></returns>
        public IEnumerable<ReportDocumentModel> GetDocumentsforReportGeneration(int ReportTemplateID, int AccountID, int FolderID, int FolderVersionID, bool fetchPermittedAccountsFolders = true)
        {
            IEnumerable<ReportDocumentModel> data = null;
            IEnumerable<int> formDesignIDs = null;
            IEnumerable<int> anchorDesignIDs = null;

            try
            {
                formDesignIDs = (from formDesignVersionMap in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                                 where formDesignVersionMap.IsActive == true
                                 && formDesignVersionMap.TemplateReportID == ReportTemplateID
                                 select (formDesignVersionMap.FormDesignID)).Distinct().ToList();

                // Get anchorDesignIDs from formDesignIDs
                anchorDesignIDs = (from desMapping in _unitOfWork.RepositoryAsync<FormDesignMapping>().Get()
                                   where formDesignIDs.Contains(desMapping.TargetDesignID)
                                   select desMapping.AnchorDesignID).Distinct().ToList();
                //AD- 24 May 2017 -> To fetch both account & portfolio, changed query to left join on Account
                data = (from fldr in _unitOfWork.RepositoryAsync<Folder>().Get()
                        join fldrvrsn in _unitOfWork.RepositoryAsync<FolderVersion>().Get()
                            on fldr.FolderID equals fldrvrsn.FolderID
                        join formins in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                            on fldrvrsn.FolderVersionID equals formins.FolderVersionID
                        join des in _unitOfWork.RepositoryAsync<FormDesign>().Get()
                            on formins.FormDesignID equals des.FormID
                        join desginMapping in _unitOfWork.RepositoryAsync<FormDesignGroupMapping>().Get()
                            on formins.FormDesignID equals desginMapping.FormID
                            into tmp
                        from desginMapping in tmp.DefaultIfEmpty()
                        join accFldrmap in _unitOfWork.RepositoryAsync<AccountFolderMap>().Get()
                              on fldr.FolderID equals accFldrmap.FolderID
                              into tempAccMap
                        from accFldrmap in tempAccMap.DefaultIfEmpty()
                        join acc in _unitOfWork.RepositoryAsync<Account>().Get()
                            on accFldrmap.AccountID equals acc.AccountID
                            into tmpAcc
                        from acc in tmpAcc.DefaultIfEmpty()
                        where fldrvrsn.IsActive && formins.IsActive && des.IsActive
                            && (AccountID == 0 || acc.AccountID == AccountID)
                            && (FolderID == 0 || fldr.FolderID == FolderID)
                            && (FolderVersionID == 0 || fldrvrsn.FolderVersionID == FolderVersionID)
                            && (anchorDesignIDs.Contains(formins.FormDesignID)) // To get only those products whoes FormDesignID is used as Anchor product in report designs
                        select new ReportDocumentModel
                        {
                            AccountID = acc.AccountID,
                            AccountName = acc.AccountName,
                            FolderID = fldr.FolderID,
                            FolderName = fldr.Name,
                            FolderVersionID = fldrvrsn.FolderVersionID,
                            FolderVersionEffectiveDate = fldrvrsn.EffectiveDate,
                            FolderVersionNumber = fldrvrsn.FolderVersionNumber,
                            FormDesignID = formins.FormDesignID,
                            FormDesignVersionID = formins.FormDesignVersionID,
                            FormInstanceID = formins.FormInstanceID,
                            FormName = formins.Name,
                            DocumentType = des.FormName,
                            IsSelected = desginMapping.AllowMultipleInstance == false ? "true" : "",//Auto Selecting the Instances for which the DocumentDesigns are configured as "Non-Mutiple Instance Forms".
                            FormInstanceIdForReportGeneration = formins.FormInstanceID
                        }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                //Get all the exception/inner exception messages and set the return code to failure                
            }

            return data ?? new List<ReportDocumentModel>();
        }


        /// <summary>
        /// Gets the DataSource used for Report Generation.
        /// </summary>
        /// <param name="formInstanceIDs"></param>
        /// <param name="dataSourceNamess"></param>
        /// <param name="reportTemplateVersionID"></param>
        /// <param name="reportTemplateName"></param>
        /// <param name="reportTemplateLocation"></param>
        /// <returns></returns>
        public ReportDataSource GetDataSourceForReport(Dictionary<string, int> dataSource, int reportTemplateVersionID, string reportTemplateName, string reportTemplateLocation)
        {
            //ServiceResult result = new ServiceResult();
            //List<int> formInstanceIDList = new List<int>();
            //List<string> dataSourceNamesList = new List<string>();

            ReportDataSource datasource = new ReportDataSource();
            string fileName = _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                                        .Where(i => i.TemplateReportVersionID == reportTemplateVersionID)
                                                        .Select(i => i.TemplateFileName).FirstOrDefault();

            datasource.ReportName = fileName;
            datasource.Location = reportTemplateLocation;
            datasource.DataSources = new List<ReportSource>();
            foreach (var item in dataSource)
            {
                int FormInstanceID = item.Value;
                string DataSourceName = item.Key;

                //if(Int32.TryParse(formInstanceIDs.ToArray()[k], out FormInstanceID))
                //    DataSourceName = dataSourceNamess.ToArray()[k];
                //int FormInstanceID = Convert.ToInt32(formInstanceIDs.GetValue(k));//formInstanceIDList.ToArray()[k];                
                XmlDocument doc = new XmlDocument();

                //ReportDataSource src = new ReportDataSource();
                var instanceData = _unitOfWork.RepositoryAsync<FormInstanceDataMap>().Get().Where(i => i.FormInstanceID == FormInstanceID).FirstOrDefault();

                if (instanceData != null)
                {
                    doc = (XmlDocument)JsonConvert.DeserializeXmlNode(instanceData.FormData, "root");
                }
                datasource.DataSources.Add(new ReportSource { DataSourceName = DataSourceName, Xml = doc });
            }

            return datasource;
        }

        /// <summary>
        /// Updates the Properties of the specified Report Template Version.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public ServiceResult UpdateReportPropertiesParameters(List<ParameterViewModel> parameters, ReportPropertiesViewModel model)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();

                TemplateReport templateReport = (from reprt in this._unitOfWork.RepositoryAsync<TemplateReport>().Get()
                                                 where reprt.TemplateReportID == model.ReportDesignId
                                                 && reprt.IsActive == true
                                                 select reprt).FirstOrDefault();
                if (templateReport != null && templateReport.ReportType != model.ReportType)
                {
                    //templateReport.HelpText = model.HelpText;
                    //templateReport.ReportDescription = model.ReportDescription;
                    templateReport.ReportType = model.ReportType;
                    templateReport.UpdatedBy = UserName;
                    templateReport.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<TemplateReport>().Update(templateReport);
                    this._unitOfWork.Save();
                }

                //TemplateReportVersion reportVersion = (from reprt in this._unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                //                                       where reprt.TemplateReportID == model.ReportDesignId
                //                                       select reprt).FirstOrDefault();
                ////reportVersion.Location = model.location;
                //reportVersion.IsReleased = model.IsRelease;
                //reportVersion.IsVisible = model.Visible;
                //reportVersion.HelpText = model.HelpText;
                //this._unitOfWork.RepositoryAsync<TemplateReportVersion>().Update(reportVersion);
                //this._unitOfWork.Save();
                ////Location
                //TemplateReportLocation location = (from loc in this._unitOfWork.RepositoryAsync<TemplateReportLocation>().Get()
                //                                   where loc.TemplateReportVersionID == reportVersion.TemplateReportVersionID
                //                                   select loc).FirstOrDefault();
                //if (location.LocationName != model.location)
                //{
                //    location.LocationName = model.location;
                //    this._unitOfWork.RepositoryAsync<TemplateReportLocation>().Update(location);
                //    this._unitOfWork.Save();
                //}

                #region Save Parameters For Template Version
                foreach (var para in parameters)
                {
                    TemplateReportVersionParameter existingParameter = (from p1 in this._unitOfWork.RepositoryAsync<TemplateReportVersionParameter>().Get()
                                                                        where p1.ParameterTypeID == para.ParameterId && p1.TemplateReportVersionID == model.ReportVersionID
                                                                        select p1).FirstOrDefault();

                    if (existingParameter == null)
                    {
                        var insertParam = new TemplateReportVersionParameter();
                        insertParam.ParameterTypeID = para.ParameterId;
                        insertParam.TemplateReportVersionID = model.ReportVersionID;
                        insertParam.TenantID = 1;
                        insertParam.AddedBy = UserName;// "superuser";
                        insertParam.AddedDate = DateTime.Now;
                        insertParam.UpdatedBy = UserName;// "superuser";
                        insertParam.UpdatedDate = DateTime.Now;
                        insertParam.IsActive = true;
                        this._unitOfWork.RepositoryAsync<TemplateReportVersionParameter>().Insert(insertParam);
                        this._unitOfWork.Save();
                    }
                }
                #endregion

                //Return success result
                result.Result = ServiceResultStatus.Success;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                {
                    Messages = new string[] { "Parameters Saved successfully." }
                });
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

        /// <summary>
        /// Gets the Properties of the Sspecified Report Template Version.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="TemplateReportVersionId"></param>
        /// <returns></returns>
        public ReportPropertiesViewModel GetProperties(int tenantId, int TemplateReportVersionId)
        {
            var list1 = from reportVersion in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                        where reportVersion.TemplateReportVersionID == TemplateReportVersionId
                        select reportVersion;

            ReportPropertiesViewModel propertiesData = (from report in _unitOfWork.RepositoryAsync<TemplateReport>().Get()
                                                        join reportVersion in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                                        on report.TemplateReportID equals reportVersion.TemplateReportID
                                                        join reportLocation in _unitOfWork.RepositoryAsync<TemplateReportLocation>().Get()
                                                        on reportVersion.TemplateReportVersionID equals reportLocation.TemplateReportVersionID
                                                        where reportVersion.TemplateReportVersionID == TemplateReportVersionId
                                                        && report.IsActive == true
                                                        select new ReportPropertiesViewModel
                                                        {
                                                            PropertyID = report.TemplateReportID,
                                                            ReportDescription = reportVersion.Description,
                                                            HelpText = reportVersion.HelpText,
                                                            IsRelease = reportVersion.IsReleased,
                                                            Visible = reportVersion.IsVisible,
                                                            Location = reportLocation.LocationName,
                                                            ReportType = report.ReportType
                                                        }).FirstOrDefault();
            return propertiesData;
        }

        /// <summary>
        /// Gets the Roles defined for the specified Report Template Version.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="reportVersionId"></param>
        /// <returns></returns>
        public IEnumerable<RoleViewModel> GetRoleNames(int tenantId, int reportVersionId)
        {
            var roles = new List<UserRole>();
            List<RoleViewModel> list = new List<RoleViewModel>();
            try
            {
                list = (from role in this._unitOfWork.RepositoryAsync<UserRole>().Get()
                        join permsn in this._unitOfWork.RepositoryAsync<TemplateReportRoleAccessPermission>().Get().Where(r => r.TemplateReportVersionID == reportVersionId)
                               on role.RoleID equals permsn.RoleID into gj
                        from subpet in gj.DefaultIfEmpty()
                        select new RoleViewModel
                        {
                            RoleName = role.Name,
                            RoleID = role.RoleID,
                            IsVisible = (subpet == null ? false : true)
                        }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                //Get all the exception/inner exception messages and set the return code to failure               
            }
            return list;
        }

        /// <summary>
        /// Adds the Permission Roles for the specified Report Template Version.
        /// </summary>
        /// <param name="sectionaccessrows"></param>
        /// <returns></returns>
        public ServiceResult AddViewPermissionSet(List<RoleViewModel> sectionaccessrows)
        {
            ServiceResult res = new ServiceResult();
            List<RoleViewModel> toDeleteList = new List<RoleViewModel>();
            List<RoleViewModel> toInsertList = new List<RoleViewModel>();
            int versionId = sectionaccessrows.FirstOrDefault().TemplateReportVersionID;

            var roleList = (from role in this._unitOfWork.RepositoryAsync<UserRole>().Get()
                            join permsn in this._unitOfWork.RepositoryAsync<TemplateReportRoleAccessPermission>().Get().Where(r => r.TemplateReportVersionID == versionId)
                            on role.RoleID equals permsn.RoleID into gj
                            from subpet in gj.DefaultIfEmpty()
                            select new RoleViewModel
                            {
                                RoleName = role.Name,
                                RoleID = role.RoleID,
                                IsVisible = (subpet == null ? false : true),
                                TemplateReportVersionID = versionId
                            }).ToList();

            var listToRemove = roleList.Where(p => !sectionaccessrows.Any(p2 => p2.RoleID == p.RoleID));

            foreach (var val in roleList)
            {
                if (listToRemove.Any(r => r.RoleID == val.RoleID))
                {
                    val.IsVisible = false;
                }
                else
                    val.IsVisible = true;
            }

            var list = from role in this._unitOfWork.RepositoryAsync<TemplateReportRoleAccessPermission>().Get()
                       where role.TemplateReportVersion.TemplateReportVersionID == versionId
                       select role;

            foreach (var val in roleList)
            {
                if (val.IsVisible == true)
                {
                    var row = (from role in this._unitOfWork.RepositoryAsync<TemplateReportRoleAccessPermission>().Get()
                               where role.TemplateReportVersion.TemplateReportVersionID == versionId && role.RoleID == val.RoleID
                               select role).ToList();

                    if (row.Count() > 0)
                    {
                        // Do Nothing
                    }
                    else
                        toInsertList.Add(val);
                }
                else
                {
                    var row = (from role in this._unitOfWork.RepositoryAsync<TemplateReportRoleAccessPermission>().Get()
                               where role.TemplateReportVersion.TemplateReportVersionID == versionId && role.RoleID == val.RoleID
                               select role).ToList();
                    if (row.Count() > 0)
                    {
                        toDeleteList.Add(val);
                    }
                    else
                    {
                        // Do Nothing
                    }
                }
            }

            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
            {
                foreach (RoleViewModel deleteItem in toDeleteList)
                {
                    var deleteRow = (from role in this._unitOfWork.RepositoryAsync<TemplateReportRoleAccessPermission>().Get()
                                     where role.TemplateReportVersion.TemplateReportVersionID == versionId
                                            && role.RoleID == deleteItem.RoleID
                                     select role).FirstOrDefault();
                    this._unitOfWork.RepositoryAsync<TemplateReportRoleAccessPermission>().Delete(deleteRow);
                    this._unitOfWork.Save();
                }

                foreach (RoleViewModel insertItem in toInsertList)
                {
                    TemplateReportRoleAccessPermission newItem = new TemplateReportRoleAccessPermission();
                    newItem.RoleID = insertItem.RoleID;
                    newItem.TemplateReportVersionID = insertItem.TemplateReportVersionID;
                    newItem.TenantID = 1;
                    newItem.AddedBy = "superuser";
                    newItem.AddedDate = DateTime.Now;
                    newItem.UpdatedBy = "superuser";
                    newItem.UpdatedDate = DateTime.Now;
                    newItem.IsActive = true;
                    this._unitOfWork.RepositoryAsync<TemplateReportRoleAccessPermission>().Insert(newItem);
                    this._unitOfWork.Save();
                }
                scope.Complete();
                //Return success result
                res.Result = ServiceResultStatus.Success;
                ((IList<ServiceResultItem>)(res.Items)).Add(new ServiceResultItem()
                {
                    Messages = new string[] { "Role Access Permissions applied successfully." }
                });
            }
            return res;
        }

        public string GetTemplateNameById(int TemplateReportVersionID)
        {
            var filePath = (from tmpltReport in this._unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                            where tmpltReport.TemplateReportVersionID == TemplateReportVersionID
                            select tmpltReport.TemplateFileName).FirstOrDefault();
            return filePath;
        }

        public IEnumerable<ParameterViewModel> GetParameters(int reportVersionID)
        {
            List<ParameterViewModel> parameters = (from para in this._unitOfWork.RepositoryAsync<TemplateReportParameter>().Get()
                                                   join paraVrsn in
                                                       (from paraVrsn2 in _unitOfWork.RepositoryAsync<TemplateReportVersionParameter>().Get()
                                                        join rptVrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                                         on paraVrsn2.TemplateReportVersionID equals rptVrsn.TemplateReportVersionID
                                                        where rptVrsn.TemplateReportVersionID == reportVersionID
                                                        select paraVrsn2)
                                                    on para.ParameterTypeID equals paraVrsn.ParameterTypeID
                                                    into result
                                                   from res in result.DefaultIfEmpty()
                                                   select new ParameterViewModel
                                                   {
                                                       ParameterId = para.ParameterTypeID,
                                                       ParameterName = para.ParameterName,
                                                       IsSelected = (res != null)
                                                   }).ToList();
            return parameters;
        }

        public ServiceResult DeleteReportDesign(string user, int tenantId, int reportDesignId)
        {
            ServiceResult result = new ServiceResult();

            try
            {
                TemplateReport itemToUpdate = _unitOfWork.RepositoryAsync<TemplateReport>().FindById(reportDesignId);

                if (itemToUpdate != null)
                {
                    itemToUpdate.IsActive = false;
                    itemToUpdate.UpdatedBy = user;
                    itemToUpdate.UpdatedDate = DateTime.Now;
                    _unitOfWork.RepositoryAsync<TemplateReport>().Update(itemToUpdate);
                    _unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
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

        public ServiceResult FinalizeReportVersion(int tenantId, int reportVersionId, string comments)
        {
            ServiceResult result = new ServiceResult();

            try
            {

                TemplateReportVersion reportVersion = _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                                             .Where(i => i.TemplateReportVersionID == reportVersionId)
                                                             .Select(i => i).FirstOrDefault();

                if (reportVersion != null)
                {
                    reportVersion.IsReleased = true;
                    //reportVersion.Comments = comments;
                    reportVersion.TenantID = tenantId;
                    reportVersion.UpdatedBy = UserName;
                    reportVersion.UpdatedDate = DateTime.Now;

                    this._unitOfWork.RepositoryAsync<TemplateReportVersion>().Update(reportVersion);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
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

        public ServiceResult QueueCollateral(int accountID, string accountName, int folderID, string folderName, int folderVersionID, IEnumerable<string> formInstanceIDList, IEnumerable<string> folderVersionNumbers, IEnumerable<string> productIds, int reportTemplateID, string folderVersionEffDt, string username, DateTime? runDate, string collateralFolderPath, string reportName)
        {
            ServiceResult result = new ServiceResult();
            TemplateReportVersion SelectedrptVrsn = new TemplateReportVersion();
            int templateReportVersionID = 0;
            DateTime? templateReportVersionEffectiveDate = null;
            string collateralStorageLocation = string.Empty;
            try
            {
                DateTime? effDt = Convert.ToDateTime(folderVersionEffDt);

                int templateRptVersionID = 0, finalizedVersionsCnt = 0;
                bool? isFinalized = null;

                //Get number of finalized versions
                finalizedVersionsCnt = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                        where rptvrsn.TemplateReportID == reportTemplateID
                                             && rptvrsn.IsActive == true
                                             && rptvrsn.IsReleased == true
                                        select rptvrsn).Count();

                //Set a flag to true if finalized versions exists
                if (finalizedVersionsCnt > 0)
                    isFinalized = true;
                else
                    isFinalized = false;

                templateRptVersionID = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                        where rptvrsn.TemplateReportID == reportTemplateID
                                             //&& rptvrsn.EffectiveDate < effDt  -- Removed this condition as product was not getting queued as no version less than eff. dt was found
                                             && (rptvrsn.IsReleased == isFinalized || rptvrsn.IsReleased == null)
                                             && rptvrsn.IsActive == true
                                        orderby (rptvrsn.VersionNumber) descending
                                        select rptvrsn.TemplateReportVersionID).FirstOrDefault();

                TemplateReportFormDesignVersionMap selectedrptVrsnmap = (from rptDocMap in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                                                                         join rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                    on rptDocMap.TemplateReportVersionID equals rptvrsn.TemplateReportVersionID
                                                                         where rptvrsn.TemplateReportID == reportTemplateID
                                                                           //&& !string.IsNullOrEmpty(rptvrsn.TemplateFileName)
                                                                           // && !rptvrsn.TemplateReportFormDesignVersionMaps.Select(i => i.FormDesignID).ToList().Except(incomingFormDesignIDs).Any()
                                                                           && rptvrsn.TemplateReportVersionID == templateRptVersionID  // TemplateReportVersionID of report version selected as per folder version effective date
                                                                         orderby rptvrsn.EffectiveDate descending
                                                                         select rptDocMap).FirstOrDefault();

                IEnumerable<int> formInstanceIDs = _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                      .Where(i => formInstanceIDList.ToList().Contains(i.AnchorDocumentID.ToString()) && i.FormInstanceID != i.AnchorDocumentID
                                                      && i.FormDesignID == selectedrptVrsnmap.FormDesignID && i.FormDesignVersionID == selectedrptVrsnmap.FormDesignVersionID)
                                                      .Select(i => i.FormInstanceID).ToList();

                var incomingFormDesignIDs = _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                     .Where(i => formInstanceIDs.ToList().Contains(i.FormInstanceID))
                                                     .Select(i => i.FormDesignID);


                SelectedrptVrsn = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                   join rptDocMap in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                                    on rptvrsn.TemplateReportVersionID equals rptDocMap.TemplateReportVersionID
                                   where rptvrsn.TemplateReportID == reportTemplateID
                                     //&& !string.IsNullOrEmpty(rptvrsn.TemplateFileName)
                                     // && !rptvrsn.TemplateReportFormDesignVersionMaps.Select(i => i.FormDesignID).ToList().Except(incomingFormDesignIDs).Any()
                                     && rptvrsn.TemplateReportVersionID == templateRptVersionID  // TemplateReportVersionID of report version selected as per folder version effective date
                                   orderby rptvrsn.EffectiveDate descending
                                   select rptvrsn).FirstOrDefault();

                if (SelectedrptVrsn != null)
                {
                    templateReportVersionID = SelectedrptVrsn.TemplateReportVersionID;
                    templateReportVersionEffectiveDate = SelectedrptVrsn.EffectiveDate;
                    collateralStorageLocation = SelectedrptVrsn.Location;

                    DataTable dataTable = GetDataTable(formInstanceIDs, folderVersionNumbers, productIds);
                    SqlParameter collateralQueueType = new SqlParameter("@CollateralQueueType", dataTable);
                    collateralQueueType.TypeName = "[dbo].[CollateralQueueType]";

                    var folder = (from inst in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                  join fldver in _unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                 on inst.FolderVersionID equals fldver.FolderVersionID
                                  join fld in _unitOfWork.RepositoryAsync<Folder>().Get()
                                  on fldver.FolderID equals fld.FolderID
                                  where formInstanceIDs.Contains(inst.FormInstanceID)
                                  select fld
                                 ).FirstOrDefault();

                    if(folder!=null)
                    {
                        folderName = folder.Name;
                    }

                    object runDateValue;
                    if (runDate == null)
                        runDateValue = DBNull.Value;
                    else
                        runDateValue = runDate;

                    List<SqlParameter> lstParam = new List<SqlParameter> { new SqlParameter("@AccountID", accountID),
                                                                       new SqlParameter("@AccountName" , accountName),
                                                                       new SqlParameter("@FolderID",folderID ),
                                                                       new SqlParameter("@FolderName", folderName ),
                                                                       new SqlParameter("@FolderVersionID", folderVersionID ),
                                                                       new SqlParameter("@FolderVersionEffectiveDate", folderVersionEffDt ),
                                                                       new SqlParameter("@TemplateReportID", reportTemplateID ),
                                                                       new SqlParameter("@TemplateReportVersionID", templateReportVersionID ),
                                                                       new SqlParameter("@TemplateReportVersionEffectiveDate", templateReportVersionEffectiveDate ),
                                                                       new SqlParameter("@CollateralStorageLocation", collateralStorageLocation ),
                                                                       new SqlParameter("@CreatedBy",username ),
                                                                       new SqlParameter("@RunDate",  runDateValue),
                                                                       new SqlParameter("@CollateralFolderPath", collateralFolderPath),
                                                                       collateralQueueType
                                                                     };
                    var res = this._unitOfWork.Repository<TemplateReportVersion>().ExecuteUpdateSql("EXEC [Setup].[usp_EnqueueCollaterals] @AccountID,@AccountName,@FolderID,@FolderName,@FolderVersionID,@FolderVersionEffectiveDate,@TemplateReportID,@TemplateReportVersionID,@TemplateReportVersionEffectiveDate,@CollateralStorageLocation,@CreatedBy,@RunDate, @CollateralFolderPath, @CollateralQueueType", lstParam.ToArray());
                    result.Result = ServiceResultStatus.Success;
                    string description = "Product(s) has been queued against template " + reportName + " of version " + SelectedrptVrsn.VersionNumber;
                    this.SaveTemplateActivityLog(Convert.ToInt32(templateReportVersionID), description, username);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return result;
        }

        private DataTable GetDataTable(IEnumerable<int> formInstanceIDs, IEnumerable<string> folderVersionNumbers, IEnumerable<string> productIds)
        {
            DataTable dt = new DataTable("CollateralQueueType");
            //dt.colummns 
            dt.Columns.Add("ProductID", typeof(string));
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("FolderVersionNumber", typeof(string));
            dt.Columns.Add("FormInstanceID", typeof(int));
            dt.Columns.Add("FormInstanceName", typeof(string));
            dt.Columns.Add("FormDesignID", typeof(int));
            dt.Columns.Add("FormDesignVersionID", typeof(int));

            int numberOfRows = formInstanceIDs.Count();
            for (int index = 0; index < numberOfRows; index++)
            {
                DataRow row = dt.NewRow();
                //string productName = productIds.ElementAt(index);
                //if (productName.Length > 8)
                //    row["ProductID"] = productIds.ElementAt(index).Substring(0, 8); //Take first 8 chars only
                //else
                row["ProductID"] = productIds.ElementAt(index);
                row["ProductName"] = "";
                row["FolderVersionNumber"] = folderVersionNumbers.ElementAt(index);
                row["FormInstanceID"] = formInstanceIDs.ElementAt(index);
                row["FormInstanceName"] = "";
                row["FormDesignID"] = 0;
                row["FormDesignVersionID"] = 0;
                dt.Rows.Add(row);
            }
            return dt;
        }

        public void SaveTemplateActivityLog(int templVerID, string description, string username)
        {
            TemplateReportActivityLog activitylog = new TemplateReportActivityLog();
            activitylog.TemplateReportVersionID = templVerID;
            activitylog.Description = description;
            activitylog.AddedBy = username;
            activitylog.AddedDate = DateTime.Now;

            this._unitOfWork.RepositoryAsync<TemplateReportActivityLog>().Insert(activitylog);
            this._unitOfWork.Save();
        }

        public GridPagingResponse<ReportUserActivityViewModel> GetReportUserActivity(GridPagingRequest gridPagingRequest)
        {
            List<ReportUserActivityViewModel> data = null;
            int count = 0;
            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                data = (from report in _unitOfWork.RepositoryAsync<TemplateReport>().Get()
                        join reportversion in _unitOfWork.Repository<TemplateReportVersion>().Get()
                        on report.TemplateReportID equals reportversion.TemplateReportID
                        join useractivity in _unitOfWork.RepositoryAsync<TemplateReportActivityLog>().Get()
                        on reportversion.TemplateReportVersionID equals useractivity.TemplateReportVersionID
                        select new ReportUserActivityViewModel
                        {
                            ReportName = report.TemplateReportName,
                            ReportVersion = reportversion.VersionNumber,
                            Description = useractivity.Description,
                            UserName = useractivity.AddedBy,
                            Date = useractivity.AddedDate,
                        }).ApplySearchCriteria(criteria)
                          .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                          .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return new GridPagingResponse<ReportUserActivityViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, data);
        }

        public bool IsReportTemplateVersionAccessiable(int reportTemplateID, string folderVersionEffDt, string currentUserName)
        {
            int userRoleID = 0, templateRptVersionID = 0;
            DateTime? effDt = Convert.ToDateTime(folderVersionEffDt);

            int templateRptVersionCnt = 0, finalizedVersionsCnt = 0;
            bool? isFinalized = null;

            try
            {
                userRoleID = (from userMap in this._unitOfWork.RepositoryAsync<User>().Get()
                              join userRoleMap in this._unitOfWork.RepositoryAsync<UserRoleAssoc>().Get()
                              on userMap.UserID equals userRoleMap.UserId
                              join role in this._unitOfWork.RepositoryAsync<UserRole>().Get()
                              on userRoleMap.RoleId equals role.RoleID
                              where userMap.UserName == currentUserName
                              select role.RoleID).FirstOrDefault();

                //Get number of finalized versions
                finalizedVersionsCnt = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                        where rptvrsn.TemplateReportID == reportTemplateID
                                             && rptvrsn.IsActive == true
                                             && rptvrsn.IsReleased == true
                                        select rptvrsn).Count();

                //Set a flag to true if finalized versions exists
                if (finalizedVersionsCnt > 0)
                    isFinalized = true;

                //get templateRptVersionID of report version to picked up
                templateRptVersionID = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                        where rptvrsn.TemplateReportID == reportTemplateID
                                             && rptvrsn.EffectiveDate < effDt
                                             && rptvrsn.IsReleased == isFinalized
                                             && rptvrsn.IsActive == true
                                        orderby rptvrsn.VersionNumber descending
                                        select rptvrsn.TemplateReportVersionID).FirstOrDefault();

                //Check whether template Report Version exists to which current user has access. 
                templateRptVersionCnt = (from permsn in this._unitOfWork.RepositoryAsync<TemplateReportRoleAccessPermission>().Get()
                                         where permsn.TemplateReportVersionID == templateRptVersionID
                                              && permsn.IsActive == true
                                              && permsn.RoleID == userRoleID
                                         select permsn.TemplateReportVersionID).Count();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return templateRptVersionCnt > 0 ? true : false;
        }

        public bool IsTemplateUploaded(int reportTemplateVersionId)
        {
            string location = string.Empty;
            bool isTemplateUploaded = true;
            try
            {
                location = this._unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                           .Where(t => t.TemplateReportVersionID == reportTemplateVersionId).Select(t => t.Location).FirstOrDefault();

                if (string.IsNullOrEmpty(location))
                    isTemplateUploaded = false;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return isTemplateUploaded;
        }

        public List<string> GetGenerateReportInputs(int formInstanceID, int reportTemplateID, int formDesignVersionID, IEnumerable<string> formInstanceIDs, string folderVersionEffDt)
        {
            List<string> lst = new List<string>();
            string location = string.Empty;
            try
            {
                Dictionary<string, int> ReportData = new Dictionary<string, int>();
                TemplateReportVersion SelectedrptVrsn = new TemplateReportVersion();
                int templateRptVersionID = 0, finalizedVersionsCnt = 0;
                DateTime? effDt = Convert.ToDateTime(folderVersionEffDt);

                bool? isFinalized = null;
                //Get number of finalized versions
                finalizedVersionsCnt = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                        where rptvrsn.TemplateReportID == reportTemplateID
                                             && rptvrsn.IsActive == true
                                             && rptvrsn.IsReleased == true
                                        select rptvrsn).Count();


                //Set a flag to true if finalized versions exists
                if (finalizedVersionsCnt > 0)
                    isFinalized = true;

                templateRptVersionID = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                        where rptvrsn.TemplateReportID == reportTemplateID
                                             && rptvrsn.EffectiveDate <= effDt
                                             && rptvrsn.IsReleased == isFinalized
                                             && rptvrsn.IsActive == true
                                        orderby (rptvrsn.VersionNumber) descending
                                        select rptvrsn.TemplateReportVersionID).FirstOrDefault();

                if (templateRptVersionID > 0)
                {
                    TemplateReportFormDesignVersionMap rptVrsnmap = (from rptDocMap in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get() //_unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(i => i.FolderVersionID == FolderVersionID).SelectMany(s => _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get().Where(k => k.TemplateReportID == ReportTemplateID && k.EffectiveDate >= s.EffectiveDate));
                                                                     join rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                                                     on rptDocMap.TemplateReportVersionID equals rptvrsn.TemplateReportVersionID
                                                                     where rptvrsn.TemplateReportID == reportTemplateID
                                                                       && !string.IsNullOrEmpty(rptvrsn.TemplateFileName)
                                                                       //&& !rptvrsn.TemplateReportFormDesignVersionMaps.Select(i => i.FormDesignID).ToList().Except(incomingFormDesignIDs).Any()
                                                                       && rptvrsn.TemplateReportVersionID == templateRptVersionID  // TemplateReportVersionID of report version selected as per folder version effective date
                                                                     orderby rptvrsn.EffectiveDate descending
                                                                     select rptDocMap).FirstOrDefault();

                    IEnumerable<int> formInstanceIDList = _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                          .Where(i => formInstanceIDs.ToList().Contains(i.AnchorDocumentID.ToString()) && i.FormInstanceID != i.AnchorDocumentID
                                                          && rptVrsnmap.FormDesignID == i.FormDesignID && rptVrsnmap.FormDesignVersionID == i.FormDesignVersionID)
                                                          .Select(i => i.FormInstanceID).ToList();

                    var incomingFormDesignIDs = _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                          .Where(i => formInstanceIDList.ToList().Contains(i.FormInstanceID))
                                                          .Select(i => i.FormDesignID);

                    SelectedrptVrsn = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get() //_unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(i => i.FolderVersionID == FolderVersionID).SelectMany(s => _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get().Where(k => k.TemplateReportID == ReportTemplateID && k.EffectiveDate >= s.EffectiveDate));
                                       join rptDocMap in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                                        on rptvrsn.TemplateReportVersionID equals rptDocMap.TemplateReportVersionID
                                       where rptvrsn.TemplateReportID == reportTemplateID
                                         && !string.IsNullOrEmpty(rptvrsn.TemplateFileName)
                                         && !rptvrsn.TemplateReportFormDesignVersionMaps.Select(i => i.FormDesignID).ToList().Except(incomingFormDesignIDs).Any()
                                         && rptvrsn.TemplateReportVersionID == templateRptVersionID  // TemplateReportVersionID of report version selected as per folder version effective date
                                       orderby rptvrsn.EffectiveDate descending
                                       select rptvrsn).FirstOrDefault();

                    if (SelectedrptVrsn != null)
                    {
                        if (string.IsNullOrEmpty(SelectedrptVrsn.Location))
                            location = string.Empty;
                        else
                            location = SelectedrptVrsn.Location;

                        formInstanceID = formInstanceIDList.FirstOrDefault();
                    }
                    else
                        location = string.Empty;


                    FormInstanceDataMap formInstanceDataMap = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                                        .Query()
                                                                                        .Filter(c => c.FormInstanceID == formInstanceID)
                                                                                        .Get().FirstOrDefault();

                    FormDesignVersion vrsn = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                                            .Query()
                                                                                            .Filter(c => c.FormDesignVersionID == formDesignVersionID)
                                                                                            .Get().FirstOrDefault();

                    lst.Add(location);
                    lst.Add(formInstanceDataMap.FormData);
                    lst.Add(vrsn.FormDesignVersionData);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return lst;
        }

        public FormInstanceViewModel GetFormInstanceIdForView(int formInstanceID, int reportTemplateID, int formDesignVersionID, IEnumerable<string> formInstanceIDs, string folderVersionEffDt)
        {

            FormInstanceViewModel viewDetails = new FormInstanceViewModel();

            List<string> lst = new List<string>();
            string location = string.Empty;
            try
            {
                Dictionary<string, int> ReportData = new Dictionary<string, int>();
                TemplateReportVersion SelectedrptVrsn = new TemplateReportVersion();
                int templateRptVersionID = 0, finalizedVersionsCnt = 0;
                DateTime? effDt = Convert.ToDateTime(folderVersionEffDt);

                bool? isFinalized = null;
                //Get number of finalized versions
                finalizedVersionsCnt = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                        where rptvrsn.TemplateReportID == reportTemplateID
                                             && rptvrsn.IsActive == true
                                             && rptvrsn.IsReleased == true
                                        select rptvrsn).Count();


                //Set a flag to true if finalized versions exists
                if (finalizedVersionsCnt > 0)
                    isFinalized = true;

                templateRptVersionID = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                        where rptvrsn.TemplateReportID == reportTemplateID
                                             && rptvrsn.EffectiveDate <= effDt
                                             && rptvrsn.IsReleased == isFinalized
                                             && rptvrsn.IsActive == true
                                        orderby (rptvrsn.VersionNumber) descending
                                        select rptvrsn.TemplateReportVersionID).FirstOrDefault();

                if (templateRptVersionID > 0)
                {
                    TemplateReportFormDesignVersionMap rptVrsnmap = (from rptDocMap in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get() //_unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(i => i.FolderVersionID == FolderVersionID).SelectMany(s => _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get().Where(k => k.TemplateReportID == ReportTemplateID && k.EffectiveDate >= s.EffectiveDate));
                                                                     join rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                                                     on rptDocMap.TemplateReportVersionID equals rptvrsn.TemplateReportVersionID
                                                                     where rptvrsn.TemplateReportID == reportTemplateID
                                                                       && !string.IsNullOrEmpty(rptvrsn.TemplateFileName)
                                                                       //&& !rptvrsn.TemplateReportFormDesignVersionMaps.Select(i => i.FormDesignID).ToList().Except(incomingFormDesignIDs).Any()
                                                                       && rptvrsn.TemplateReportVersionID == templateRptVersionID  // TemplateReportVersionID of report version selected as per folder version effective date
                                                                     orderby rptvrsn.EffectiveDate descending
                                                                     select rptDocMap).FirstOrDefault();

                    IEnumerable<FormInstanceViewModel> formInstanceIDList = (from instance in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                                            .Where(i => formInstanceIDs.ToList().Contains(i.AnchorDocumentID.ToString()) && i.FormInstanceID != i.AnchorDocumentID
                                                                            && rptVrsnmap.FormDesignID == i.FormDesignID && rptVrsnmap.FormDesignVersionID == i.FormDesignVersionID)
                                                                             select new FormInstanceViewModel
                                                                             {
                                                                                 FormInstanceID = instance.FormInstanceID,
                                                                                 FormDesignVersionID = instance.FormDesignVersionID
                                                                             }).ToList();


                    viewDetails = formInstanceIDList.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return viewDetails;
        }
        private IList<FormInstanceComplianceValidationlog> GetPrintXManualUpoadedlog()
        {
            IList<FormInstanceComplianceValidationlog> log = new List<FormInstanceComplianceValidationlog>();
            var result = _unitOfWork.Repository<FormInstanceComplianceValidationlog>().Get().Where(m => m.ComplianceType == "pdfx").ToList();
                                                                    

            if (result!= null)
            {
                return result.ToList();
            }


            return log;
        }
        private bool IsPrintXManualUploaded(int collateralQueueId, IList<FormInstanceComplianceValidationlog> manaulPrintxList)
        {
            var exists = manaulPrintxList.Where(m => m.CollateralProcessQueue1Up == collateralQueueId).ToList();
            if (exists != null)
            {
                if (exists.Count() > 0)
                    return true;
            }
            return false;
        }
        public GridPagingResponse<QueuedReportModel> GetQueuedCollateralsList(GridPagingRequest gridPagingRequest)
        {
            List<QueuedReportModel> lst = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);



                lst = (from queue in _unitOfWork.RepositoryAsync<CollateralProcessQueue>().Get()
                       join report in _unitOfWork.RepositoryAsync<TemplateReport>().Get()
                       on queue.TemplateReportID equals report.TemplateReportID
                       join statusMaster in _unitOfWork.RepositoryAsync<ProcessStatusMaster>().Get()
                       on queue.ProcessStatus1Up equals statusMaster.ProcessStatus1Up
                       select new QueuedReportModel
                       {
                           CollateralProcessQueue1Up = queue.CollateralProcessQueue1Up,
                           ProductID = queue.ProductID,
                           AccountID = queue.AccountID ?? 0,
                           AccountName = queue.AccountName,
                           FolderName = queue.FolderName,
                           FolderVersionNumber = queue.FolderVersionNumber,
                           FormInstanceName = queue.FormInstanceName,
                           CollateralName = report.TemplateReportName,
                           Status = statusMaster.ProcessStatusName,
                           FormInstanceID = queue.FormInstanceID ?? 0,
                           TemplateReportVersionID = queue.TemplateReportVersionID ?? 0,
                           ProcessedDate = (queue.UpdatedDate != null) ? queue.UpdatedDate.Value : queue.CreatedDate.Value,
                       }).ApplySearchCriteria(criteria)
                         .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                         .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);

                IList<FormInstanceComplianceValidationlog> uploadPrintx = GetPrintXManualUpoadedlog();
                foreach (var queue in lst)
                {                   
                    queue.PrintXManallyUploaded = IsPrintXManualUploaded(queue.CollateralProcessQueue1Up, uploadPrintx);
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return new GridPagingResponse<QueuedReportModel>(gridPagingRequest.page, count, gridPagingRequest.rows, lst);
        }

        public string GetDateFormat(CollateralProcessQueue objQue)
        {
            string strFormatedDate = string.Empty;
            DateTime? objDatetime = (objQue.UpdatedDate != null) ? objQue.UpdatedDate : objQue.CreatedDate;
            strFormatedDate = Convert.ToDateTime(objDatetime.ToString()).ToString("dd/mm/yyyy");
            return strFormatedDate;
        }

        public string GetFilePath(int processQueue1Up, string fileFormat)
        {
            string filePath = string.Empty;
            try
            {
                var queuedProduct = (from queue in _unitOfWork.RepositoryAsync<CollateralProcessQueue>().Get()
                                     where queue.CollateralProcessQueue1Up == processQueue1Up
                                     select queue).FirstOrDefault();
                if (queuedProduct != null)
                {
                    if (fileFormat == "pdf")
                        filePath = queuedProduct.FilePath + "|" + queuedProduct.ProductID;
                    else if (fileFormat == "word")
                        filePath = queuedProduct.WordFilePath + "|" + queuedProduct.ProductID;
                    else if (fileFormat == "pdfx")
                        filePath = queuedProduct.PrintxFilePath + "|" + queuedProduct.ProductID;
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return filePath;
        }

        public QueuedReportModel IsReportAlreadyGenerated(int formInstanceId, int reportTemplateId)
        {
            QueuedReportModel queuedProduct = null;
            try
            {
                int formId = (from r in _unitOfWork.RepositoryAsync<TemplateReport>().Get()
                              join rptv in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                              on r.TemplateReportID equals rptv.TemplateReportID
                              join rptm in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                              on rptv.TemplateReportVersionID equals rptm.TemplateReportVersionID
                              where r.TemplateReportID == reportTemplateId
                              && rptv.IsReleased == true
                              orderby rptv.EffectiveDate descending
                              select rptm.FormDesignID).FirstOrDefault();

                if (formId == 0 || formId == null)
                {
                    formId = (from r in _unitOfWork.RepositoryAsync<TemplateReport>().Get()
                              join rptv in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                              on r.TemplateReportID equals rptv.TemplateReportID
                              join rptm in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                              on rptv.TemplateReportVersionID equals rptm.TemplateReportVersionID
                              where r.TemplateReportID == reportTemplateId
                              orderby rptv.EffectiveDate descending
                              select rptm.FormDesignID).FirstOrDefault();
                }

                int forminstanceid = (from f in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                      where f.AnchorDocumentID == formInstanceId
                                      && f.AnchorDocumentID != f.FormInstanceID
                                      && f.FormDesignID == formId
                                      select f.FormInstanceID).FirstOrDefault();

                queuedProduct = (from q in _unitOfWork.RepositoryAsync<CollateralProcessQueue>().Get()
                                 where q.FormInstanceID == forminstanceid && (q.ProcessStatus1Up == 4 || q.ProcessStatus1Up == 1)
                                 && q.TemplateReportID == reportTemplateId
                                 orderby q.CollateralProcessQueue1Up descending
                                 select new QueuedReportModel
                                 {
                                     CollateralProcessQueue1Up = q.CollateralProcessQueue1Up,
                                     CreatedDate = q.CreatedDate,
                                     FormInstanceID = q.FormInstanceID ?? 0
                                 }).FirstOrDefault();

                if (queuedProduct != null)
                    queuedProduct.ProcessedDate = queuedProduct.CreatedDate.Value;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return queuedProduct;
        }


        public IEnumerable<ReportDesignViewModel> GetCollateralTemplatesForProduct(int formInstanceId, string reportLocation)
        {
            IEnumerable<ReportDesignViewModel> data = null;

            try
            {
                DateTime effectivedate = (from fldrve in _unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                          join forin in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                             on fldrve.FolderVersionID equals forin.FolderVersionID
                                          where forin.FormInstanceID == formInstanceId
                                          select fldrve.EffectiveDate
                                              ).FirstOrDefault();

                List<int> formdesignid = (from fldrve in _unitOfWork.RepositoryAsync<FormDesignMapping>().Get()
                                          join forin in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                             on fldrve.AnchorDesignID equals forin.FormDesignID
                                          where forin.FormInstanceID == formInstanceId
                                          select fldrve.TargetDesignID
                                              ).ToList();

                data = (from tmp in _unitOfWork.RepositoryAsync<TemplateReport>().Get()
                        join tmpve in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                        on tmp.TemplateReportID equals tmpve.TemplateReportID
                        join tmpl in _unitOfWork.RepositoryAsync<TemplateReportLocation>().Get()
                        on tmpve.TemplateReportVersionID equals tmpl.TemplateReportVersionID
                        join tmpm in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                        on tmpve.TemplateReportVersionID equals tmpm.TemplateReportVersionID
                        where (tmpl.LocationName == reportLocation || tmpl.LocationName == "All")
                            && tmp.IsActive == true
                            && tmpve.EffectiveDate <= effectivedate
                            && formdesignid.Contains(tmpm.FormDesignID)
                        select new ReportDesignViewModel
                        {
                            ReportId = tmp.TemplateReportID,
                            ReportName = tmp.TemplateReportName
                        }).Distinct().ToList();



                //data = (from formins in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                //        join des in _unitOfWork.RepositoryAsync<FormDesignMapping>().Get()
                //            on formins.FormDesignID equals des.AnchorDesignID                        
                //        join reportFormDesign in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                //        on des.TargetDesignID equals reportFormDesign.FormDesignID
                //        join rptLoc in _unitOfWork.RepositoryAsync<TemplateReportLocation>().Get()
                //        on reportFormDesign.TemplateReportVersionID equals rptLoc.TemplateReportVersionID
                //        join tmpReport in _unitOfWork.RepositoryAsync<TemplateReport>().Get()
                //        on reportFormDesign.TemplateReportID equals tmpReport.TemplateReportID
                //        where formins.FormInstanceID == formInstanceId
                //            && formins.FormDesignVersionID == reportFormDesign.FormDesignVersionID
                //            && rptLoc.LocationName == reportLocation
                //            && tmpReport.IsActive == true
                //        // && formDesignIDs.Contains(formins.FormDesignID) // To get only those products whoes FormDesignID exists in TemplateReportFormDesignVersionMap
                //        //&& (anchorDesignIDs.Contains(formins.FormDesignID)) // To get only those products whoes FormDesignID is used as Anchor product in report designs
                //        select new ReportDesignViewModel
                //        {
                //            ReportId = tmpReport.TemplateReportID,
                //            ReportName = tmpReport.TemplateReportName
                //        }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return data;
        }

        public GridPagingResponse<ReportDocumentModel> GetAccountAndFolders(int ReportTemplateID, int AccountID, int FolderID, int FolderVersionID, string reportName, string planFamilyName, GridPagingRequest gridPagingRequest, bool shouldSelectFormInstance)
        {
            int count = 0;
            SearchCriteria criteria = new SearchCriteria();
            criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

            string[] templateList = { };
            bool isCombinedECOReportName = false;
            if (ConfigurationManager.AppSettings["CombinedEOCTemplateList"] != null && ConfigurationManager.AppSettings["CombinedEOCTemplateList"] != "")
            {
                templateList = ConfigurationManager.AppSettings["CombinedEOCTemplateList"].Split(',');
                if (templateList.Contains(reportName))
                {
                    isCombinedECOReportName = true;
                }
            }

            List<ReportDocumentModel> result = null;
            IEnumerable<int> formDesignIDs = null;
            IEnumerable<int> anchorDesignIDs = null;
            try
            {
                formDesignIDs = (from formDesignVersionMap in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                                 where formDesignVersionMap.IsActive == true
                                 && formDesignVersionMap.TemplateReportID == ReportTemplateID
                                 select (formDesignVersionMap.FormDesignID)).Distinct().ToList();

                // Get anchorDesignIDs from formDesignIDs
                anchorDesignIDs = (from desMapping in _unitOfWork.RepositoryAsync<FormDesignMapping>().Get()
                                   where formDesignIDs.Contains(desMapping.TargetDesignID)
                                   select desMapping.AnchorDesignID).Distinct().ToList();

                if (isCombinedECOReportName)
                {
                    result = getReportModel(planFamilyName, anchorDesignIDs, shouldSelectFormInstance, gridPagingRequest, criteria, count);
                }
                else
                {
                    //AD- 24 May 2017 -> To fetch both account & portfolio, changed query to left join on Account
                    result = (from fldr in _unitOfWork.RepositoryAsync<Folder>().Get()
                              join fldrvrsn in _unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                  on fldr.FolderID equals fldrvrsn.FolderID
                              join formins in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                  on fldrvrsn.FolderVersionID equals formins.FolderVersionID
                              join des in _unitOfWork.RepositoryAsync<FormDesign>().Get()
                                  on formins.FormDesignID equals des.FormID
                              join desVer in _unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                              on des.FormID equals desVer.FormDesignID
                              join desginMapping in _unitOfWork.RepositoryAsync<FormDesignGroupMapping>().Get()
                                  on formins.FormDesignID equals desginMapping.FormID
                                  into tmp
                              from desginMapping in tmp.DefaultIfEmpty()
                              join accFldrmap in _unitOfWork.RepositoryAsync<AccountFolderMap>().Get()
                                  on fldr.FolderID equals accFldrmap.FolderID
                                  into tempAccMap
                              from accFldrmap in tempAccMap.DefaultIfEmpty()
                              join acc in _unitOfWork.RepositoryAsync<Account>().Get()
                                  on accFldrmap.AccountID equals acc.AccountID
                                  into tmpAcc
                              from acc in tmpAcc.DefaultIfEmpty()
                              where fldrvrsn.IsActive && formins.IsActive && des.IsActive
                                    && des.IsMasterList == false
                                    && desVer.FormDesignVersionID == formins.FormDesignVersionID
                                  //&& (acc.AccountID == null || AccountID == 0 || acc.AccountID == AccountID)
                                  //&& (FolderID == 0 || fldr.FolderID == FolderID)
                                  //&& (FolderVersionID == 0 || fldrvrsn.FolderVersionID == FolderVersionID)
                                  && (anchorDesignIDs.Contains(formins.FormDesignID)) // To get only those products whoes FormDesignID is used as Anchor product in report designs
                              select new ReportDocumentModel
                              {
                                  AccountID = acc.AccountID,
                                  AccountName = acc.AccountName ?? "",
                                  FolderID = fldr.FolderID,
                                  FolderName = fldr.Name,
                                  FolderVersionID = fldrvrsn.FolderVersionID,
                                  FolderVersionEffectiveDate = fldrvrsn.EffectiveDate,
                                  FolderVersionNumber = fldrvrsn.FolderVersionNumber,
                                  FormInstanceID = shouldSelectFormInstance == true ? formins.FormInstanceID : 0,
                                  FormName = shouldSelectFormInstance == true ? formins.Name : "",
                                  AnchorDocumentName = shouldSelectFormInstance == true ? des.FormName : "",
                                  FormDesignVersionID = desVer.FormDesignVersionID,
                                  FormDesignID = (int)desVer.FormDesignID,
                              }).Distinct().ApplySearchCriteria(criteria)
                                .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                                .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
                }
            }

            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return new GridPagingResponse<ReportDocumentModel>(gridPagingRequest.page, count, gridPagingRequest.rows, result);
        }

        public GridPagingResponse<QueuedReportModel> ViewCollateralsAtFolder(int formInstanceID, GridPagingRequest gridPagingRequest)
        {
            int count = 0;
            SearchCriteria criteria = new SearchCriteria();
            criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
            List<QueuedReportModel> lst = null;
            List<int> formInstanceIdLst = new List<int>();

            try
            {
                formInstanceIdLst = (from fi in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                     where fi.AnchorDocumentID == formInstanceID
                                     select fi.FormInstanceID
                                     ).ToList();


                lst = (from queue in _unitOfWork.RepositoryAsync<CollateralProcessQueue>().Get()
                       join report in _unitOfWork.RepositoryAsync<TemplateReport>().Get()
                       on queue.TemplateReportID equals report.TemplateReportID
                       join statusMaster in _unitOfWork.RepositoryAsync<ProcessStatusMaster>().Get()
                       on queue.ProcessStatus1Up equals statusMaster.ProcessStatus1Up
                       where formInstanceIdLst.Contains(queue.FormInstanceID ?? 0)
                       select new QueuedReportModel
                       {
                           CollateralProcessQueue1Up = queue.CollateralProcessQueue1Up,
                           ProductID = queue.ProductID,
                           AccountID = queue.AccountID ?? 0,
                           AccountName = queue.AccountName==string.Empty?"": queue.AccountName,
                           FolderName = queue.FolderName,
                           FolderVersionNumber = queue.FolderVersionNumber,
                           FormInstanceName = queue.FormInstanceName,
                           CollateralName = report.TemplateReportName,
                           Status = statusMaster.ProcessStatusName,
                           FormInstanceID = queue.FormInstanceID ?? 0,
                           TemplateReportVersionID = queue.TemplateReportVersionID ?? 0,
                           ProcessedDate = (queue.UpdatedDate != null) ? queue.UpdatedDate.Value : queue.CreatedDate.Value
                       }).ApplySearchCriteria(criteria)
                         .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                         .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return new GridPagingResponse<QueuedReportModel>(gridPagingRequest.page, count, gridPagingRequest.rows, lst);
        }

        public bool CheckIfCollateralIsOfSBDesignType(int templateReportID)
        {
            bool isSBDesignProduct = false;
            try
            {
                isSBDesignProduct = (from verMap in _unitOfWork.Repository<TemplateReportFormDesignVersionMap>().Get()
                                     join des in _unitOfWork.Repository<FormDesign>().Get()
                                     on verMap.FormDesignID equals des.FormID
                                     where verMap.TemplateReportID == templateReportID
                                     && des.FormName == "SBDesign"
                                     select verMap
                                     ).ToList().Any();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return isSBDesignProduct;
        }

        public bool CheckIfCollateralsOfEOCDesignType(int templateReportID) {
            bool isEOCDesignProduct = false;

            string[] templateList = { };

            if (ConfigurationManager.AppSettings["CombinedEOCTemplateList"] != null && ConfigurationManager.AppSettings["CombinedEOCTemplateList"] != "")
            {
                templateList = ConfigurationManager.AppSettings["CombinedEOCTemplateList"].Split(',');
            }

            try
            {
                isEOCDesignProduct = (from tmplt in _unitOfWork.Repository<TemplateReport>().Get()
                                     join verMap in _unitOfWork.Repository<TemplateReportFormDesignVersionMap>().Get()
                                     on tmplt.TemplateReportID equals verMap.TemplateReportID
                                     join des in _unitOfWork.Repository<FormDesign>().Get()
                                     on verMap.FormDesignID equals des.FormID
                                     where verMap.TemplateReportID == templateReportID
                                     && des.FormName == "MedicareANOC/EOCDesign"
                                     && templateList.Contains(tmplt.TemplateReportName)
                                     select verMap
                                     ).ToList().Any();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return isEOCDesignProduct;
        }

        public void UpdateLayoutTypeMedicate(string formInstanceIDs, int count)
        {
            try
            {
                string layoutTypePath = "PlanInformation";
                string fieldName = "LayoutType";
                string[] formInstanceIDArr = formInstanceIDs.Split(',');
                string dropdownValue = string.Empty;

                switch (count)
                {
                    case 1:
                        dropdownValue = "One";
                        break;
                    case 2:
                        dropdownValue = "Two";
                        break;
                    case 3:
                        dropdownValue = "Three";
                        break;
                }

                foreach (string instanceId in formInstanceIDArr.Where(c => c.Length > 0))
                {
                    int formInstanceId = int.Parse(instanceId);
                    FormInstance formInstance = this._unitOfWork.RepositoryAsync<FormInstance>()
                                                .Query()
                                                .Filter(c => c.FormInstanceID == formInstanceId)
                                                .Get().FirstOrDefault();


                    FormInstanceDataMap formInstanceDataMap = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                              .Query()
                                                              .Filter(c => c.FormInstanceID == formInstance.AnchorDocumentID)
                                                              .Get().FirstOrDefault();
                    if (formInstanceDataMap != null)
                    {
                        JObject source = JObject.Parse(formInstanceDataMap.FormData);
                        if (source.SelectToken(layoutTypePath) != null)
                        {
                            if (source.SelectToken(layoutTypePath)[fieldName] != null)
                            {
                                source.SelectToken(layoutTypePath)[fieldName] = dropdownValue;
                                formInstanceDataMap.FormData = JsonConvert.SerializeObject(source);
                                this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Update(formInstanceDataMap);
                                this._unitOfWork.Save();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

        }

        public ServiceResult QueueSBDesignCollateral(string accountIDs, string accountNames, string folderIDs, string folderNames, string formInstanceIDs, string folderVersionIDs, string folderVersionNumbers, string productIds, string formDesignIds, string formDesignVersionIds, int reportTemplateID, string folderVersionEffDts, string reportName, string userName, string collateralFolderPath)
        {
            IEnumerable<string> accountIdList = accountIDs.Split(new char[] { ',' });
            IEnumerable<string> accountNameList = accountNames.Split(new char[] { ',' });
            IEnumerable<string> folderIdList = folderIDs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> folderNameList = folderNames.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> formInstanceIdList = formInstanceIDs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> folderVersionIdList = folderVersionIDs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> folderVersionNumberList = folderVersionNumbers.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> productIdList = productIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> effDatesList = folderVersionEffDts.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> formDesignIdList = formDesignIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> formDesignVersionIdList = formDesignVersionIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);


            List<DateTime> folderVersionEffDatesList = new List<DateTime>();
            foreach (string date in effDatesList)
            {
                DateTime dt = DateTime.Parse(date);
                folderVersionEffDatesList.Add(dt);
            }

            ServiceResult result = new ServiceResult();
            TemplateReportVersion SelectedrptVrsn = new TemplateReportVersion();
            int templateReportVersionID = 0;
            DateTime? templateReportVersionEffectiveDate = null;
            string collateralStorageLocation = string.Empty;
            try
            {
                int templateRptVersionID = 0, finalizedVersionsCnt = 0;
                bool? isFinalized = null;

                //Get number of finalized versions
                finalizedVersionsCnt = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                        where rptvrsn.TemplateReportID == reportTemplateID
                                             && rptvrsn.IsActive == true
                                             && rptvrsn.IsReleased == true
                                        select rptvrsn).Count();

                //Set a flag to true if finalized versions exists
                if (finalizedVersionsCnt > 0)
                    isFinalized = true;
                else
                    isFinalized = false;

                templateRptVersionID = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                        where rptvrsn.TemplateReportID == reportTemplateID
                                             && (rptvrsn.IsReleased == isFinalized || rptvrsn.IsReleased == null)
                                             && rptvrsn.IsActive == true
                                        orderby (rptvrsn.VersionNumber) descending
                                        select rptvrsn.TemplateReportVersionID).FirstOrDefault();

                TemplateReportFormDesignVersionMap selectedrptVrsnmap = (from rptDocMap in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                                                                         join rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                    on rptDocMap.TemplateReportVersionID equals rptvrsn.TemplateReportVersionID
                                                                         where rptvrsn.TemplateReportID == reportTemplateID
                                                                           //&& !string.IsNullOrEmpty(rptvrsn.TemplateFileName)
                                                                           // && !rptvrsn.TemplateReportFormDesignVersionMaps.Select(i => i.FormDesignID).ToList().Except(incomingFormDesignIDs).Any()
                                                                           && rptvrsn.TemplateReportVersionID == templateRptVersionID  // TemplateReportVersionID of report version selected as per folder version effective date
                                                                         orderby rptvrsn.EffectiveDate descending
                                                                         select rptDocMap).FirstOrDefault();

                IEnumerable<FormInstance> formInstances = _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                      .Where(i => formInstanceIdList.ToList().Contains(i.AnchorDocumentID.ToString()) && i.FormInstanceID != i.AnchorDocumentID
                                                      && i.FormDesignID == selectedrptVrsnmap.FormDesignID && i.FormDesignVersionID == selectedrptVrsnmap.FormDesignVersionID)
                                                      .Select(i => i).ToList();

                List<string> formInstanceStrIds = new List<string>();

                foreach (var formInstStr in formInstanceIdList)
                {
                    int formInstID = 0;
                    if(int.TryParse(formInstStr, out formInstID) == true)
                    {
                        var frmInst = from fi in formInstances
                                      where fi.AnchorDocumentID == formInstID
                                      select fi;
                        if(frmInst != null && frmInst.Count() > 0)
                        {
                            formInstanceStrIds.Add(frmInst.First().FormInstanceID.ToString());
                        }
                    }
                }

                List<int> formInstanceIds =  formInstances.Select(i => i.FormInstanceID).ToList();

                var incomingFormDesignIDs = _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                     .Where(i => formInstanceIds.ToList().Contains(i.FormInstanceID))
                                                     .Select(i => i.FormDesignID);

                var incomingFormDesignStrIDs = incomingFormDesignIDs.Select(a => a.ToString());

                var incomingFormDesignVersionIDs = _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                     .Where(i => formInstanceIds.Contains(i.FormInstanceID))
                                     .Select(i => i.FormDesignVersionID);

                var incomingFormDesignVersionStrIDs = incomingFormDesignVersionIDs.Select(a => a.ToString());

                SelectedrptVrsn = (from rptvrsn in _unitOfWork.RepositoryAsync<TemplateReportVersion>().Get()
                                   join rptDocMap in _unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                                    on rptvrsn.TemplateReportVersionID equals rptDocMap.TemplateReportVersionID
                                   where rptvrsn.TemplateReportID == reportTemplateID
                                     && rptvrsn.TemplateReportVersionID == templateRptVersionID  // TemplateReportVersionID of report version selected as per folder version effective date
                                   orderby rptvrsn.EffectiveDate descending
                                   select rptvrsn).FirstOrDefault();

                if (SelectedrptVrsn != null)
                {
                    templateReportVersionID = SelectedrptVrsn.TemplateReportVersionID;
                    templateReportVersionEffectiveDate = SelectedrptVrsn.EffectiveDate;
                    collateralStorageLocation = SelectedrptVrsn.Location;

                    DataTable dataTable = GetSBDesignDataTable(accountIdList, accountNameList, folderIdList, folderNameList, formInstanceStrIds, folderVersionIdList, folderVersionNumberList, productIdList, folderVersionEffDatesList, incomingFormDesignStrIDs, incomingFormDesignVersionStrIDs);
                    SqlParameter collateralQueueType = new SqlParameter("@CollateralQueueType", dataTable);
                    collateralQueueType.TypeName = "[dbo].[SBDesignCollateralQueueType]";

                    List<SqlParameter> lstParam = new List<SqlParameter> {
                                                                       new SqlParameter("@TemplateReportID", reportTemplateID ),
                                                                       new SqlParameter("@TemplateReportVersionID", templateReportVersionID ),
                                                                       new SqlParameter("@TemplateReportVersionEffectiveDate", templateReportVersionEffectiveDate ),
                                                                       new SqlParameter("@CollateralStorageLocation", collateralStorageLocation ),
                                                                       new SqlParameter("@CreatedBy",userName ),
                                                                       new SqlParameter("@CollateralFolderPath", collateralFolderPath),
                                                                       collateralQueueType
                                                                     };
                    var res = this._unitOfWork.Repository<TemplateReportVersion>().ExecuteUpdateSql("EXEC [Setup].[usp_EnqueueSBDesignCollaterals] @TemplateReportID,@TemplateReportVersionID,@TemplateReportVersionEffectiveDate,@CollateralStorageLocation,@CreatedBy,@CollateralFolderPath, @CollateralQueueType", lstParam.ToArray());
                    result.Result = ServiceResultStatus.Success;
                    string description = "Product(s) has been queued against template " + reportName + " of version " + SelectedrptVrsn.VersionNumber;
                    this.SaveTemplateActivityLog(Convert.ToInt32(templateReportVersionID), description, userName);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return result;
        }

        private DataTable GetSBDesignDataTable(IEnumerable<string> accountIdList, IEnumerable<string> accountNameList, IEnumerable<string> folderIdList, IEnumerable<string> folderNameList, IEnumerable<string> formInstanceIdList, IEnumerable<string> folderVersionIdList, IEnumerable<string> folderVersionNumberList, IEnumerable<string> productIdList, List<DateTime> effDatesList, IEnumerable<string> formDesignIdList, IEnumerable<string> formDesignVersionIdList)
        {
            DataTable dt = new DataTable("CollateralQueueType");
            //dt.colummns 
            dt.Columns.Add("ProductID", typeof(string));
            dt.Columns.Add("AccountID", typeof(int));
            dt.Columns.Add("AccountName", typeof(string));
            dt.Columns.Add("FolderID", typeof(int));
            dt.Columns.Add("FolderName", typeof(string));
            dt.Columns.Add("FolderVersionID", typeof(int));
            dt.Columns.Add("FolderVersionNumber", typeof(string));
            dt.Columns.Add("FormInstanceID", typeof(int));
            dt.Columns.Add("FormInstanceName", typeof(string));
            dt.Columns.Add("FormDesignID", typeof(int));
            dt.Columns.Add("FormDesignVersionID", typeof(int));
            dt.Columns.Add("FolderVersionEffectiveDate", typeof(DateTime));

            int numberOfRows = formInstanceIdList.Count();
            for (int index = 0; index < numberOfRows; index++)
            {
                DataRow row = dt.NewRow();
                row["ProductID"] = productIdList.ElementAt(index);
                row["AccountID"] = accountIdList.ElementAt(index) == "" ? "0" : accountIdList.ElementAt(index);
                row["AccountName"] = accountNameList.ElementAt(index);
                row["FolderID"] = folderIdList.ElementAt(index);
                row["FolderName"] = folderNameList.ElementAt(index);
                row["FolderVersionID"] = folderVersionIdList.ElementAt(index);
                row["FolderVersionNumber"] = folderVersionNumberList.ElementAt(index);
                row["FormInstanceID"] = formInstanceIdList.ElementAt(index);
                row["FormInstanceName"] = "";
                row["FormDesignID"] = formDesignIdList.ElementAt(index);
                row["FormDesignVersionID"] = formDesignVersionIdList.ElementAt(index);
                row["FolderVersionEffectiveDate"] = effDatesList.ElementAt(index);
                dt.Rows.Add(row);
            }
            return dt;
        }

        private ServiceResult FillfailServiceResult(ServiceResult result, string message)
        {
            result.Result = ServiceResultStatus.Failure;
            var serviceResultItem = new ServiceResultItem();
            serviceResultItem.Messages = new string[1];
            serviceResultItem.Messages[0] = message;
            var items = new List<ServiceResultItem>();
            items.Add(serviceResultItem);
            result.Items = items;
            return result;
        }


        public ServiceResult SaveCollateralImages(string desciption, string imagePath, string fileName, string name)
        {
            ServiceResult result = new ServiceResult();
            var serviceResultItem = new ServiceResultItem();

            try
            {
                var fileExtensions = fileName.Split('.');
                var fileExtension = fileExtensions[fileExtensions.Length - 1];

                if (!(fileExtension == "jpg"))
                //if (!(fileExtension == "jpeg" || fileExtension == "png" || fileExtension == "jpg"))
                {
                    return FillfailServiceResult(result, "Only jpg image is allowed");
                }
                 
                var uniqueImageNameExists = _unitOfWork.RepositoryAsync<CollateralImages>().Get()
                                            .Where(m => m.ImagePath == imagePath).ToList();

                if (uniqueImageNameExists.Count()>=1)
                {
                    return FillfailServiceResult(result, "Image must be unique");                    
                }
                 uniqueImageNameExists = _unitOfWork.RepositoryAsync<CollateralImages>().Get()
                                            .Where(m => m.Name== name).ToList();

                if (uniqueImageNameExists.Count() >= 1)
                {
                    return FillfailServiceResult(result, "Image name must be unique");
                }
                CollateralImages collateralImages = new CollateralImages()
                {
                    creationDate = DateTime.Now,
                    Description = desciption,
                    ImagePath = imagePath,
                    Name = name
                };
                _unitOfWork.RepositoryAsync<CollateralImages>().Insert(collateralImages);                
                _unitOfWork.Save();
                result.Result = ServiceResultStatus.Success;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return result;
        }
        public ServiceResult DeleteCollateralImages(int id)
        {
            ServiceResult result = new ServiceResult();
            try
            {               
                _unitOfWork.RepositoryAsync<CollateralImages>().Delete(id);                
                _unitOfWork.Save();
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return result;
        }
        public List<CollateralImageView> GetCollateralImages()
        {
            return GetCollateralImages(0);
        }
        public List<CollateralImageView> GetCollateralImages(int id)
        {
            List<CollateralImageView> images = null;
            Expression<Func<CollateralImages, bool>> filter = (m => m.ID == m.ID);
            if (id>0)
            {
               filter = (m => m.ID == id);
            }
            
            images = (from image in _unitOfWork.RepositoryAsync<CollateralImages>().Get().Where(filter)
                   select new CollateralImageView
                   {
                       ImageID = image.ID,
                       Description = image.Description,
                       URL = image.ImagePath,
                       Name = image.Name                       
                   }).ToList();
            foreach (var image in images)
            {
                string[] url = image.URL.Split('/');
                image.FileName = url[url.Length - 1];
            }
            return images;
        }
        public GridPagingResponse<CollateralImageView> GetCollateralImages(GridPagingRequest gridPagingRequest)
        {
            List<CollateralImageView> images = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                images = (from image in _unitOfWork.RepositoryAsync<CollateralImages>().Get()
                       select new CollateralImageView
                       {
                           ImageID = image.ID,
                           Description = image.Description,
                           URL = image.ImagePath,
                           Name = image.Name
                       }).ApplySearchCriteria(criteria)
                         .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                         .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);

                foreach( var image in images)
                {
                    string[] url = image.URL.Split('\\');
                    image.FileName = url[url.Length - 1];
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return new GridPagingResponse<CollateralImageView>(gridPagingRequest.page, count, gridPagingRequest.rows, images);
        }

        public string[] GetPlanFamily(string reportName)
        {                     
            var plans = (from fi in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                         join acc in _unitOfWork.RepositoryAsync<AccountProductMap>().Get()
                         on fi.FormInstanceID equals acc.FormInstanceID
                         join fd in _unitOfWork.RepositoryAsync<FormDesign>().Get()
                         on fi.FormDesignID equals fd.FormID
                         where fd.FormName == "Medicare"
                         select acc.ANOCChartPlanType + "_" + acc.RXBenefit + "_" + acc.SNPType
                   );

            return plans.Distinct().ToArray();
        }

        private List<ReportDocumentModel> getReportModel(string planFamilyName, IEnumerable<int> anchorDesignIDs, bool shouldSelectFormInstance, GridPagingRequest gridPagingRequest, SearchCriteria criteria, int count)
        {

            var plans = (from fi in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                         join acc in _unitOfWork.RepositoryAsync<AccountProductMap>().Get()
                         on fi.FormInstanceID equals acc.FormInstanceID
                         join fd in _unitOfWork.RepositoryAsync<FormDesign>().Get()
                         on fi.FormDesignID equals fd.FormID
                         where fd.FormName == "Medicare" && planFamilyName == (acc.ANOCChartPlanType + "_" + acc.RXBenefit + "_" + acc.SNPType)
                         select acc.FormInstanceID.ToString()
                   );

            string[] formInstanceIDs = plans.Distinct().ToArray();

            List<ReportDocumentModel> result = null;
            result = (from fldr in _unitOfWork.RepositoryAsync<Folder>().Get()
                      join fldrvrsn in _unitOfWork.RepositoryAsync<FolderVersion>().Get()
                          on fldr.FolderID equals fldrvrsn.FolderID
                      join formins in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                          on fldrvrsn.FolderVersionID equals formins.FolderVersionID
                      join des in _unitOfWork.RepositoryAsync<FormDesign>().Get()
                          on formins.FormDesignID equals des.FormID
                      join desVer in _unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                      on des.FormID equals desVer.FormDesignID
                      join desginMapping in _unitOfWork.RepositoryAsync<FormDesignGroupMapping>().Get()
                          on formins.FormDesignID equals desginMapping.FormID
                          into tmp
                      from desginMapping in tmp.DefaultIfEmpty()
                      join accFldrmap in _unitOfWork.RepositoryAsync<AccountFolderMap>().Get()
                          on fldr.FolderID equals accFldrmap.FolderID
                          into tempAccMap
                      from accFldrmap in tempAccMap.DefaultIfEmpty()
                      join acc in _unitOfWork.RepositoryAsync<Account>().Get()
                          on accFldrmap.AccountID equals acc.AccountID
                          into tmpAcc
                      from acc in tmpAcc.DefaultIfEmpty()
                      where fldrvrsn.IsActive && formins.IsActive && des.IsActive
                            && des.IsMasterList == false
                            && desVer.FormDesignVersionID == formins.FormDesignVersionID
                            && formInstanceIDs.Contains(formins.FormInstanceID.ToString())
                          //&& (acc.AccountID == null || AccountID == 0 || acc.AccountID == AccountID)
                          //&& (FolderID == 0 || fldr.FolderID == FolderID)
                          //&& (FolderVersionID == 0 || fldrvrsn.FolderVersionID == FolderVersionID)
                          && (anchorDesignIDs.Contains(formins.FormDesignID)) // To get only those products whoes FormDesignID is used as Anchor product in report designs
                      select new ReportDocumentModel
                      {
                          AccountID = acc.AccountID,
                          AccountName = acc.AccountName ?? "",
                          FolderID = fldr.FolderID,
                          FolderName = fldr.Name,
                          FolderVersionID = fldrvrsn.FolderVersionID,
                          FolderVersionEffectiveDate = fldrvrsn.EffectiveDate,
                          FolderVersionNumber = fldrvrsn.FolderVersionNumber,
                          FormInstanceID = shouldSelectFormInstance == true ? formins.FormInstanceID : 0,
                          FormName = shouldSelectFormInstance == true ? formins.Name : "",
                          AnchorDocumentName = shouldSelectFormInstance == true ? des.FormName : "",
                          FormDesignVersionID = desVer.FormDesignVersionID,
                          FormDesignID = (int)desVer.FormDesignID,
                      }).Distinct().ApplySearchCriteria(criteria)
                          .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                          .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);

            return result;
        }

        public string SaveCollatralPrintXFormatedPDF(int processQueue1Up, string CollatralFilePath)
        {

            CollateralProcessQueue CollatralObj = this._unitOfWork.RepositoryAsync<CollateralProcessQueue>()
                                                                        .Query()
                                                                        .Filter(c => c.CollateralProcessQueue1Up == processQueue1Up)
                                                                        .Get()
                                                                        .FirstOrDefault();
            if (CollatralObj != null)
            {
                CollatralObj.PrintxFilePath = CollatralFilePath;
                this._unitOfWork.RepositoryAsync<CollateralProcessQueue>().Update(CollatralObj);
                this._unitOfWork.Save();
            }
            string formInstanceID = string.Empty;
            var queuedProduct = (from queue in _unitOfWork.RepositoryAsync<CollateralProcessQueue>().Get()
                                 where queue.CollateralProcessQueue1Up == processQueue1Up
                                 select queue).FirstOrDefault();
            if (queuedProduct != null)
            {
                formInstanceID = queuedProduct.FormInstanceID.ToString();
            }
            return formInstanceID;
        }

        public GridPagingResponse<UploadReportModel> GetCollateralProcessUploadList(GridPagingRequest gridPagingRequest)
        {
            List<UploadReportModel> lst = null;
            int count = 0;
            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
                lst = (from queue in _unitOfWork.RepositoryAsync<CollateralProcessUpload>().Get()
                       select new UploadReportModel
                       {
                           ID = queue.ID,
                           ProductID = queue.ProductID,
                           AccountID = queue.AccountID ?? 0,
                           AccountName = queue.AccountName,
                           FolderName = queue.FolderName,
                           FolderVersionNumber = queue.FolderVersionNumber,
                           FormInstanceName = queue.FormInstanceName,
                           CollateralName = queue.CollateralName,
                           FormInstanceID = queue.FormInstanceID ?? 0,
                           CreatedDate = queue.CreatedDate,
                       }).ApplySearchCriteria(criteria)
                         .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                         .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return new GridPagingResponse<UploadReportModel>(gridPagingRequest.page, count, gridPagingRequest.rows, lst);
        }

        public ServiceResult UploadCollateral(UploadReportViewModel uploadReportViewModel)
        {
            ServiceResult serviceResult = new ServiceResult();

            CollateralProcessUpload uploadReportModel = new CollateralProcessUpload()
            {
                ProductID = uploadReportViewModel.ProductID,
                AccountID = uploadReportViewModel.AccountID,
                AccountName = uploadReportViewModel.AccountName,
                FolderName = uploadReportViewModel.FolderName,
                FolderVersionNumber = uploadReportViewModel.FolderVersionNumber,
                CreatedDate = DateTime.Now,
                WordFile = uploadReportViewModel.WordFile,
                PrintxFile = uploadReportViewModel.PrintxFile,
                File508 = uploadReportViewModel.File508,
                FolderID = uploadReportViewModel.FolderID,
                FolderVersionID = uploadReportViewModel.FolderVersionId,
                FormDesignID = uploadReportViewModel.FormDesignID,
                FormDesignVersionID = uploadReportViewModel.FormDesignVersionID,
                FormInstanceID = uploadReportViewModel.FormInstanceID,
                FormInstanceName= uploadReportViewModel.FormInstanceName,
                CollateralName = uploadReportViewModel.CollateralName,
                CreatedBy = uploadReportViewModel.CreatedBy
            };
            _unitOfWork.RepositoryAsync<CollateralProcessUpload>().Insert(uploadReportModel);
            _unitOfWork.Save();
            serviceResult.Result = ServiceResultStatus.Success;

            return serviceResult;
        }

        public DownloadFileModel GetManualUploadedFilePath(int id, string fileFormat)
        {
            DownloadFileModel downloadFileModel = new DownloadFileModel();
            try
            {
                var queuedProduct = (from queue in _unitOfWork.RepositoryAsync<CollateralProcessUpload>().Get()
                                     where queue.ID == id
                                     select queue).FirstOrDefault();
                if (queuedProduct != null)
                {
                    if (fileFormat == "pdf")
                    {
                        downloadFileModel.FileName = string.Format("{0}.pdf", queuedProduct.FormInstanceName);
                        downloadFileModel.FileContent = queuedProduct.File508;
                    }
                    else if (fileFormat == "word")
                    {
                        downloadFileModel.FileName = string.Format("{0}.docx", queuedProduct.FormInstanceName);
                        downloadFileModel.FileContent = queuedProduct.WordFile;
                    }
                    else if (fileFormat == "pdfx")
                    {
                        downloadFileModel.FileName = string.Format("{0}_print.pdf", queuedProduct.FormInstanceName);
                        downloadFileModel.FileContent = queuedProduct.PrintxFile;
                    }
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return downloadFileModel;
        }
    }
    public enum UserRoleEnum
    {
        EBAAnalyst = 21,
        TPAAnalyst = 22,
        Viewer = 23,
        SuperUser = 24
    }
}

