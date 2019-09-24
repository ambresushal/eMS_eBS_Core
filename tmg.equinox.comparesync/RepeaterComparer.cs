using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.documentcomparer.RepeaterCompareUtils;

namespace tmg.equinox.documentcomparer
{
    public class RepeaterComparer
    {

        private JToken fieldsMacro;
        private JToken keysMacro;
        private JToken sourceRepeater;
        private JToken targetRepeater;
        private string rootSection;
        private List<string> parentSections;
        private string matchType;
        public RepeaterComparer(JToken fieldsMacro, JToken keysMacro, JToken sourceRepeater, JToken targetRepeater,Dictionary<string,string> labelPathMap,string matchType) 
        {
            this.fieldsMacro = fieldsMacro;
            this.keysMacro = keysMacro;
            this.sourceRepeater = sourceRepeater;
            this.targetRepeater = targetRepeater;
            string path = "";
            if (fieldsMacro != null)
            {
                path = fieldsMacro.Path;
            }
            else
            {
                path = keysMacro.Path;
            }
            string[] parts = path.Split('.');
            this.rootSection = labelPathMap[parts[0]];
            this.matchType = matchType;
            var index = 0;
            parentSections = new List<string>();
            string parentPath = parts[0];
            foreach (string s in parts)
            {
                if (index > 0 && index < parts.Length - 1)
                {
                    parentPath = parentPath + "." + s;
                    this.parentSections.Add( labelPathMap[parentPath]);
                }
                index++;
            }
        }

        public RepeaterCompareResult Compare() 
        {
            RepeaterCompareResult result = new RepeaterCompareResult();
            result.ParentSections = this.parentSections;
            result.RootSectionName = this.rootSection;
            if (this.fieldsMacro != null)
            {
                result.Path = this.fieldsMacro.Parent.Path;
            }
            else {
                result.Path = this.keysMacro.Parent.Path;
            }
            result.RepeaterName = ((JProperty)this.keysMacro.Parent.Parent).Name;
            InitKeysMacro();
            result.Keys = GetKeys();
            if (this.fieldsMacro != null)
            {
                result.Fields = GetFields();
            }
            result.Rows = new List<RepeaterCompareRow>();
            if ((sourceRepeater != null && sourceRepeater.Count() > 0) && (targetRepeater != null))
            {
                List<JToken> sourceItems = null;
                if (sourceRepeater.Type == JTokenType.Array)
                {
                    sourceItems = ((JArray)sourceRepeater).ToList();
                }
                List<JToken> targetItems = null; 
                if(targetRepeater.Type == JTokenType.Array)
                {
                    targetItems = ((JArray)targetRepeater).ToList();
                }

                if ((sourceItems != null && sourceItems.Count > 0) || (targetItems != null && targetItems.Count > 0))
                {
                    JToken repeaterFirstRow = null;
                    if (sourceItems.Count > 0) 
                    {
                        repeaterFirstRow = sourceItems.First();
                    }
                    else 
                    {
                        repeaterFirstRow = targetItems.First();
                    }
                    //get primary keys

                    RepeaterCompareCriteria criteria = GetCompareCriteria(repeaterFirstRow);
                    result.ChildContainerName = criteria.ChildContainerName;

                    List<JToken> targetMatches = null, sourceMatches = null;
                    //get source primary rows match
                    if (sourceRepeater != null &&sourceRepeater.Count() >0)
                    {
                        sourceMatches = RepeaterMatchUtil.GetMatchesInRepeater(sourceRepeater, criteria.PrimaryKeysMacro, "Source");
                    }
                    //get target primary rows match
                    if (targetRepeater != null && targetRepeater.Count() > 0)
                    {
                        targetMatches = RepeaterMatchUtil.GetMatchesInRepeater(targetRepeater, criteria.PrimaryKeysMacro, "Target");
                    }
                    

                    //get rows in source only, target only and source/target matches
                    List<JToken> sourcePrimaryOnly = new List<JToken>();
                    List<JToken> targetPrimaryOnly = new List<JToken>();
                    Dictionary<JToken, JToken> sourceAndTargetPrimary = new Dictionary<JToken, JToken>();


                    //compare row by row and generate result
                    if (sourceMatches.Count > 0 || targetMatches.Count > 0)
                    {
                        //source only and matches
                        foreach (JToken sourceMatch in sourceMatches)
                        {
                            JToken targetMatch = RepeaterMatchUtil.GetMatchingRowFromTargetRepeater(sourceMatch, targetMatches, criteria.PrimaryKeysMacro,"Source");
                            if (targetMatch == null)
                            {
                                sourcePrimaryOnly.Add(sourceMatch);
                            }
                            else
                            {
                                sourceAndTargetPrimary.Add(sourceMatch, targetMatch);
                            }
                        }
                        //target only
                        if (targetMatches != null)
                        {
                            foreach (JToken targetMatch in targetMatches)
                            {
                                JToken sourceMatch = RepeaterMatchUtil.GetMatchingRowFromTargetRepeater(targetMatch, sourceMatches, criteria.PrimaryKeysMacro, "Target");
                                if (sourceMatch == null)
                                {
                                    targetPrimaryOnly.Add(targetMatch);
                                }
                            }
                        }

                        //get Compare results
                        result.Rows.AddRange(CompareRows(sourceAndTargetPrimary, criteria,"Primary"));
                        if (matchType == "Mismatches only" || matchType == "Both")
                        {
                            result.Rows.AddRange(CompareRows(sourcePrimaryOnly, criteria, "Source", "Primary"));
                            result.Rows.AddRange(CompareRows(targetPrimaryOnly, criteria, "Target", "Primary"));
                        }
                    }
                }
                else
                {
                    if (sourceItems == null)
                    {
                        result.IsRepeaterMissingInSource = true;
                    }
                    if (targetItems == null)
                    {
                        result.IsRepeaterMissingInTarget = true;
                    }

                }
            }
            else 
            {
                if (sourceRepeater == null) 
                {
                    result.IsRepeaterMissingInSource = true;
                }
                if (targetRepeater == null)
                {
                    result.IsRepeaterMissingInTarget = true;
                }
            }
            SetSyncAndMatchForRepeater(result);
            return result;
        }

