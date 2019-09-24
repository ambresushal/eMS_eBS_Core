using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.document.rulebuilder.globalUtility;


namespace tmg.equinox.web.Framework.Caching
{
    public class FormInstanceDataCacheHandler : IDataCacheHandler
    {
        public void Add(int formInstanceId, int? userId)
        {
            throw new NotImplementedException();
        }

        public string Get(int tenantId, int formInstanceId, int? userId)
        {
            throw new NotImplementedException();
        }

        public string IsExists(int tenantId, int formInstanceId, int? userId)
        {
            throw new NotImplementedException();
        }

        public string GetSection(int formInstanceId, string sectionName, int? userId)
        {
            string sectionData = null;
            Dictionary<string, object> dataDict = new Dictionary<string, object>();
            string filePath = RuleEngineGlobalUtility.GetFilePath(formInstanceId);
            string formData = System.IO.File.ReadAllText(filePath);
            dataDict.Add(sectionName, JObject.Parse(formData)[sectionName]);
            sectionData = JsonConvert.SerializeObject(dataDict);

            return sectionData;
        }

        public bool Remove(int formInstanceId, int? userId)
        {
            throw new NotImplementedException();
        }

        public string UpdateSection(int formInstanceId, string sectionName, string sectionData, int? userId)
        {
            throw new NotImplementedException();
        }
    }
}