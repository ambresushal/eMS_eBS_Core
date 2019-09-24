using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tmg.equinox.document.rulebuilder.model;

namespace tmg.equinox.document.rulebuilder.globalUtility
{
    public static class RuleEngineGlobalUtility
    {

        public static string _filePath = Path.GetDirectoryName(Application.ExecutablePath).Replace("\\bin\\Debug", string.Empty) + "\\JSONSources";

        public static int  formdesignversionID;
        public static string formdesignversionData;

        public static Dictionary<string, string> StringItemsToDictionary(string keyColumnString)
        {
            return !string.IsNullOrEmpty(keyColumnString) ? keyColumnString.Split(',').ToDictionary(m => m.ToString(), m => m.ToString())
                     : new Dictionary<string, string>();
        }

        public static T Clone<T>(T source)
        {
            var json = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(json);
        }

        //TODO :PreIntegration Fix
        public static string GetFilePath(int formInstanceId)
        {
            string fileName = Directory.GetFiles(_filePath).Select(x => Path.GetFileNameWithoutExtension(x.ToString()))
                              .Where(whr => whr.ToString().Contains('_') && whr.ToString().Split('_')[1] == formInstanceId.ToString())
                              .Select(sel => sel.ToString()).FirstOrDefault();

            return _filePath + "\\" + fileName + ".json";
        }

        public static Dictionary<string, List<KeyValuePair<string, string>>> GetColumnMappings(string mappingString)
        {
            Dictionary<string, List<KeyValuePair<string, string>>> mappingDictionary = new Dictionary<string, List<KeyValuePair<string, string>>>();

            if (!string.IsNullOrEmpty(mappingString))
            {
                List<string> sourceColumns = mappingString.Split(',').Select(sel => sel).ToList();

                mappingDictionary = (from source in sourceColumns
                                     select new
                                     {
                                         SourceName = source.Substring(source.IndexOf('@') + 1, source.IndexOf('.') - 1),
                                         ColumnName = source.Contains(':') ? source.Substring(source.IndexOf('.') + 1, source.IndexOf(':')) : source.Substring(source.IndexOf('.') + 1),
                                         ColumnValue = source.Contains(':') ? source.Substring(source.IndexOf(':') + 1) : string.Empty
                                     }).GroupBy(m => m.SourceName)
                                       .ToDictionary(x => x.Key, x => x.ToList().Select(sel => new KeyValuePair<string, string>(sel.ColumnName, sel.ColumnValue)).ToList());
            }
            return mappingDictionary;
        }

        public static JObject CreateRowObject(List<KeyValuePair<string, string>> columns)
        {
            string jtokenString = string.Empty;
            for (int i = 0; i < columns.Count; i++)
            {
                jtokenString = jtokenString + "'" + columns[i].Key + "':'" + columns[i].Value + "'";
                jtokenString = ((columns.Count == 1) || (i == columns.Count - 1)) ? jtokenString : jtokenString + ",";
            }
            jtokenString = "{" + jtokenString + "}";
            JObject jObject = JObject.Parse(jtokenString);
            return jObject;
        }

        public static JObject CopyJObjectFromSourceToTarget(JObject source, JObject target)
        {
            var dict = source as IDictionary<string, JToken>;
            var targetDict = target as IDictionary<string, JToken>;
            foreach (var key in dict.Keys)
            {
                if (targetDict.ContainsKey(key))
                {
                    targetDict[key] = dict[key].ToString();
                }
            }
            return target;
        }

        public static List<JToken> GetAdHocColumnDetails(this List<JToken> copyFrom, List<KeyValuePair<string, string>> columnToSelect)
        {
            List<JToken> selectedColumnDetails = new List<JToken>();
            foreach (JObject ser in copyFrom)
            {
                JObject selectedColumnObject = RuleEngineGlobalUtility.CreateRowObject(columnToSelect);
                RuleEngineGlobalUtility.CopyJObjectFromSourceToTarget(ser, selectedColumnObject);
                selectedColumnDetails.Add(selectedColumnObject);
            }
            return selectedColumnDetails;
        }

        public static ExecutionType GetExecutionType(RuleOperatorType ruleOperator)
        {
            ExecutionType executionType = ExecutionType.none;
            if (RuleOperatorType.crossjoin == ruleOperator)
            {
                executionType = ExecutionType.crossjoin;
            }

            else if (RuleOperatorType.crossjoin == ruleOperator || RuleOperatorType.intersect == ruleOperator || RuleOperatorType.union == ruleOperator || RuleOperatorType.coljoin == ruleOperator
                    || RuleOperatorType.except == ruleOperator)
            {
                executionType = ExecutionType.collectioncomparer;
            }

            else if (RuleOperatorType.contains == ruleOperator || RuleOperatorType.greaterthan == ruleOperator || RuleOperatorType.lessthan == ruleOperator
                       || RuleOperatorType.greaterthanequalto == ruleOperator || RuleOperatorType.lessthanequalto == ruleOperator || RuleOperatorType.equalto == ruleOperator
                       || RuleOperatorType.distinct == ruleOperator || RuleOperatorType.notequalto == ruleOperator)
            {
                executionType = ExecutionType.collectionvaluecomparer;
            }
            return executionType;
        }

        public static RuleExpressionInput GetSourceProcessInput(string outputPropertyName, Dictionary<string, RuleFilterExpression> ruleExpression, Dictionary<string, string> keyColumns, Dictionary<string, JToken> ruleSources, FilterType filterType,OutputProperties outputFormat)
        {
            RuleExpressionInput mergeActionProcessInput = new RuleExpressionInput();
            mergeActionProcessInput.ExpressionKeyName = outputPropertyName;
            mergeActionProcessInput.SourceItemDictionary = ruleSources;
            mergeActionProcessInput.SourceMergeExpression = ruleExpression;
            mergeActionProcessInput.KeyColumns = keyColumns;
            mergeActionProcessInput.FilterType = filterType;
            mergeActionProcessInput.outputFormat = outputFormat;

            return mergeActionProcessInput;
        }
        
        private static List<JToken> UpdateRowIDProperty(List<JToken> Data)
        {
            int id = 0;
            foreach (JToken data in Data)
            {
                data["RowIDProperty"] = id;
                id++;
            }
            return Data;
        }
    }
}