        private List<string> GetChildKeys(JToken sourceRow)
        {
            List<string> childKeys = new List<string>();
            List<JToken> keys = keysMacro.Values().ToList();
            foreach (JToken key in keys) 
            {
                string keyValue = key["Key"].ToString();
                if (sourceRow[keyValue] == null) 
                {
                    childKeys.Add(keyValue);
                }
            }
            return childKeys;
        }

        private bool DoChildKeysHaveValues(JToken childKeysMacro)
        {
            List<JToken> keys = childKeysMacro.Values().ToList();
            bool hasValue = false;
            foreach (JToken key in keys)
            {
                string keyValue = key["Key"].ToString();
                if(!String.IsNullOrEmpty(key["SourceValue"].ToString()) || !String.IsNullOrEmpty(key["TargetValue"].ToString()))
                {
                    hasValue = true;
                    break;
                }
            }
            return hasValue;
        }


        private List<RepeaterCompareKey> GetKeys()
        {
            List<RepeaterCompareKey> allKeys = new List<RepeaterCompareKey>();
            List<JToken> keys = keysMacro.Values().ToList();
            foreach (JToken key in keys)
            {
                RepeaterCompareKey compareKey = new RepeaterCompareKey();
                compareKey.KeyName = key["Key"].ToString();
                compareKey.KeyLabel = key["Label"].ToString();
                compareKey.SourceKey = key["SourceValue"].ToString();
                compareKey.TargetKey = key["TargetValue"].ToString();
                allKeys.Add(compareKey);
            }
            return allKeys;
        }

        private List<RepeaterCompareField> GetFields()
        {
            List<RepeaterCompareField> allFields = new List<RepeaterCompareField>();
            List<JToken> fields = fieldsMacro.Values().ToList();
            foreach (JToken field in fields)
            {
                RepeaterCompareField compareField = new RepeaterCompareField();
                compareField.FieldName = field["Field"].ToString();
                allFields.Add(compareField);
            }
            return allFields;
        }


        private JToken GetChildKeysMacro(JToken sourceRow)
        {
            JToken childKeysMacro = keysMacro.DeepClone();
            List<JToken> keys = keysMacro.Values().ToList();
            foreach (JToken key in keys)
            {
                string keyValue = key["Key"].ToString();
                if (sourceRow[keyValue] != null)
                {
                    var keyObj = childKeysMacro.First().SelectToken("$[?(@.Key == '" + keyValue + "')]");
                    if (keyObj != null)
                    {
                        keyObj.Remove();
                    }
                }
            }
            return childKeysMacro;
        }

