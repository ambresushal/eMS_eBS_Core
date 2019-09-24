using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;

namespace tmg.equinox.web
{
    public static class QhpConfig
    {
        #region Public/ Protected / Private Member Variables
        private static NameValueCollection qhpConfigurationSection;
        private static string qhpXMLAppDataPath;
        private static string qhpStaging;
        private static string qhpExceptionFileName;
        private static bool deleteOldQhpMappingExceptionFilesFlag;
        private static string qhpXSDSchemaFileName;
        private static string qhpXmlFileExtension;
        private static string qhptargetNameSpace;
        public static string QhpXMLAppDataPath
        {
            get
            {
                return qhpXMLAppDataPath;
            }
        }
        public static string QhpStaging
        {
            get
            {
                return qhpStaging;
            }
        }
        public static string QhpExceptionFileName
        {
            get
            {
                return qhpExceptionFileName;
            }
        }
        public static bool DeleteQhpMappingExceptionFileFlag
        {
            get
            {
                return deleteOldQhpMappingExceptionFilesFlag;
            }
        }

        public static string QhpXSDSchemaFileName
        {
            get
            {
                return qhpXSDSchemaFileName;
            }
        }

        public static string QhpXmlFileExtension
        {
            get
            {
                return qhpXmlFileExtension;
            }
        }
        public static string QhpTargetNameSpace
        {
            get
            {
                return qhptargetNameSpace;
            }
        }

        #endregion Member Variables

        #region Constructor/Dispose

        #endregion Constructor/Dispose


        #region Public Methods
        public static void InitializeQhpConfigSettings()
        {
            qhpConfigurationSection = (NameValueCollection)ConfigurationManager.GetSection("qhp");
            qhpXMLAppDataPath = (!string.IsNullOrEmpty(Convert.ToString(qhpConfigurationSection["QhpXMLAppDataPath"]))) ? Convert.ToString(qhpConfigurationSection["QhpXMLAppDataPath"]) : "App_Data\\QHP\\";
            qhpStaging = (!string.IsNullOrEmpty(Convert.ToString(qhpConfigurationSection["QhpStaging"]))) ? Convert.ToString(qhpConfigurationSection["QhpStaging"]) : "Staging\\";
            qhpExceptionFileName = (!string.IsNullOrEmpty(Convert.ToString(qhpConfigurationSection["QhpExceptionFileName"]))) ? Convert.ToString(qhpConfigurationSection["QhpExceptionFileName"]) : "QhpMappingExceptions";
            deleteOldQhpMappingExceptionFilesFlag = (!string.IsNullOrEmpty(Convert.ToString(qhpConfigurationSection["DeleteQhpMappingExceptionFiles"]))) ? Convert.ToBoolean(qhpConfigurationSection["DeleteQhpMappingExceptionFiles"]) : false;
            qhpXSDSchemaFileName = (!string.IsNullOrEmpty(Convert.ToString(qhpConfigurationSection["QhpXSDFileName"]))) ? Convert.ToString(qhpConfigurationSection["QhpXSDFileName"]) : "QHPTemplate.xsd";
            qhpXmlFileExtension = (!string.IsNullOrEmpty(Convert.ToString(qhpConfigurationSection["QhpXmlFileExt"]))) ? Convert.ToString(qhpConfigurationSection["QhpXmlFileExt"]) : ".xml";
            qhptargetNameSpace = (!string.IsNullOrEmpty(Convert.ToString(qhpConfigurationSection["QhpTargetNameSpace"]))) ? Convert.ToString(qhpConfigurationSection["QhpTargetNameSpace"]) : "http://vo.ffe.cms.hhs.gov";
            
        }
        #endregion Public Methods



        #region Private Methods

        #endregion Private Methods



        #region Helper Methods

        #endregion Helper Methods

    }
}