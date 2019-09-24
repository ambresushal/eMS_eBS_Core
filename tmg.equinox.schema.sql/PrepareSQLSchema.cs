using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.models;
using tmg.equinox.schema.Base.Common;
using tmg.equinox.schema.Base.Interface;
using tmg.equinox.schema.Base.Model;

namespace tmg.equinox.schema.sql
{
    public class PrepareSQLSchema : IPrepareSchema
    {
        private static readonly ILog _logger = LogProvider.For<PrepareSQLSchema>();
        tmg.equinox.schema.Base.Interface.IJSchema _schema;
        List<ReportingTableInfo> tables = new List<ReportingTableInfo>();
        JsonDesign _jsonDesign = new JsonDesign();
        public PrepareSQLSchema(IJSchema schema, JsonDesign jsonDesign)
        {
            _jsonDesign = jsonDesign;
            _schema = schema;
        }
        public void PrepareSchema()
        {
            try
            {
                JToken sections = _schema.Get();
                foreach (var section in sections)
                {
                    CreateTable(section, "", SchemaConstant.SECTION);
                }
                _logger.Debug("Prepared table schema from JSON.");
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Exception occured while prepareing table schema from JSON.", ex);
                throw ex;
            }
        }
        public List<ReportingTableInfo> GetSchema()
        {
            return tables;
        }
        private void CreateTable(JToken section, string referenceTableName, string schemaType)
        {
            string designGenratedName = (section[SchemaConstant.GENERATEDNAME] as JValue).ToString();
            var tableName = ((section[SchemaConstant.MDMNAME] as JValue) == null) || ((section[SchemaConstant.MDMNAME] as JValue).ToString() == "") ? (section[SchemaConstant.GENERATEDNAME] as JValue).ToString() : (section[SchemaConstant.MDMNAME] as JValue).ToString().Replace(" ", "_");

            //if (tableName == "LimitInformationDetail")
            //{

            //}

            var elements = section[SchemaConstant.ELEMENTS];
            var table = new ReportingTableInfo();
            table.Name = tableName;
            table.SchemaName = _jsonDesign.TableSchemaName;
            table.ParentName = referenceTableName;
            table.DesignId = _jsonDesign.JsonDesignId;
            table.DesignVersionId = _jsonDesign.JsonDesignVersionId;
            table.CreationDate = DateTime.Now;
            table.Label = _jsonDesign.TableLabel;
            table.Description = _jsonDesign.TableDescription;
            table.DesignType = _jsonDesign.TableDesignType;
            table.DocumentPath = section.SelectToken("FullName").ToString();
            table.DesignVersionNumber = _jsonDesign.VersionNumber;
            table.Columns = new List<ReportingTableColumnInfo>();
            bool isSection = false;
            int ctr = 0;

            foreach (var element in elements)
            {
                if ((isSectionExists(element) == false && isRepeaterExists(element) == false))//section having only columns
                {
                    CreateColumn(table, referenceTableName, element, ctr, schemaType, section["ChildDataSources"], designGenratedName);
                    isSection = false;
                    ctr++;
                }
                else
                {
                    isSection = true; // element having type section or repeater
                    if (isTableAlreadyCreated(table))
                    {
                        tables.Add(table);
                        referenceTableName = tableName;
                    }
                    var criteria = SchemaConstant.REPEATER;
                    if (isRepeaterExists(element) == false)
                    {
                        criteria = SchemaConstant.SECTION;
                    }
                    else
                    if (isSectionExists(element) == false)
                    {
                        criteria = SchemaConstant.REPEATER;
                    }
                    CreateTable(element[criteria], referenceTableName, criteria);
                }
            }

            if (isTableAlreadyCreated(table))
            {
                if (isSection == false)
                {
                    tables.Add(table);
                    _logger.Debug("Prepared [ReportingTableInfo] table object from JSON and Added into list.");
                }
            }
            if (section.SelectToken("ChildDataSources") != null && section.SelectToken("ChildDataSources").Count() > 0)
            {
                CreateChildTable(section.SelectToken("ChildDataSources"), section, tableName, schemaType, table.DocumentPath);
            }            
        }

