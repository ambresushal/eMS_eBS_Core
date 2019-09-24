using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tmg.equinox.applicationservices.viewmodels.UIElement;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignVersion
{
    public class RepeaterDesign
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string GeneratedName { get; set; }
        public string FullName { get; set; }
        public string Label { get; set; }
        public string HelpText { get; set; }
        public string RepeaterType { get; set; }
        public int? LayoutColumn { get; set; }
        public string LayoutClass { get; set; }
        public int RowCount { get; set; }
        public List<ElementDesign> Elements { get; set; }
        public DataSourceDesign PrimaryDataSource { get; set; }
        public List<DataSourceDesign> ChildDataSources { get; set; }
        public int ChildCount { get; set; }
        public bool LoadFromServer { get; set; }
        public bool IsDataRequired { get; set; }
        public bool AllowBulkUpdate { get; set; }

        public bool? IsEnabled { get; set; }

        // Properties for Configuring Param Query Features 
        public bool DisplayTopHeader { get; set; }
        public bool DisplayTitle { get; set; }
        public int FrozenColCount { get; set; }
        public int FrozenRowCount { get; set; }
        public bool AllowPaging { get; set; }
        public int RowsPerPage { get; set; }
        public bool AllowExportToExcel { get; set; }
        public bool AllowExportToCSV { get; set; }
        public string FilterMode { get; set; }
        public string Op { get; set; }
        public string MDMName { get; set; }
        public RepeaterUIElementPropertyModel RepeaterUIElementProperties { get; set; }
        public bool IsSameSectionRuleSource { get; set; }

        internal void BuildDefaultJSONDataObject(ref StringBuilder jsonBuilder, bool isNewFormInstance)
        {
            if (this.Elements.Count > 0)
            {
                var elementList = from elem in this.Elements where elem.IsPrimary == true select elem;
                bool hasPrimary = false;
                if (elementList != null)
                {
                    int index = 0;
                    foreach (var elem in elementList)
                    {
                        if (!String.IsNullOrEmpty(elem.GeneratedName) && IsPrimaryElement(elem))
                        {
                            elem.BuildDefaultJSONDataObject(ref jsonBuilder, isNewFormInstance);
                            if (index < (elementList.Count() - 1))
                            {
                                jsonBuilder.Append(" , ");
                            }
                        }
                        index++;
                    }
                    if (index > 0)
                    {
                        hasPrimary = true;
                    }
                }
                if (this.ChildDataSources != null)
                {
                    int idx = 0;
                    if (hasPrimary == true)
                    {
                        jsonBuilder.Append(" , ");
                    }
                    foreach (var cds in this.ChildDataSources)
                    {
                        jsonBuilder.Append("\"" + cds.DataSourceName + "\": ");
                        jsonBuilder.Append(" [{ ");
                        int mapIdx = 0;
                        foreach (var map in cds.Mappings)
                        {
                            jsonBuilder.Append("\"" + map.TargetElement + "\": \"\"");
                            if (mapIdx < cds.Mappings.Count() - 1)
                            {
                                jsonBuilder.Append(" , ");
                            }
                            mapIdx++;
                        }
                        jsonBuilder.Append(" }] ");
                        if (idx < this.ChildDataSources.Count() - 1)
                        {
                            jsonBuilder.Append(" , ");
                        }
                        idx++;
                    }
                }
            }
        }

        internal void BuildSyncMacro(ref StringBuilder jsonBuilder, bool isNewFormInstance)
        {
            jsonBuilder.Append("\"" + this.GeneratedName + "\": ");
            if (this.Elements.Count > 0)
            {
                jsonBuilder.Append(" { \"Keys\" : [ ");
                int index = 0;
                var keys = from elem in this.Elements select elem;
                if (keys != null)
                {
                    //List<string> repeaterKeys = DocumentMacroSyncConfig.GetConfiguredRepeaterKeys(this.FullName);
                    List<string> repeaterKeys = new List<string>();
                    foreach (var elem in keys)
                    {
                        if (elem.IsKey)
                        {
                            repeaterKeys.Add(elem.GeneratedName);
                        }
                    }
                    if (repeaterKeys != null && repeaterKeys.Count > 0)
                    {
                        keys = (from r in keys
                                join k in repeaterKeys on r.GeneratedName equals k
                                select r).ToList();


                        foreach (var elem in keys)
                        {
                            if (!String.IsNullOrEmpty(elem.GeneratedName))
                            {
                                jsonBuilder.Append(" {\"Key\":\"");
                                jsonBuilder.Append(elem.GeneratedName);
                                jsonBuilder.Append("\" ,\"Label\":\"");
                                jsonBuilder.Append(elem.Label);
                                jsonBuilder.Append("\",\"MatchType\" :\"\", \"SourceValue\":\"\", \"TargetValue\":\"\",\"WildCard\":\"\" }");
                            }
                            if (index < keys.Count() - 1)
                            {
                                jsonBuilder.Append(" , ");
                            }
                            index++;
                        }
                    }
                }
                jsonBuilder.Append(" ] , ");
                jsonBuilder.Append(" \"Label\" : \"" + this.Label + "\" ,");
                jsonBuilder.Append(" \"IsMacroSelected\" : \"false\" ,");
                jsonBuilder.Append("\"Fields\" : [ ");
                //var fields = from elem in this.Elements where elem.IsPrimary == true && elem.IsLabel == false select elem;
                //if (fields != null)
                //{
                //    index = 0;
                //    foreach (var elem in fields)
                //    {
                //        if (!String.IsNullOrEmpty(elem.GeneratedName))
                //        {
                //            jsonBuilder.Append(" { \"Field\" : \"");
                //            jsonBuilder.Append(elem.GeneratedName);
                //            jsonBuilder.Append("\" }");
                //        }
                //        if (index < fields.Count() - 1)
                //        {
                //            jsonBuilder.Append(" , ");
                //        }
                //        index++;
                //    }
                //}
                jsonBuilder.Append(" ]");
                jsonBuilder.Append(" }");
            }
            else
            {
                jsonBuilder.Append(" { \"Keys\" : [] , \"Fields\": [] }");

            }
        }

        private bool IsPrimaryElement(ElementDesign element)
        {
            bool isPrimary = true;
            if (this.ChildDataSources != null)
            {
                foreach (var cds in this.ChildDataSources)
                {
                    var elem = (from el in cds.Mappings where el.TargetElement == element.GeneratedName select el.TargetElement).FirstOrDefault();
                    if (!String.IsNullOrEmpty(elem))
                    {
                        isPrimary = false;
                        break;
                    }
                }
            }
            return isPrimary;
        }
    }
}
