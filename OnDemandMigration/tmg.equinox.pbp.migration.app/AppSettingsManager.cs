using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.pbp.migration.app
{
    public static class AppSettingsManager
    {

        public static string AccessFilePath
        {
            get
            {
                string path = "";
                if (ConfigurationManager.AppSettings["MigrationFilesPath"] != null)
                {
                    path = ConfigurationManager.AppSettings["MigrationFilesPath"].ToString();
                }
                return path;
            }
        }
    }
}