        private List<string> GetPrimaryKeys(JToken sourceRow)
        {
            List<string> primaryKeys = new List<string>();
            List<JToken> keys = keysMacro.Values().ToList();
            foreach (JToken key in keys)
            {
                string keyValue = key["Key"].ToString();
                if (sourceRow[keyValue] != null)
                {
                    primaryKeys.Add(keyValue);
                }
            }
            return primaryKeys;
        }

        private JToken GetPrimaryKeysMacro(JToken sourceRow)
        {
            JToken primaryKeysMacro = keysMacro.DeepClone();
            List<JToken> keys = keysMacro.Values().ToList();
            foreach (JToken key in keys)
            {
                string keyValue = key["Key"].ToString();
                if (sourceRow[keyValue] == null)
                {
                    var keyObj = primaryKeysMacro.First().SelectToken("$[?(@.Key == '" + keyValue + "')]");
                    if (keyObj != null) 
                    {
                        keyObj.Remove();
                    }
                }
            }
            return primaryKeysMacro;
        }

        private List<string> GetPrimaryFields(JToken sourceRow) 
        {
            List<string> primaryFields = new List<string>();
            List<JToken> fields = fieldsMacro.Values().ToList();
            foreach (JToken field in fields)
            {
                string fieldValue = field["Field"].ToString();
                if (sourceRow[fieldValue] != null)
                {
                    primaryFields.Add(fieldValue);
                }
            }
            return primaryFields;
        }

        private List<string> GetChildFields(JToken sourceRow)
        {
            List<string> childFields = new List<string>();
            List<JToken> fields = fieldsMacro.Values().ToList();
            foreach (JToken field in fields)
            {
                string fieldValue = field["Field"].ToString();
                if (sourceRow[fieldValue] == null)
                {
                    childFields.Add(fieldValue);
                }
            }
            return childFields;
        }

        private int GetTypeOfMatch(List<string> primaryFields,List<string> childFields)
        {
            int typeOfMatch = 0;
            if (childFields.Count == 0)
            {
                typeOfMatch = 1;
            }
            //primary and child
            if (primaryFields.Count > 0 && childFields.Count > 0)
            {
                typeOfMatch = 2;
            }
            //child
            if (primaryFields.Count == 0 && childFields.Count > 0)
            {
                typeOfMatch = 3;
            }
            return typeOfMatch;
        }

        private string GetChildContainerName(JToken sourceRepeaterFirstRow)
        {
            string childGridContainerName = "";
            var childTokens = sourceRepeaterFirstRow.Children();
            foreach (JToken token in childTokens)
            {
                string tokenType = token.First.Type.ToString();
                if (tokenType == "Array")
                {
                    childGridContainerName = ((JProperty)token).Name;
                }
            }
            return childGridContainerName;
        }

        private RepeaterCompareCriteria GetCompareCriteria(JToken repeaterFirstRow)
        {
            List<string> primaryKeys = GetPrimaryKeys(repeaterFirstRow);
            //get primary keys macro
            JToken primaryKeysMacro = GetPrimaryKeysMacro(repeaterFirstRow);

            //get child keys
            List<string> childKeys = GetChildKeys(repeaterFirstRow);
            //get child keys macro
            JToken childKeysMacro = GetChildKeysMacro(repeaterFirstRow);

            //get primary fields
            List<string> primaryFields = new List<string>();
            List<string> childFields = new List<string>();
            int typeOfMatch = 0;
            if (this.fieldsMacro != null)
            {
                //get primary fields
                primaryFields = GetPrimaryFields(repeaterFirstRow);
                //get child fields
                childFields = GetChildFields(repeaterFirstRow);

                typeOfMatch = GetTypeOfMatch(primaryFields, childFields);
            }

            //get name of child grid container
            //ASSUMPTION - ONLY ONE CHILD for the grid - will need to be enhanced later
            string childGridContainerName = GetChildContainerName(repeaterFirstRow);

            bool hasChildKeyValue = DoChildKeysHaveValues(childKeysMacro);

            RepeaterCompareCriteria criteria = new RepeaterCompareCriteria();
            criteria.ChildFields = childFields;
            criteria.Childkeys = childKeys;
            criteria.ChildKeysMacro = childKeysMacro;
            criteria.PrimaryFields = primaryFields;
            criteria.PrimaryKeys = primaryKeys;
            criteria.PrimaryKeysMacro = primaryKeysMacro;
            criteria.ChildContainerName = childGridContainerName;
            criteria.TypeOfMatch = typeOfMatch;
            criteria.DoChildKeysHaveValues = hasChildKeyValue;
            return criteria;
        }
        /// <summary>
        /// compare rows where the source and target keys match
        /// </summary>
        /// <param name="matches">Matching rows in Source and Target</param>
        /// <param name="criteria">Criteria for compare</param>
        /// <param name="compareType">Primary or Child row</param>
        /// <returns></returns>
        private List<RepeaterCompareRow> CompareRows(Dictionary<JToken,JToken> matches, RepeaterCompareCriteria criteria,string compareType)
        {
            List<RepeaterCompareRow> rows = new List<RepeaterCompareRow>();
            foreach (var match in matches) 
            {
                JToken sourceRow = match.Key;
                JToken targetRow = match.Value;
                RepeaterCompareRow row = CompareRow(criteria,sourceRow,targetRow,compareType);
                switch (matchType)
                {
                    case "Mismatches only":
                        if (row.IsMatch == false)
                        {
                            rows.Add(row);
                        }
                        break;
                    case "Matches only":
                        if (row.IsMatch == true)
                        {
                            rows.Add(row);
                        }
                        break;
                    default:
                        rows.Add(row);
                        break;
                }
            }
            return rows;
        }

