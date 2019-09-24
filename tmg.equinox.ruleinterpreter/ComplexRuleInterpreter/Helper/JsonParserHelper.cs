using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.operatorutility;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Helper
{
    public class JsonParserHelper
    {
        public static JToken CrossJ(JArray sequence1, JArray sequence2)
        {
            JToken ignoreList = JToken.Parse("{'RowIDProperty':'0'}");
            var combo = (from first in sequence1
                         from second in sequence2
                         select new JObject
                         {
                             first.Children().Except(ignoreList),
                             second.Children()
                         }).ToList();

            JToken result = JToken.FromObject(combo);

            return result;
        }

        public static JToken CrossJoin(JArray sequence1, JArray sequence2)
        {
            List<string> uniqueProp = new List<string>();
            JArray result = new JArray();

            foreach (var s1 in sequence1)
            {
                foreach (var s2 in sequence2)
                {
                    JObject newRow = new JObject();
                    foreach (JProperty prop in s1)
                    {
                        if (!uniqueProp.Contains(prop.Name))
                        {
                            newRow.Add(prop);
                            uniqueProp.Add(prop.Name);
                        }
                    }

                    foreach (JProperty childProp in s2)
                    {
                        if (!uniqueProp.Contains(childProp.Name))
                        {
                            newRow.Add(childProp);
                        }
                    }
                    result.Add(newRow);
                    uniqueProp.RemoveRange(0, uniqueProp.Count);
                }
            }
            return result;
        }

        public static JArray MergeArray(JToken defaultRow, JArray data, string syncAllCols, string columnMaps = "")
        {
            JArray mergedArray = new JArray();
            JToken row = defaultRow.DeepClone();
            EmptyDefaultRow(row);
            Dictionary<string, string> mapDict = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(columnMaps))
            {
                List<string> colMaps = columnMaps.Split(',').ToList();
                foreach (string col in colMaps)
                {
                    if (col.IndexOf(':') > -1)
                    {
                        string[] colMap = col.Split(':');
                        mapDict.Add(colMap[0], colMap[1]);
                    }
                    else
                    {
                        mapDict.Add(col, col);
                    }
                }
            }
            foreach (JObject content in data)
            {
                JToken emptyRow = row.DeepClone();
                foreach (JProperty prop in content.Children())
                {
                    if (!String.IsNullOrEmpty(columnMaps) && mapDict.ContainsKey(prop.Name) == true)
                    {
                        emptyRow[mapDict[prop.Name]] = prop.Value;
                    }
                    else
                    {
                        if (syncAllCols == "true")
                        {
                            if (emptyRow[prop.Name] != null)
                            {
                                emptyRow[prop.Name] = prop.Value;
                            }
                        }
                    }
                }
                mergedArray.Add(emptyRow);
            }

            return mergedArray;
        }

        public static JArray MergeArray(JArray sourceOne, JArray sourceTwo, List<string> keys)
        {
            JArray result = new JArray();
            foreach (var row in sourceOne)
            {
                JToken mergedRow = row.DeepClone();
                var targetRow = sourceTwo.Intersect(new JArray() { mergedRow }, new ObjectEqualityComparer(keys)).FirstOrDefault();
                if (targetRow != null)
                {
                    foreach (JProperty prop in targetRow.Children())
                    {
                        if (mergedRow[prop.Name] == null)
                        {
                            mergedRow[prop.Name] = prop.Value;
                        }
                    }
                }

                result.Add(mergedRow);
            }

            return result;
        }

        public static JToken UnPivotArray(JToken source, string columnToUnpivot)
        {
            JArray result = new JArray();
            JArray unPivotResult = new JArray();
            foreach (JToken item in source)
            {
                JToken sourceRow = item.DeepClone();
                JArray targetToken = new JArray();
                JToken childObject = sourceRow.SelectToken(columnToUnpivot);
                JObject parentObject = (JObject)sourceRow.DeepClone();
                parentObject.Remove(columnToUnpivot);

                JArray sourceToken = childObject as JArray;
                
                targetToken.Add(parentObject);
                if (sourceToken != null && targetToken != null && sourceToken.HasValues  && targetToken.HasValues && sourceToken.Count >0 && targetToken.Count > 0)
                {
                    JToken crossjoin = JsonParserHelper.CrossJoin(sourceToken, targetToken);
                    unPivotResult = new JArray(unPivotResult.Union(crossjoin));
                }

            }
            return unPivotResult;
        }
        private static void EmptyDefaultRow(JToken defaultRow)
        {
            foreach (JProperty item in defaultRow)
            {
                item.Value = "";
            }
        }
    }
}
