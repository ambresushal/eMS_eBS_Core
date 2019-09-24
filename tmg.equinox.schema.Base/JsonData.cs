using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using tmg.equinox.schema.Base.Interface;
using tmg.equinox.schema.Base;
using tmg.equinox.schema.Base.Model;
using System.IO;
using tmg.equinox.config;
using tmg.equinox.core.logging.Logging;

namespace tmg.equinox.schema.Base
{
    public class JsonData : IJsonData
    {
        JObject jsonData = new JObject();
        private static readonly ILog _logger = LogProvider.For<JsonData>();
        public JsonData()
        {
            LoadTemplate();
        }
        public JsonData(JData jsonTableData)
        {
            LoadTemplate(jsonTableData);
        }
        private void LoadTemplate(JData jsonTableData)
        {
            jsonData = JObject.Parse(jsonTableData.FormData);
            _logger.Debug("Form data JSON template loaded.");
        }
        private void LoadTemplate()
        {
            jsonData = JObject.Parse(File.ReadAllText(@Config.JsonDataTemplatePath));
        }

        public string GetContractNumber()
        {
            List<JToken> jtContractNumber = jsonData.FindTokens("ContractNumber");
            if (jtContractNumber.Count > 0)
                return jtContractNumber[0].ToString();
            else
                return "";
        }
        public JToken GetTableData(string tableName, string documentPath)
        {
            try
            {
                //_logger.Debug("Get table [" + tableName + "]data from JSON.");
                if (jsonData.FindTokens(tableName).Count <= 0)
                {
                    var jdata = jsonData.SelectToken(documentPath);
                    if (jdata != null)
                    {
                        return jdata;
                        
                    }
                    return null;
                }
                else
                {
                    if (jsonData.FindTokens(tableName).ToList().Count == 1)
                        return jsonData.FindTokens(tableName).ToList().First();
                    else
                        return jsonData.SelectToken(documentPath);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Table [" + tableName + "]Could not find into JSON. See more information : ", ex);
                throw ex;
            }
        }
        public bool ifRecordsExists(JToken jToken)
        {
            try
            {
                var row = jToken[0];
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public int GetRowCount(string tableName)
        {
            _logger.Debug("Get table row count from JSON.");
            return jsonData.FindTokens(tableName).Count();
        }


    }
}
