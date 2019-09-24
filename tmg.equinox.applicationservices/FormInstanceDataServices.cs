using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.infrastructure.util;
using System;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Collections.Generic;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;

namespace tmg.equinox.applicationservices
{
    public partial class FormInstanceDataServices : IFormInstanceDataServices
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members

        #region Constructor
        public FormInstanceDataServices(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        public string GetSectionData(int tenantId, int formInstanceId, string sectionName, FormDesignVersionDetail detail, string currentUserName)
        {
            string data = "";
            SqlParameter paramFormInstanceID = new SqlParameter("@FormInstanceID", formInstanceId);
            SqlParameter paramSectionName = new SqlParameter("@SectionName", sectionName);


            FormInstanceDataMap dataMap = this._unitOfWork.Repository<FormInstanceDataMap>().ExecuteSql("exec [dbo].[uspGetSectionData] @FormInstanceID,@SectionName", paramFormInstanceID, paramSectionName).ToList().FirstOrDefault();

            if (dataMap != null)
            {
                ICompressionBase handler = CompressionFactory.GetCompressionFactory(CompressionType.JSON, null, "", "", "");
                dataMap.FormData = handler.Decompress(dataMap.FormData).ToString();
                return data = this.AddNodeToSectionData(sectionName, dataMap.FormData);
            }
            else
            {
                string defaultJSONData = detail.GetDefaultJSONDataObject();
                detail.IsNewFormInstance = true;
                this.SaveDefaultJSONData(tenantId, formInstanceId, defaultJSONData, currentUserName);
                data = JsonConvert.SerializeObject(JObject.Parse(defaultJSONData)[sectionName]);
                return data = this.AddNodeToSectionData(sectionName, data);
            }
        }

        public void UpdateSectionData(int tenantId, int formInstanceId, string sectionName, string sectionData)
        {
            ICompressionBase handler = CompressionFactory.GetCompressionFactory(CompressionType.JSON, null, "", "", "");
            sectionData = handler.Compress(sectionData).ToString();

            SqlParameter paramFormInstanceID = new SqlParameter("@FormInstanceID", formInstanceId);
            SqlParameter paramSectionName = new SqlParameter("@SectionName", sectionName);
            SqlParameter paramSectionData = new SqlParameter("@SectionData", sectionData);

            FormInstanceDataMap dataMap = this._unitOfWork.Repository<FormInstanceDataMap>().ExecuteSql("exec [dbo].[uspUpdateSectionData] @FormInstanceID,@SectionName,@SectionData", paramFormInstanceID, paramSectionName, paramSectionData).FirstOrDefault();
        }

        public ServiceResult SaveFormInstanceSectionsData(int tenantId, string jsonString, string currentUserName)
        {
            ServiceResult result = new ServiceResult();

            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
            SqlParameter paramtenantId = new SqlParameter("@TenantID", tenantId);
            SqlParameter paramcurrentUserName = new SqlParameter("@CurrentUserName", currentUserName);
            SqlParameter formInstances = new SqlParameter("@FormInstanceSectionList", SqlDbType.Structured)
            {
                TypeName = "[dbo].[FormInstanceSectionType]",
                Value = dt
            };

            FormInstanceDataMap dataMaps = this._unitOfWork.Repository<FormInstanceDataMap>().ExecuteSql("exec [dbo].[uspSaveFormInstanceSectionsData] @FormInstanceSectionList,@TenantID,@CurrentUserName", formInstances, paramtenantId, paramcurrentUserName).FirstOrDefault();

            if (dataMaps != null)
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

        public void SaveDefaultJSONData(int tenantId, int formInstanceId, string defaultJSONData, string currentUserName)
        {

            FormInstanceDataMap formInstanceDataMap = null;
            var fiMap = (from c in this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                .Query()
                                                                .Filter(c => c.FormInstanceID == formInstanceId)
                                                                .Get()
                         select new
                         {
                             FormInstanceDataMapID = c.FormInstanceDataMapID,
                             FormInstanceID = c.FormInstanceID,
                             ObjectInstanceID = c.ObjectInstanceID
                         }).FirstOrDefault();
            if (fiMap == null)
            {
                formInstanceDataMap = new FormInstanceDataMap();
                formInstanceDataMap.FormInstanceID = formInstanceId;
                formInstanceDataMap.ObjectInstanceID = 0;
                formInstanceDataMap.FormData = defaultJSONData;

                this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Insert(formInstanceDataMap);
                this._unitOfWork.Save();



                ICompressionBase compressionBase = CompressionFactory.GetCompressionFactory(CompressionType.Memory);
                byte[] jsonDataCompressed = (byte[])compressionBase.Compress(defaultJSONData);


                FormInstanceHistory formInstanceHistory = new FormInstanceHistory();
                formInstanceHistory.FormInstanceID = formInstanceId;
                formInstanceHistory.FormData = jsonDataCompressed;
                formInstanceHistory.TenantID = tenantId;
                formInstanceHistory.EnteredBy = currentUserName;
                formInstanceHistory.EnteredDate = DateTime.Now;
                formInstanceHistory.Action = "Add";
                this._unitOfWork.RepositoryAsync<FormInstanceHistory>().Insert(formInstanceHistory);
                this._unitOfWork.Save();

            }
        }

        public string AddNodeToSectionData(string sectionName, string sectionData)
        {
            if (sectionData != "null" && sectionData != null)
            {
                JObject dataJObj = JObject.Parse("{'" + sectionName + "':[]}");
                dataJObj[sectionName] = JObject.Parse(sectionData);
                return sectionData = JsonConvert.SerializeObject(dataJObj);
            }

            return string.Empty;
        }

        public dynamic IsMasterListFolderVersionRelease(int formInstanceId)
        {
            bool inProgress = false;
            string obj = null;
            var folderVersionObj = obj;
            try
            {
                if (formInstanceId > 0)
                {
                    var folderStatus = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                        join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                            on fi.FolderVersionID equals fv.FolderVersionID
                                        where fi.FormInstanceID == formInstanceId
                                        select new
                                        {
                                            fv.FolderVersionID,
                                            fv.FolderVersionStateID
                                        }).FirstOrDefault();
                    return folderStatus;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return folderVersionObj;
        }

        public ServiceResult SaveFormInstanceCommentLog(int formInstanceId, int formDesignId, int formDesignVersionId, string userName, string commentData)
        {
            ServiceResult result = null;
            result = new ServiceResult();
            try
            {

                var designType = (from des in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                  join type in this._unitOfWork.RepositoryAsync<DocumentDesignType>().Get() on des.DocumentDesignTypeID equals type.DocumentDesignTypeID
                                  where des.FormID == formDesignId && (type.DocumentDesignName == "Collateral" || type.DocumentDesignName == "MasterList")
                                  select des).ToList();

                if (designType.Count > 0)
                {
                    FormInstanceCommentLog commentLog = this._unitOfWork.RepositoryAsync<FormInstanceCommentLog>()
                                                                            .Query()
                                                                            .Filter(c => c.FormInstanceID == formInstanceId)
                                                                            .Get()
                                                                            .FirstOrDefault();
                    if (commentLog != null)
                    {
                        commentLog.CommentData = commentData;
                        commentLog.UpdatedBy = userName;
                        commentLog.UpdatedDate = DateTime.Now;
                        this._unitOfWork.RepositoryAsync<FormInstanceCommentLog>().Update(commentLog);
                        this._unitOfWork.Save();
                    }
                    else
                    {
                        FormInstanceCommentLog objCommentLog = new FormInstanceCommentLog();
                        objCommentLog.FormInstanceID = formInstanceId;
                        objCommentLog.FormDesignID = formDesignId;
                        objCommentLog.FormDesignVersionID = formDesignVersionId;
                        objCommentLog.CommentData = commentData;
                        objCommentLog.AddedBy = userName;
                        objCommentLog.AddedDate = DateTime.Now;
                        this._unitOfWork.RepositoryAsync<FormInstanceCommentLog>().Insert(objCommentLog);
                        this._unitOfWork.Save();
                    }
                }
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return result;
        }
        public string GetFormInstanceCommentLog(int formInstanceId)
        {
            string data = String.Empty;
            try
            {
                var comments = (from c in this._unitOfWork.RepositoryAsync<FormInstanceCommentLog>()
                                             .Query()
                                             .Filter(c => c.FormInstanceID == formInstanceId)
                                             .Get()
                                select c).FirstOrDefault();
                if (comments != null && comments.CommentData != null)
                {
                    data = comments.CommentData;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return data;
        }

        //public List<FormInstanceAllProductsViewModel> GetFormInstanceProductsList(int formInstanceId)
        //{
        //    List<FormInstanceAllProductsViewModel> productsList = new List<FormInstanceAllProductsViewModel>();
        //    List<int> productsDesignIds = new List<int>() {2447, 2448, 2449, 2450, 2405 };
        //    try
        //    {
        //        productsList = (from apm in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get()
        //                        join fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
        //                                    on apm.FormInstanceID equals fi.FormInstanceID
        //                        where productsDesignIds.Contains(fi.FormDesignID)
        //                        select new FormInstanceAllProductsViewModel() { FormInstanceID = apm.FormInstanceID, ProductID = apm.ProductID, ProductName = apm.ProductName, ProductType = apm.ProductType, DocumentName = fi.Name}).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
        //    }
        //    return productsList;
        //}
    }
}