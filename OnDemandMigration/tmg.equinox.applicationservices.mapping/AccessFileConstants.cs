using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace tmg.equinox.applicationservices.pbp
{
    public class AccessFileConstants
    {
        public static string SELECTQUERY = "SELECT {0} FROM {1} WHERE {2}";
        public static string QIDTABLE = "PBP";
        public static string QIDCOLUMN = "QID";
    }

    public static class DMUtilityLog
    {
        static string DMUtilityLogPath = "DMUtilityLog";
        static string TimeStampFolderPath = DMUtilityLogPath + "\\" + DateTime.Now.ToString().Replace("-", "").Replace(":", "").Replace(" ", "_");
        public static void WriteDMUtilityLog(string qID, string viewType, string Message)
        {

            string DMUtilityLogFilePath = TimeStampFolderPath + "\\" + qID + viewType + ".CSV";

            if (!Directory.Exists(DMUtilityLogPath))
                Directory.CreateDirectory(DMUtilityLogPath);

            if (!Directory.Exists(TimeStampFolderPath))
                Directory.CreateDirectory(TimeStampFolderPath);

            if (Message == null)
                Message = "MDB Table Name,MDB Column Name,JsonPath,MDB Value";

            using (StreamWriter writer = new StreamWriter(DMUtilityLogFilePath,true))
            {
                writer.WriteLine(Message);
            }

        }
    }
}