        /// <summary>
        /// Compare rows - for rows that have no matches - ie. only Source or only Target
        /// </summary>
        /// <param name="compareRows">Source or Target rows</param>
        /// <param name="criteria">Criteria for compare</param>
        /// <param name="compareSource">Source or Target row</param>
        /// <param name="compareType">Primary or Child row</param>
        /// <returns></returns>
        private List<RepeaterCompareRow> CompareRows(List<JToken> compareRows, RepeaterCompareCriteria criteria ,string compareSource,string compareType)
        {
            List<RepeaterCompareRow> rows = new List<RepeaterCompareRow>();
            foreach (var compareRow in compareRows)
            {
                JToken sourceRow = null;
                if (compareSource == "Source") 
                {
                    sourceRow = compareRow;
                }
                JToken targetRow = null;
                if (compareSource == "Target") 
                {
                    targetRow = compareRow;
                }
                RepeaterCompareRow row = CompareRow(criteria, sourceRow, targetRow,compareType);
                rows.Add(row);
            }
            return rows;
        }

        private RepeaterCompareRow CompareRow(RepeaterCompareCriteria criteria, JToken sourceRow, JToken targetRow, string compareType)
        {
            RepeaterCompareRow row = new RepeaterCompareRow();
            row.Keys = new List<RepeaterCompareKey>();
            CompareRow(criteria, sourceRow, targetRow, ref row, compareType);
            SetSyncAndMatchForPrimaryRow(ref row);
            return row;
        }

