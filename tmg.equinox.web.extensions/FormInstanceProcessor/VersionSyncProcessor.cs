using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using tmg.equinox.web.VersionManagement;

namespace tmg.equinox.web.FormInstanceProcessor
{
    public class VersionSyncProcessor
    {
        private string _sectionName;
        private string _defualtJSONData;
        private string _sectionData;
        public VersionSyncProcessor(string sectionName, string defualtJSONData, string sectionData)
        {
            this._sectionName = sectionName;
            this._defualtJSONData = defualtJSONData;
            this._sectionData = sectionData;
        }

        public string Run()
        {
            string result = VersionSync();
            return result;
        }

        private string VersionSync()
        {
            string result = "";

            string defaultSectionData = JsonConvert.SerializeObject(JObject.Parse(_defualtJSONData)[_sectionName]);
            if (defaultSectionData == "null")
                throw new System.Exception($"[{_sectionName}] section does not exist.");

            JObject dataJObj = JObject.Parse("{'" + _sectionName + "':[]}");
            dataJObj[_sectionName] = JObject.Parse(defaultSectionData);

            defaultSectionData = JsonConvert.SerializeObject(dataJObj);

            VersionDataSynchronizer syncMgr = new VersionDataSynchronizer(_sectionData, defaultSectionData);

            if (syncMgr.isSyncRequired() == true)
            {
                result = syncMgr.Synchronize();
            }
            else
            {
                result = _sectionData;
            }

            return result;
        }
    }
}