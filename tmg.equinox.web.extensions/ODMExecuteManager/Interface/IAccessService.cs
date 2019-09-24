using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.web.ODMExecuteManager.Model;

namespace tmg.equinox.web.ODMExecuteManager.Interface
{
    public interface IAccessService
    {
        JObject GetQIDBenefitData(string QID, string mdbFilePath);
        List<string> GetQIDList(string mdbFilePath);
        JObject GetQIDData(string mdbFilePath, List<TableInfo> tables, string QID);
    }
}