        private void CreateChildTable(JToken section, JToken parentSection, string referenceTableName, string schemaType,string documentPath)
        {

            string designGenratedName = section[0].SelectToken("DataSourceName").ToString();

            var tableName = referenceTableName + "_" + section[0].SelectToken("DataSourceName").ToString();
            var elements = parentSection[SchemaConstant.ELEMENTS];
            var childElements = section[0].SelectToken("Mappings");

            var table = new ReportingTableInfo();
            table.Name = tableName;
            table.SchemaName = _jsonDesign.TableSchemaName;
            table.ParentName = referenceTableName;
            table.DesignId = _jsonDesign.JsonDesignId;
            table.DesignVersionId = _jsonDesign.JsonDesignVersionId;
            table.CreationDate = DateTime.Now;
            table.Label = _jsonDesign.TableLabel;
            table.Description = _jsonDesign.TableDescription;
            table.DesignType = _jsonDesign.TableDesignType;
            table.DocumentPath = documentPath;
            table.DesignVersionNumber = _jsonDesign.VersionNumber;
            table.Columns = new List<ReportingTableColumnInfo>();
            bool isSection = false;
            int ctr = 0;

            foreach (var ele in elements)
            {
                var element = ele;
                bool isPresent = false;
                foreach (var child in childElements)
                {
                    if ((ele["GeneratedName"] as JValue).ToString() == child.SelectToken("TargetElement").ToString())
                    {
                        isPresent = true;
                    }
                }

                if (!isPresent) { continue; }

                if ((isSectionExists(element) == false && isRepeaterExists(element) == false))//section having only columns
                {
                    CreateColumn(table, referenceTableName, element, ctr, schemaType, parentSection["ChildDataSources"], designGenratedName);
                    isSection = false;
                    ctr++;
                }
                else
                {
                    isSection = true; // element having type section or repeater
                    if (isTableAlreadyCreated(table))
                    {
                        tables.Add(table);
                        referenceTableName = tableName;
                    }

                    var criteria = SchemaConstant.REPEATER;
                    if (isRepeaterExists(element) == false)
                    {
                        criteria = SchemaConstant.SECTION;
                    }
                    else
                    if (isSectionExists(element) == false)
                    {
                        criteria = SchemaConstant.REPEATER;
                    }
                }
            }

            if (isTableAlreadyCreated(table))
            {
                if (isSection == false)
                {
                    tables.Add(table);
                    _logger.Debug("Prepared [ReportingTableInfo] table object from JSON and Added into list.");
                }
            }

        }
        private bool isTableColumnAlreadyCreated(ReportingTableInfo table, string columnName)
        {
            _logger.Debug("Check column object is already exists into table columns.");
            if (table.Columns.ToList().Exists(x => x.Name == columnName))
                return true;
            else
                return false;
        }
        //checkif only section is just the placehoder then do not create table and also add above tables & columns
        private bool isTableAlreadyCreated(ReportingTableInfo table)
        {
            _logger.Debug("Check table object is already exists into table list.");
            return (tables.Exists(m => m.Name == table.Name) == false && table.Columns.Count > 0); //checkif only section is just the placehoder then do not create table and also add above tables & columns
        }
        private void CreateColumn(ReportingTableInfo table, string referenceTableName, JToken element, int ctr, string schemaType, JToken childDataSource, string designGenratedName)
        {
            var type = (element["Type"] as JValue).ToString();
            var dataType = (element["DataType"] as JValue).ToString();
            var length = (element["MaxLength"] as JValue).ToString();
            var defaultValue = (element["DefaultValue"] as JValue).Value;
            var label = ((element["MDMName"] as JValue) == null) || ((element["MDMName"] as JValue).ToString() == "") ? (element["Label"] as JValue).ToString() : (element["MDMName"] as JValue).ToString();
            var description = (element["Label"] as JValue).ToString();
            var visible = (element["Label"] as JValue).ToString();
            var visibleValue = true;
            if (visible != "")
            {
                if (visible.ToLower() == "false")
                {
                    visibleValue = false;
                }

            }
            if (ctr == 0)
            {
                table.Columns.Add(new ReportingTableColumnInfo { Name = "ID", DataType = "long", IsIdentity = true, IsPrimaryKey = true, Visible = true });
                table.Columns.Add(new ReportingTableColumnInfo { Name = "FormInstanceId", DataType = "long", isNullable = true, Visible = true });

                if (schemaType == SchemaConstant.REPEATER)
                {
                    table.Columns.Add(new ReportingTableColumnInfo { Name = "Sequence", DataType = "long", isNullable = true, Visible = true });
                }
                if (referenceTableName != "")
                {
                    table.Columns.Add(new ReportingTableColumnInfo
                    {
                        Name = string.Format("{0}_ID", referenceTableName),
                        isForiegnKey = true,
                        ForiegnKeyColumnName = "ID",
                        ForiegnKeyTableName = referenceTableName,
                        ReferenceTable = referenceTableName,
                        DataType = "long",
                        CustomType = schemaType,
                        Label = label,
                        Description = description,
                        Visible = visibleValue
                    });
                }
            }

            if ((element["GeneratedName"] as JValue).ToString() == "")
            {
                _logger.Debug(string.Format("GeneratedName Name found blank. {0}", label));
                return;
            }
            if (!(dataType == "NA" || type == "blank"))
            {
                string fullName = GetFullPathNameDataSource(childDataSource, element);

                fullName = GetFullPathNameIfRepeater(fullName, element, schemaType);

                //if (fullName != "" && table.DocumentPath == "")
                //{
                //    if (fullName.IndexOf(table.Name) > 0)
                //    {
                //        table.DocumentPath = fullName.Substring(0, fullName.LastIndexOf(table.Name) + table.Name.Length).Replace("0", "").Replace("..", ".");
                //        table.DocumentPath = fullName.Substring(0, fullName.IndexOf(table.Name) + table.Name.Length).Replace("0", "").Replace("..", ".");
                //        if (table.DocumentPath.Substring(0, 1) == ".")
                //            table.DocumentPath = table.DocumentPath.Remove(0, 1);
                //    }
                //    else if (fullName.IndexOf(designGenratedName) > 0) {                        
                //        table.DocumentPath = fullName.Substring(0, fullName.LastIndexOf(designGenratedName) + designGenratedName.Length).Replace("0", "").Replace("..", ".");
                //        if (table.DocumentPath.Substring(0, 1) == ".")
                //            table.DocumentPath = table.DocumentPath.Remove(0, 1);
                //    }
                //}
                string columnName = ((element["MDMName"] as JValue) == null) || ((element["MDMName"] as JValue).ToString() == "") ? (element["GeneratedName"] as JValue).ToString() : (element["MDMName"] as JValue).ToString().Replace(" ", "");
                if (!isTableColumnAlreadyCreated(table, columnName))
                {
                    table.Columns.Add(new ReportingTableColumnInfo
                    {
                        Name = columnName,
                        DataType = dataType,
                        Length = (dataType != "string") ? null : ((Convert.ToInt32(length) == 0) ? GetDefaultLengthByDataType(dataType) : length.ToString()),
                        isNullable = (defaultValue == null) ? true : false,
                        valuePath = fullName,
                        CustomType = schemaType,
                        Label = label,
                        Description = description,
                        Visible = visibleValue
                    });
                }
            }

            _logger.Debug("Prepared [ReportingTableColumnInfo] column object from JSON and Added into table columns.");
        }
        private string GetDefaultLengthByDataType(string dataType)
        {
            var len = "max";
            switch (dataType.ToLower())
            {
                case "string":
                case "varchar":
                case "nvarchar":
                    len = "2000";
                    break;
                case "int":
                    len = "10";
                    break;
                case "long":
                    len = "20";
                    break;
                case "date":
                    len = "20";
                    break;
                case "bool":
                    len = "2";
                    break;
                case "decimal":
                    len = "20";
                    break;
                case "float":
                    len = "20";
                    break;
            }

            return len;
        }

