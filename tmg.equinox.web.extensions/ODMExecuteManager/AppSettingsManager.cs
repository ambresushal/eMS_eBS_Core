using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.ODMExecuteManager
{
    public static class AppSettingsManager
    {
        public static string AccessFilePath
        {
            get
            {
                string path = "";
                if (ConfigurationManager.AppSettings["ODMAccessFilePath"] != null)
                {
                    path = ConfigurationManager.AppSettings["ODMAccessFilePath"].ToString();
                }
                return path;
            }
        }

        public static string[] GetViewTypes()
        {
            return new string[] { "PBP", "SOT" };
        }
    }
    public enum MigrationBatchStatus
    {
        Queued,
        InProgress,
        Completed,
        Fail
    }
}