        private void CompareRow(RepeaterCompareCriteria criteria,JToken sourceRow, JToken targetRow, ref RepeaterCompareRow row, string rowCompareType)
        {
            JToken keysMacro = null;
            List<string> fields = null;
            if (rowCompareType == "Primary")
            {
                keysMacro = criteria.PrimaryKeysMacro;
                if (criteria.PrimaryFields != null && criteria.PrimaryFields.Count() > 0)
                {
                    fields = criteria.PrimaryFields;
                }
            }
            else
            {
                keysMacro = criteria.ChildKeysMacro;
                if (criteria.ChildFields != null)
                {
                    fields = criteria.ChildFields;
                }
            }

            foreach (JToken token in keysMacro.Values())
            {
                RepeaterCompareKey key = new RepeaterCompareKey();
                key.KeyName = token["Key"].ToString();
                if (sourceRow != null && sourceRow[key.KeyName] != null)
                {
                    key.SourceKey = sourceRow[key.KeyName].ToString();
                }
                else
                {
                    key.SourceKey = "Missing";
                    key.IsMissingInSource = true;
                    row.MissingRowInSource = true;

                }
                if (targetRow != null && targetRow[key.KeyName] != null)
                {
                    key.TargetKey = targetRow[key.KeyName].ToString();
                }
                else
                {
                    if (!string.IsNullOrEmpty(token["TargetValue"].ToString()))
                    {
                        key.TargetKey = token["TargetValue"].ToString();
                        key.IsMissingInTarget = true;
                        row.MissingRowInTarget = sourceRow;
                    }
                    else
                    {
                        key.TargetKey = "Missing";
                        key.IsMissingInTarget = true;
                        row.MissingRowInTarget = sourceRow;
                    }
                }
                if (key.SourceKey == key.TargetKey)
                {
                    key.SourceTargetKeyMatch = true;
                }
                row.Keys.Add(key);
            }
            row.Fields = new List<RepeaterCompareField>();
            if (fields != null)
            {
                foreach (var field in fields)
                {
                    RepeaterCompareField rcField = new RepeaterCompareField();
                    rcField.FieldName = field;
                    JToken sourceField = null;
                    JToken targetField = null;
                    if (sourceRow != null && sourceRow[field] != null)
                    {
                        sourceField = sourceRow[field];
                    }
                    if (targetRow != null && targetRow[field] != null)
                    {
                        targetField = targetRow[field];
                    }
                    if (sourceField != null)
                    {
                        rcField.SourceValue = sourceField.ToString();
                    }
                    else
                    {
                        rcField.SourceValue = "Missing";
                        rcField.IsMissingInSource = true;
                    }
                    if (targetField != null)
                    {
                        rcField.TargetValue = targetField.ToString();
                    }
                    else
                    {
                        rcField.TargetValue = "Missing";
                        rcField.IsMissingInTarget = true;
                        // row.CanSync = true;
                    }
                    if ((sourceField != null && targetField != null) && (rcField.SourceValue == rcField.TargetValue))
                    {
                        rcField.IsMatch = true;
                    }
                    else
                    {
                        rcField.IsMatch = false;
                    }
                    row.Fields.Add(rcField);
                }
            }
            if (rowCompareType == "Primary") 
            {
                CompareChildRows(criteria, sourceRow, targetRow, ref row);
            }
        }

        private void CompareChildRows(RepeaterCompareCriteria criteria, JToken sourceRow, JToken targetRow,ref RepeaterCompareRow row) 
        {
            if (!String.IsNullOrEmpty(criteria.ChildContainerName) && criteria.ChildFields.Count > 0) 
            {
                //get source matches
                JToken sourceChildRowContainer = null;
                List<JToken> sourceMatches = null;
                if (sourceRow != null)
                {
                    if (sourceRow[criteria.ChildContainerName] != null && sourceRow[criteria.ChildContainerName].HasValues == true)
                    {
                        sourceChildRowContainer = sourceRow[criteria.ChildContainerName];
                    }
                    if (criteria.Childkeys.Count > 0 && criteria.DoChildKeysHaveValues == true)
                    {
                        sourceMatches = RepeaterMatchUtil.GetMatchesInRepeater(sourceChildRowContainer, criteria.ChildKeysMacro, "Source");
                    }
                    else
                    {
                        if (sourceChildRowContainer != null)
                        {
                            sourceMatches = sourceChildRowContainer.Children().ToList();
                        }
                    }
                }

                //get targetmatches
                JToken targetChildRowContainer = null;
                List<JToken> targetMatches = null;
                if (targetRow != null) 
                {
                    if (targetRow[criteria.ChildContainerName] != null && targetRow[criteria.ChildContainerName].HasValues == true)
                    {
                        targetChildRowContainer = targetRow[criteria.ChildContainerName];
                    }
                    if (criteria.Childkeys.Count > 0 && criteria.DoChildKeysHaveValues == true)
                    {
                        targetMatches = RepeaterMatchUtil.GetMatchesInRepeater(targetChildRowContainer, criteria.ChildKeysMacro, "Target");
                    }
                    else
                    {
                        targetMatches = targetChildRowContainer.Children().ToList();
                    }
                }

                //get rows in source only, target only and source/target matches
                List<JToken> sourceOnly = new List<JToken>();
                List<JToken> targetOnly = new List<JToken>();
                Dictionary<JToken, JToken> sourceAndTarget = new Dictionary<JToken, JToken>();


                //compare row by row and generate result
                if (sourceMatches != null && targetMatches != null && (sourceMatches.Count > 0 || targetMatches.Count > 0))
                {
                    //source only and matches
                    foreach (JToken sourceMatch in sourceMatches)
                    {
                        JToken targetMatch = RepeaterMatchUtil.GetMatchingRowFromTargetRepeater(sourceMatch, targetMatches, criteria.ChildKeysMacro,"Source");
                        if (targetMatch == null)
                        {
                            sourceOnly.Add(sourceMatch);
                        }
                        else
                        {
                            sourceAndTarget.Add(sourceMatch, targetMatch);
                        }
                    }
                    //target only
                    foreach (JToken targetMatch in targetMatches)
                    {
                        JToken sourceMatch = RepeaterMatchUtil.GetMatchingRowFromTargetRepeater(targetMatch, sourceMatches, criteria.ChildKeysMacro,"Target");
                        if (sourceMatch == null)
                        {
                            targetOnly.Add(targetMatch);
                        }
                    }
                    row.ChildRows = new List<RepeaterCompareRow>();
                    //get Compare results
                    row.ChildRows.AddRange(CompareRows(sourceAndTarget, criteria,"Child"));
                    if (matchType == "Mismatches only" || matchType == "Both") 
                    {
                        row.ChildRows.AddRange(CompareRows(sourceOnly, criteria, "Source", "Child"));
                        row.ChildRows.AddRange(CompareRows(targetOnly, criteria, "Target", "Child"));
                    }
                }
            }
        }

