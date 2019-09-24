using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.util;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class FormInstanceRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        #endregion Constructor

        #region Public Methods
        public static bool IsFormInstanceExists(this IRepositoryAsync<FormInstance> formInstanceRepository, int tenantId, int folderVersionId, int formDesignId, int formDesignVersionId)
        {
            var instance = formInstanceRepository.Query()
                                                .Filter(f => f.FormDesign.FormDesignGroupMappings.Where(a => a.AllowMultipleInstance == false && a.FormID == formDesignId).Any())
                                                .Get().ToList();

            if (instance != null && instance.Any())
                return instance
                         .Where(fi => fi.TenantID == tenantId && fi.FormDesignID == formDesignId && fi.FolderVersionID == folderVersionId && fi.FormDesignVersionID == formDesignVersionId && fi.IsActive == true)
                         .Any();
            else
                return false;
        }

        public static bool IsFormInstanceNameExist(this IRepositoryAsync<FormInstance> formInstanceRepository, int tenantId, int folderVersionId, int formDesignId, int formDesignVersionId, string formName)
        {
            var instance = formInstanceRepository.Query()
                                                .Filter(fi => fi.TenantID == tenantId && fi.FormDesignID == formDesignId && fi.FolderVersionID == folderVersionId
                                                        && fi.FormDesignVersionID == formDesignVersionId && fi.IsActive == true && fi.Name == formName && fi.FormInstanceID == fi.AnchorDocumentID)
                                                .Get().ToList();
            if (instance != null && instance.Any())
                return true;
            else
                return false;
        }
        
        public static FormInstanceDataMap GetFormInstanceDataDecompressed(this IRepositoryAsync<FormInstanceDataMap> formInstanceRepository, int formInstanceID)
        {

            SqlParameter paramFormInstanceID = new SqlParameter("@FormInstanceID", formInstanceID);

            FormInstanceDataMap dataMap = formInstanceRepository.ExecuteSql("exec [dbo].[uspGetFormInstanceData] @FormInstanceID", paramFormInstanceID).FirstOrDefault();

            if (dataMap != null)
            {
                if (!string.IsNullOrEmpty(dataMap.FormData))
                {
                    ICompressionBase handler = CompressionFactory.GetCompressionFactory(CompressionType.JSON, null, "", "", "");
                    dataMap.FormData = handler.Decompress(dataMap.FormData).ToString();
                }
            }

            return dataMap;
        }

        public static List<FormInstanceDataMap> GetFormInstanceDataList(this IRepositoryAsync<FormInstanceDataMap> formInstanceRepository, List<int> formInstanceID)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FormInstanceId", typeof(int));
            DataRow row = null;
            foreach (int x in formInstanceID)
            {
                row = dt.NewRow();
                row[0] = x;
                dt.Rows.Add(row);
            }

            SqlParameter formInstances = new SqlParameter("@FormInstanceIdList", SqlDbType.Structured)
            {
                TypeName = "[dbo].[FormInstances]",
                Value = dt
            };


            List<FormInstanceDataMap> dataMaps = formInstanceRepository.ExecuteSql("exec [dbo].[uspGetFormInstanceDataList] @FormInstanceIdList", formInstances).ToList();

            if (dataMaps != null)
            {
                foreach (var item in dataMaps)
                {
                    if (!string.IsNullOrEmpty(item.FormData))
                    {
                        ICompressionBase handler = CompressionFactory.GetCompressionFactory(CompressionType.JSON, null, "", "", "");
                        item.FormData = handler.Decompress(item.FormData).ToString();
                    }
                }

            }
            return dataMaps;
        }

        public static bool SaveFormInstanceDataCompressed(this IRepositoryAsync<FormInstanceDataMap> formInstanceRepository, int formInstanceID, string formData)
        {
            if (!string.IsNullOrEmpty(formData))
            {
                ICompressionBase handler = CompressionFactory.GetCompressionFactory(CompressionType.JSON, null, "", "", "");
                formData = handler.Compress(formData).ToString();
            }

            SqlParameter paramFormInstanceID = new SqlParameter("@FormInstanceID", formInstanceID);
            SqlParameter paramFormData = new SqlParameter("@FormData", formData);

            int count = formInstanceRepository.ExecuteUpdateSql("exec [dbo].[uspSaveFormInstanceData] @FormInstanceID,@FormData", paramFormInstanceID, paramFormData);

            bool result = count > 0 ? true : false;
            return result;
        }

        public static List<FormInstanceDataMap> GetFormInstanceAuditChecklistDataList(this IRepositoryAsync<FormInstanceDataMap> formInstanceRepository, List<int> formInstanceID)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FormInstanceId", typeof(int));
            DataRow row = null;
            foreach (int x in formInstanceID)
            {
                row = dt.NewRow();
                row[0] = x;
                dt.Rows.Add(row);
            }

            SqlParameter formInstances = new SqlParameter("@FormInstanceIdList", SqlDbType.Structured)
            {
                TypeName = "[dbo].[FormInstances]",
                Value = dt
            };

            List<FormInstanceDataMap> dataMaps = formInstanceRepository.ExecuteSql("exec [dbo].[uspGetAuditFormInstanceDataList] @FormInstanceIdList", formInstances).ToList();

            if (dataMaps != null)
            {
                foreach (var item in dataMaps)
                {
                    if (!string.IsNullOrEmpty(item.FormData))
                    {
                        ICompressionBase handler = CompressionFactory.GetCompressionFactory(CompressionType.JSON, null, "", "", "");
                        item.FormData = handler.Decompress(item.FormData).ToString();
                    }
                }
            }
            return dataMaps;
        }
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
