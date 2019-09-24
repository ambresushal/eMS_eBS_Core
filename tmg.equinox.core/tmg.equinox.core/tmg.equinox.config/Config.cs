using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.config
{
    public class Config
    {
        public static string ImagePath
        {
            get
            {
                return ConfigurationManager.AppSettings["ImagePath"].ToString();
            }

        }
        public static string MappingSheet
        {
            get
            {
                return ConfigurationManager.AppSettings["MappingSheet"].ToString();
            }

        }
        public static string LogPath
        {
            get
            {
                return ConfigurationManager.AppSettings["LogPath"].ToString();
            }

        }
        public static string JsonTemplatePath
        {
            get
            {
                return ConfigurationManager.AppSettings["JsonTemplatePath"].ToString();
            }

        }
        public static string JsonDataTemplatePath
        {
            get
            {
                return ConfigurationManager.AppSettings["JsonDataTemplatePath"].ToString();
            }

        }
        public static string JsonData
        {
            get
            {
                return ConfigurationManager.AppSettings["JsonData"].ToString();
            }

        }
        public static string SourceSheetPath
        {
            get
            {
                return ConfigurationManager.AppSettings["SourceSheetPath"].ToString();
            }

        }
        public static string FolderLocation
        {
            get
            {
                return ConfigurationManager.AppSettings["FolderLocation"].ToString();
            }

        }
        public static string SuperUserRoleName()
        {
            string roleName = ConfigurationManager.AppSettings["SuperUserRoleName"];
            if (string.IsNullOrEmpty(roleName))
            {
                roleName = "TMG SuperUser";
            }
            return roleName.ToString();
        }
        public static string TMGDB()
        {
            if (ConfigurationManager.ConnectionStrings["TMGDB"] == null)
                return "TMGDB";
            else
                return ConfigurationManager.ConnectionStrings["TMGDB"].ToString();
        }
        public static string GetReportingCenterConnectionString()
        {
            if (ConfigurationManager.ConnectionStrings["ReportingCenterContext"] == null)
                return "ReportingCenterContext";
            else
                return ConfigurationManager.ConnectionStrings["ReportingCenterContext"].ToString();
        }

        public static string GetODMAccessConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ODMAccessConnectionString"].ToString();
        }

        public static string GetReportingCenterConnectionStringKey()
        {
            return "ReportingCenterContext";
        }
        /// <summary>
        /// Added new method to get the SQL Server Instance name from configuration file
        /// </summary>
        /// <returns></returns>
        public static string GetServerInstanceName()
        {
            return ConfigurationManager.AppSettings["ServerInstance"].ToString();
        }

        /// <summary>
        /// Added new method to get the UserName to connect the SQL server
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            return ConfigurationManager.AppSettings["UserName"].ToString();
        }

        /// <summary>
        /// Added new method to get the Password to connect the SQL server
        /// </summary>
        /// <returns></returns>
        public static string GetPassword()
        {
            return ConfigurationManager.AppSettings["Password"].ToString();
        }

        /// <summary>
        /// Added new method to get the reporting database name from configuration file
        /// </summary>
        /// <returns></returns>
        public static string GetReportingDatabaseName()
        {
            return ConfigurationManager.AppSettings["ReportingDBName"].ToString();
        }

        public static string GetHangFireQueues()
        {            
            var queueNameList = ConfigurationManager.AppSettings["HangFireQueues"];
            string hangFireQueues = string.Empty;
            if (queueNameList != null)
            {
                hangFireQueues = queueNameList.ToString() ?? string.Empty;
            }
            return hangFireQueues;
        }

        public static string GetHangfireServiceName()
        {
            var queueNameList = ConfigurationManager.AppSettings["hangfireservicename"];
            string hangFireQueues = "eBenefitSyncBackgoundService";
            if (queueNameList != null)
            {
                hangFireQueues = queueNameList.ToString() ?? "eBenefitSyncBackgoundService";
            }
            return hangFireQueues;
        }

        public static bool ContainHangFireQueues(string queueName)
        {
            var queueNameList = ConfigurationManager.AppSettings["HangFireQueues"];
            string hangFireQueues = string.Empty;
            if (queueNameList != null)
            {
                return queueNameList.ToString().ToLower().Contains(queueName.ToLower());
            }
            else
            {
                return false;
            }
        }


        public static string GetApplicationName()
        {
            var appNameSetting = ConfigurationManager.AppSettings["ApplicationName"];
            string appName = "eMedicareSync";
            if (appNameSetting != null)
            {
                appName = appNameSetting.ToString() ?? appName;
            }
            return appName;
        }
    }
}