        private void InitKeysMacro() 
        {
            List<JToken> keys = this.keysMacro.Values().ToList();
            foreach (JToken key in keys)
            {
                string sourceValue = key["SourceValue"].ToString();
                string targetValue = key["TargetValue"].ToString();
                if (targetValue == "") 
                {
                    key["TargetValue"] = sourceValue;
                }
            }
        }
        
        private void SetSyncAndMatchForPrimaryRow(ref RepeaterCompareRow row)
        {
            if (row != null) 
            {
                row.CanSync = true;
                SetSyncForRow(row);
                if (row.ChildRows != null) 
                {
                    if (row.ChildRows != null) 
                    {
                        foreach (var childRow in row.ChildRows)
                        {
                            childRow.CanSync = true;
                            SetSyncForRow(childRow);
                        }
                    }
                }

                if (row.CanSync == true)
                {
                    SetMatchForRow(row);
                    if (row.ChildRows != null) 
                    {
                        foreach (var childRow in row.ChildRows)
                        {
                            SetMatchForRow(childRow);
                            if (childRow.IsMatch == false)
                            {
                                row.IsMatch = false;
                            }
                        }

                    }
                }
                else 
                {
                    row.IsMatch = false;
                    if (row.ChildRows != null)
                    {
                        foreach (var childRow in row.ChildRows)
                        {
                            childRow.IsMatch = false;
                        }

                    }
                }
            }
    }

        private void SetSyncForRow(RepeaterCompareRow row)
        {
            if (row.Fields != null)
            {
                foreach (var field in row.Fields)
                {
                    if (field.IsMissingInSource == true || field.IsMissingInTarget == true)
                    {
                        row.CanSync = false;
                        break;
                    }
                }
            }
            if (row.Keys != null)
            {
                foreach (var key in row.Keys)
                {
                    if (key.IsMissingInSource == true || key.IsMissingInTarget == true)
                    {
                        row.CanSync = false;
                        break;
                    }
                }
            }
        }

        private void SetMatchForRow(RepeaterCompareRow row) 
        {
            row.IsMatch = true;
            if (row.Fields != null)
            {
                foreach (var field in row.Fields)
                {
                    if (field.IsMatch == false)
                    {
                        row.IsMatch= false;
                        break;
                    }
                }
            }
        }

        private void SetSyncAndMatchForRepeater(RepeaterCompareResult result) 
        {
            if (result.Rows != null && result.Rows.Count > 0 && !result.IsRepeaterMissingInSource && !result.IsRepeaterMissingInTarget) 
            {
                result.IsMatch = true;
                foreach (RepeaterCompareRow row in result.Rows) 
                {
                    if (row.CanSync == true) 
                    {
                        result.CanSync = true;
                        if (row.IsMatch == false) 
                        {
                            result.IsMatch = false;
                            break;
                        }
                    }
                }
            }
            else 
            {
                result.CanSync = false;
            }
        }
    }



    public class RepeaterCompareCriteria 
    {
        public List<string> PrimaryFields { get; set; }
        public List<string> ChildFields { get; set; }
        public List<string> PrimaryKeys { get; set; }
        public List<string> Childkeys { get; set; }
        public JToken PrimaryKeysMacro { get; set; }
        public JToken ChildKeysMacro { get; set; }
        public string ChildContainerName { get; set; }
        public int TypeOfMatch { get; set; }
        public bool DoChildKeysHaveValues { get; set; }
    }
}
