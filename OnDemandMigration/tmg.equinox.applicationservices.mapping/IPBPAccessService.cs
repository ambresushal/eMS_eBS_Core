using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.pbp
{
    public interface IPBPAccessService
    {
        JObject GetQIDBenefitData(string QID, string mdbFilePath);
        List<String> GetQIDList(string mdbFilePath);

        JObject GetQIDData(string mdbFilePath, List<PBPTable> tables, string QID);

        List<AccessDatabaseTableInfo> GetAccessDatabaseTables(string mdbFilePath);
    }
}
