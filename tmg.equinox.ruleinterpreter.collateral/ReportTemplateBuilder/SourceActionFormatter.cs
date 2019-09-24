using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Helper;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class SourceActionFormatter
    {

        private JToken _source;
        string _sourceAction;
        JToken sourceResult;
        JToken _sourcesContainer;
        public SourceActionFormatter(JToken sourcesContainer, JToken source, string sourceAction)
        {
            _source = source;
            _sourceAction = sourceAction;
            _sourcesContainer = sourcesContainer;
        }

        public JToken FormatSource()
        {
            if (!string.IsNullOrEmpty(_sourceAction))
            {
                string actionType = GetSourceActionType();

                switch (actionType)
                {

                    case "UNPIVOT":
                        {
                            sourceResult = UnPivotFormatter();
                        }
                        break;

                }
            }
            return sourceResult;
        }


        private string UnPivotTargetElement()
        {
            string actionColumn = string.Empty;
            if (_sourceAction.Contains('['))
            {
                int startIndex;
                int endIndex;
                startIndex = _sourceAction.IndexOf("[") + 1;
                endIndex = _sourceAction.IndexOf("]", startIndex);
                actionColumn = _sourceAction.Substring(startIndex, endIndex - startIndex).Trim();
            }
            return actionColumn;
        }

        private string GetSourceActionType()
        {
            string actionType = string.Empty;
            if (_sourceAction.Contains('['))
            {
                int startIndex = 0;
                int endIndex;
                endIndex = _sourceAction.IndexOf("[");
                actionType = _sourceAction.Substring(startIndex, endIndex - startIndex).Trim();
            }
            return actionType;
        }


        private JToken UnPivotFormatter()
        {

            string targetActions = UnPivotTargetElement();
            string[] targetElements = targetActions.Split(',');
            JToken sourceToMerge = null;
            string columnToUnpivot = targetElements[0];
            string sourceNameToMerge = targetElements.Count() > 1 ? targetElements[1] : string.Empty;
            if (!string.IsNullOrEmpty(sourceNameToMerge))
                sourceToMerge = _sourcesContainer.SelectToken(sourceNameToMerge);


            JArray result = new JArray();
            JArray unPivotResult = new JArray();
            foreach (JToken item in _source)
            {
                JToken sourceRow = item.DeepClone();
                JArray targetToken = new JArray();
                JToken childObject = sourceRow.SelectToken(columnToUnpivot);
                JObject parentObject = (JObject)sourceRow.DeepClone();
                parentObject.Remove(columnToUnpivot);

                JArray sourceToken = childObject as JArray;
                targetToken.Add(parentObject);
                if (sourceToken != null && targetToken != null && sourceToken.HasValues && targetToken.HasValues && sourceToken.Count > 0 && targetToken.Count > 0)
                {
                    JToken crossjoin = JsonParserHelper.CrossJoin(sourceToken, targetToken);
                    unPivotResult = new JArray(unPivotResult.Union(crossjoin));
                }

            }

            if (sourceToMerge != null && !string.IsNullOrEmpty(sourceNameToMerge) && unPivotResult != null && unPivotResult.Count() >0)
            {
                List<string> dataCols = GetColumns(unPivotResult.First());
                List<string> contextCols = GetColumns(sourceToMerge.First());
                List<string> keys = dataCols.Intersect(contextCols, StringComparer.OrdinalIgnoreCase).Where(a => a != "SequenceNo" && a != "RowIDProperty" && a != "Key" && a != "FilterExpression").ToList();
                JArray sourceArray = sourceToMerge as JArray;
                JArray targetArray = unPivotResult;
                result = JsonParserHelper.MergeArray(targetArray, sourceArray, keys);
            }
            else
            {
                result = unPivotResult;
            }
            return result;
        }

        private List<string> GetColumns(JToken token)
        {
            List<string> cols = new List<string>();
            foreach (var tok in token.Children())
            {
                string colName = ((JProperty)tok).Name;
                if (colName != "RowIDProperty")
                {
                    cols.Add(colName);
                }
            }
            return cols;
        }
    }
}
