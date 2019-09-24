using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.PBPImport
{
    public class OleDbHelper
    {
        public string GetOleDbConnectingString(string mdfDbPath)
        {
            String Provider = ConfigurationManager.AppSettings["AccessDBProvider"].ToString();
            string strOledbConnectingString = String.Format(@Provider, mdfDbPath);
            return strOledbConnectingString;
        }
    }
}
