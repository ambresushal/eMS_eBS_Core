using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.pbpimportservices
{
    public class OleDbHelperClass
    {
        public string GetOleDbConnectingString(string mdfDbPath)
        {
            string strOledbConnectingString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + mdfDbPath + "; Persist Security Info=True;";
            return strOledbConnectingString;
        }
    }
}
