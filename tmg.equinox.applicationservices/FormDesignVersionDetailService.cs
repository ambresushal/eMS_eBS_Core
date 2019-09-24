using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using tmg.equinox.applicationservices.FormDesignBuilderFromDomainModel;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignBuilderFromDomainModel;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.FormDesignDetail;
using tmg.equinox.infrastructure.util;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace tmg.equinox.applicationservices
{
    public partial class FormDesignService : IFormDesignService
    {
        #region Public Methods
        public FormDesignVersionDetail GetFormDesignVersionDetail(int tenantId, int formDesignVersionId)
        {
            FormDesignVersionDetail detail = null;
            try
            {
                FormDesignBuilder builder = new FormDesignBuilder(tenantId, formDesignVersionId, _unitOfWork);
                detail = builder.BuildFormDesign();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return detail;
        }

        public ServiceResult SaveCompiledFormDesignVersion(int tenantId, int formDesignVersionId, string jsonData, string userName)
        {
            ServiceResult result = new ServiceResult();
            result.Result = ServiceResultStatus.Failure;
            string formDesignVersionData = jsonData;
            try
            {
                if (!string.IsNullOrEmpty(jsonData))
                {
                    ICompressionBase handler = CompressionFactory.GetCompressionFactory(CompressionType.JSON, null, "", "", "");
                    jsonData = handler.Compress(jsonData).ToString();
                }

                SqlParameter paramTenantID = new SqlParameter("@TenantID", tenantId);
                SqlParameter paramFormDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                SqlParameter paramJsonData = new SqlParameter("@JsonData", jsonData);
                SqlParameter paramCurrentUserName = new SqlParameter("@CurrentUserName", userName);
                SqlParameter paramUpdateFieldType = new SqlParameter("@UpdateFieldType", "FormDesignVersionData");

                FormDesignVersion frmDesignVersion = this._unitOfWork.Repository<FormDesignVersion>().ExecuteSql("exec [dbo].[uspSaveCompiledFormDesignVersionData] @TenantID,@FormDesignVersionID,@JsonData,@CurrentUserName,@UpdateFieldType", paramTenantID, paramFormDesignVersionID, paramJsonData, paramCurrentUserName, paramUpdateFieldType).FirstOrDefault();

                if (frmDesignVersion != null)
                {
                    SaveFormDesignHistory(formDesignVersionId, formDesignVersionData, userName, DateTime.Now, "Compile", tenantId);
                    result.Result = ServiceResultStatus.Success;
                    //add design for reporting database update
                    AddDesignToReportingDBUpdateQueue((int)frmDesignVersion.FormDesignID, formDesignVersionId, formDesignVersionData);
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
                if (reThrow)
                    throw;
            }
            return result;
        }

        private void AddDesignToReportingDBUpdateQueue(int formDesignId, int formDesignVersionId, string formDesignVersionData)
        {
            try
            {
                var schemaUpdateTracker = this._unitOfWork.RepositoryAsync<SchemaUpdateTracker>()
                                            .Query()
                                            .Filter(c => c.FormdesignVersionID == formDesignVersionId)
                                            .Get().FirstOrDefault();
                if (schemaUpdateTracker != null)
                {
                    var previousDataHash = schemaUpdateTracker.OldJsonHash;
                    string currentDataHash = HashGenerator.ToMD5(formDesignVersionData.ToString());

                    if (previousDataHash != currentDataHash)
                    {
                        schemaUpdateTracker.Status = 1;
                        this._unitOfWork.RepositoryAsync<SchemaUpdateTracker>().Update(schemaUpdateTracker);
                        this._unitOfWork.Save();
                    }
                }
                else
                {
                    string currentDataHash = HashGenerator.ToMD5(formDesignVersionData.ToString());
                    SchemaUpdateTracker schematracker = new SchemaUpdateTracker()
                    {
                        FormdesignID = formDesignId,
                        FormdesignVersionID = formDesignVersionId,
                        OldJsonHash = "",
                        CurrentJsonHash = currentDataHash,
                        AddedDate = DateTime.Now,
                        Status = 1,
                        UpdatedDate = null
                    };
                    this._unitOfWork.RepositoryAsync<SchemaUpdateTracker>().Insert(schematracker);
                    this._unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public string GetCompiledFormDesignVersion(int tenantId, int formDesignVersionId)
        {
            string detail = "";
            try
            {
                var formDesignVersion = (from ver in this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                            .Query()
                                            .Include(inc => inc.FormDesign)
                                            .Filter(c => c.TenantID == tenantId && c.FormDesignVersionID == formDesignVersionId)
                                            .Get()
                                         select new
                                         {
                                             ver.FormDesignVersionData,
                                             ver.FormDesign.IsMasterList
                                         }).FirstOrDefault();

                if (formDesignVersion != null && formDesignVersion.FormDesignVersionData != null)
                {
                    FormDesignVersionDetail verdetail = JsonConvert.DeserializeObject<FormDesignVersionDetail>(formDesignVersion.FormDesignVersionData);
                    if (verdetail != null)
                    {
                        verdetail.IsMasterList = formDesignVersion.IsMasterList;
                        detail = JsonConvert.SerializeObject(verdetail);
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return detail;
        }

        public string GetEventMapJSON(int tenantId, int formDesignVersionId)
        {
            string detail = "";
            try
            {
                var formDesignVersion = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                    .Get()
                    .Where(s => s.FormDesignVersionID == formDesignVersionId)
                    .Select(s => s.RuleEventMapJSON)
                    .FirstOrDefault();

                if (formDesignVersion != null)
                {
                    detail = formDesignVersion;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return detail;
        }

        public string GetExecutionTreeJSON(int tenantId, int formDesignVersionId)
        {
            string detail = "";
            try
            {
                var ruleTreeJson = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                        .Get()
                                        .Where(f => f.FormDesignVersionID == formDesignVersionId)
                                        .Select(s => s.RuleExecutionTreeJSON)
                                        .FirstOrDefault();
                if (ruleTreeJson != null)
                {
                    detail = ruleTreeJson;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return detail;
        }

        public FormDesignVersionDetailFromDM GetFormDesignVersionDetailFromDataModel(int tenantId, int formDesignVersionId)
        {
            FormDesignVersionDetailFromDM detail = null;
            try
            {
                var builder = new FormDesignBuilderFromDM(tenantId, formDesignVersionId, _unitOfWork);
                detail = builder.BuildFormDesignFromDataModel();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return detail;
        }

        public void SaveFormDesignHistory(int formDesignVersionId, string formDesignVersionData, string enteredBy, DateTime enteredDate, string action, int tenantId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ICompressionBase compressionBase = CompressionFactory.GetCompressionFactory(CompressionType.Memory);
                byte[] jsonDataCompressed = (byte[])compressionBase.Compress(formDesignVersionData);

                /*byte[] jsonDataCompressed = Compression.CompressDataThroughByteArray(formDesignVersionData);*/

                //CheckFormDesignVersionHistoryOriginalJsonData(jsonDataCompressed);

                FormDesignHistory formDesignHistory = new FormDesignHistory();
                formDesignHistory.FormDesignVersionId = formDesignVersionId;
                formDesignHistory.FormDesignVersionData = jsonDataCompressed;
                formDesignHistory.EnteredBy = enteredBy;
                formDesignHistory.EnteredDate = enteredDate;
                formDesignHistory.TenantID = tenantId;
                formDesignHistory.Action = action;

                //Call to repository method to insert record.
                this._unitOfWork.RepositoryAsync<FormDesignHistory>().Insert(formDesignHistory);
                this._unitOfWork.Save();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
        }

        public int GetLatestFormDesignVersionID(string formDesign, DateTime effectiveDate)
        {
            int formDesignVersionID = 0;
            var formDesignVersions = (from ver in this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                        .Query()
                                        .Include(inc => inc.FormDesign)
                                        .Filter(c => c.EffectiveDate <= effectiveDate && c.FormDesign.FormName == formDesign)
                                        .Get()
                                      select new
                                      {
                                          ver.FormDesignVersionID,
                                          ver.StatusID,
                                          ver.EffectiveDate
                                      });
            if (formDesignVersions != null && formDesignVersions.Count() > 0)
            {
                var fdvList = formDesignVersions.OrderByDescending(a => a.EffectiveDate);
                foreach (var fdv in fdvList)
                {
                    if (fdv.StatusID == 1 || fdv.StatusID == 3)
                    {
                        formDesignVersionID = fdv.FormDesignVersionID;
                    }
                    if (fdv.StatusID == 3)
                    {
                        break;
                    }
                }
            }
            return formDesignVersionID;
        }

        public List<FormDesignVersionActivityLog> GetFormDesignVersionActivityLogData(int formDesignId, int formDesignVersionId, int formDesignPreviousVersionId)
        {
            List<FormDesignVersionActivityLog> data = null;
            try
            {
                SqlParameter paramFormID = new SqlParameter("@FormID", formDesignId);
                SqlParameter paramFirstVersion = new SqlParameter("@FirstVersion", formDesignPreviousVersionId);
                SqlParameter paramSecondVersion = new SqlParameter("@SecondVersion", formDesignVersionId);
                data = this._unitOfWork.Repository<FormDesignVersionActivityLog>().ExecuteSql("exec [dbo].[GetFormDesignVersionsDetail] @FormID,@FirstVersion,@SecondVersion", paramFormID, paramFirstVersion, paramSecondVersion).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return data;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// FormDesignVersionData is stored in byte format in Arc.FormDesignHistory table
        /// So if we want to check original JsonData we need to pass byte array data stored in table
        /// to this method to get original json data.
        /// </summary>
        /// <param name="jsonDataCompressed"></param>
        private static void CheckFormDesignVersionHistoryOriginalJsonData(byte[] jsonDataCompressed)
        {
            ICompressionBase compressionBase = CompressionFactory.GetCompressionFactory(CompressionType.Memory, jsonDataCompressed);
            byte[] decompress = (byte[])compressionBase.Decompress();
            /*byte[] decompress = Compression.Decompress(jsonDataCompressed);*/

            string jsonStr = Encoding.UTF8.GetString(decompress);
        }

        #endregion
    }
}