        private string GetFullPathNameIfRepeater(string fullName, JToken element, string schemaType)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                if (schemaType == SchemaConstant.REPEATER)
                {
                    fullName = (element["FullName"] as JValue).ToString();
                    var replace = string.Format("0.{0}", (element["GeneratedName"] as JValue).ToString());
                    fullName = fullName.Replace((element["GeneratedName"] as JValue).ToString(), replace);
                }
                else
                    fullName = (element["FullName"] as JValue).ToString();
            }
            return fullName;
        }
        private string GetFullPathNameDataSource(JToken childDataSource, JToken element)
        {
            if (childDataSource != null)
            {
                foreach (var child in childDataSource)
                {
                    var targetParent = (child["TargetParent"] as JValue).ToString();
                    var dataSourceName = (child["DataSourceName"] as JValue).ToString();
                    foreach (var mapping in child["Mappings"])
                    {
                        var path = string.Format("{0}.{1}", targetParent, (mapping["TargetElement"] as JValue).ToString());
                        if (path == (element["FullName"] as JValue).ToString())
                        {
                            dataSourceName = string.Format("{0}.0.{1}.0.{2}", targetParent, dataSourceName, (element["GeneratedName"] as JValue).ToString());
                            return dataSourceName;
                        }
                    }
                }
            }
            return "";
        }
        private bool isSectionExists(JToken element)
        {
            _logger.Debug("Check whether [Section] object exists or not.");
            return element["Section"].HasValues;
        }
        private bool isRepeaterExists(JToken element)
        {
            _logger.Debug("Check whether [Repeater] object exists or not.");
            return element["Repeater"].HasValues;
        }
    }
}
