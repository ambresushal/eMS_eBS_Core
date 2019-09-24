using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using tmg.equinox.schema.sql;
using tmg.equinox.applicationservices.viewmodels.ReportingCenter;
using tmg.equinox.caching.Interfaces;
using tmg.equinox.repository.interfaces;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.entities.Models;
using System.Runtime.Caching;
using tmg.equinox.caching.client;
using System.ComponentModel;
using System.Web.Script.Serialization;
using tmg.equinox.schema.Base.Model;
using tmg.equinox.repository.models;

namespace tmg.equinox.applicationservices.ReportingCenter
{
    public class ReportingCenterSchemaService : IReportingCenterSchemaService
    {

        #region Private Memebers
        private IRptUnitOfWorkAsync _unitOfWork { get; set; }
        private const string CacheKey = "JSONString";
        private const string CacheAllTableKey = "AllTables";
        IList<ReportingTableInfo> reportingTableColumnInfoData = null;
        public StringBuilder JSONString = new StringBuilder();
        #endregion Private Memebers

        public ReportingCenterSchemaService(IRptUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            CachingManager.InitializeMemoryCache();
        }

        public ReportingCenterSchemaService()
        {
            CachingManager.InitializeMemoryCache();
        }

        public string GetReportingSchemaForDisplay()
        {
            var schemas = new List<SchemaViewModel>();
            if (CachingManager.memoryCachingStore.Exists(CacheKey))
                return CachingManager.memoryCachingStore.TryGetValue<string>(CacheKey);
            else
            {

                //   int TableID;
                IList<RCSchemaViewModel> Schemas = GetSchemaList();
                foreach (RCSchemaViewModel schema in Schemas)
                {
                    var displaySchema = new SchemaViewModel
                    {
                        ColType = "Schema",
                        imageUrl = @"/Content/css/custom-theme/images/dbSchema1.jfif",
                        text = schema.SchemaName
                    };

                    List<SchemaTableViewModel> tables = GetTablesBySchemaList(schema.SchemaName);

                    foreach (SchemaTableViewModel table in tables)
                    {

                        table.items = CreateTableJson(table.text);


                    }
                    displaySchema.items = tables;


                    schemas.Add(displaySchema);
                }


                var serializer = new JavaScriptSerializer() { MaxJsonLength = 2147483647 };


                var schemaData = serializer.Serialize(schemas);
                CachingManager.memoryCachingStore.AddOrUpdate<string>(CacheKey, schemaData);
                return schemaData;
            }
        }
        //public string CreateJsonWithLinq()
        //{
        //    if (CachingManager.memoryCachingStore.Exists(CacheKey))
        //        return CachingManager.memoryCachingStore.TryGetValue<string>(CacheKey);
        //    else
        //    {
        //        int TableID;
        //        string SchemaName = "", TableName = "";
        //        IList<RCSchemaViewModel> Schemas = GetSchemaList();
        //        foreach (RCSchemaViewModel schema in Schemas)
        //        {
        //            SchemaName = schema.SchemaName;
        //            JSONString.Append("{");
        //            JSONString.Append(string.Format("\"text\" :\"{0}\",", SchemaName));
        //            JSONString.Append("\"ColType\": \"Schema\", \"imageUrl\": \"/Content/css/custom-theme/images/dbSchema1.jfif\" , \"items\": [");
        //            IList<RCTablesViewModel> Tables = GetTablesBySchemaList(SchemaName);
        //            foreach (RCTablesViewModel table in Tables)
        //            {
        //                TableID = (int)table.ID;
        //                TableName = table.Name;
        //                CreateTableJson(TableID, TableName);
        //                JSONString.Remove(JSONString.Length - 1, 1);
        //                JSONString.Append("]},");
        //            }
        //            JSONString.Remove(JSONString.Length - 1, 1);
        //            JSONString.Append("]},");
        //        }
        //        if (JSONString.Length > 0) JSONString.Remove(JSONString.Length - 1, 1);
        //        // Store data in the cache    
        //        CachingManager.memoryCachingStore.AddOrUpdate<string>(CacheKey, JSONString.ToString());
        //        return JSONString.ToString();
        //    }
        //}

        public List<SchemaTableColumnViewModel> CreateTableJson(string TableName)
        {
            // JSONString.Append("{");
            //JSONString.Append(string.Format("\"text\" :\"{0}\",", TableName));
            //JSONString.Append("\"ColType\": \"Table\", \"imageUrl\": \"/Content/css/custom-theme/images/table.jfif\", \"items\": [");
            ReportingTableInfo itemTable = GetAllTables().Where(m => m.Name == TableName).FirstOrDefault();
            return WriteColumns(itemTable);
        }

        public void CreateForiegnKeyTableJson(string TableName)
        {
            //print foriegn key table here

            JSONString.Append("{");
            JSONString.Append(string.Format("\"text\" :\"{0}\",", TableName));
            JSONString.Append("\"ColType\": \"Foreign Key Table\", \"imageUrl\": \"/Content/css/custom-theme/images/wb-foreign-key.png\", \"items\": [");

            ReportingTableInfo itemForignTable = GetAllTables().Where(m => m.Name == TableName).FirstOrDefault();

            WriteColumns(itemForignTable);
            JSONString.Remove(JSONString.Length - 1, 1);
            JSONString.Append("]},");
        }
        private string isNull(string fieldName)
        {
            if (fieldName == null)
                return " ";
            else
                return fieldName;
        }

        public List<SchemaTableColumnViewModel> WriteColumns(ReportingTableInfo itemTable)
        {
            /*string ColumnName = "", DataType = "", Length = "", Description = "";
            string ForiegnKeyTableName = "", ForiegnKeyColumnName = "", valuePath = "";
            string ReferenceTable = "", CustomType = "", Label = "";
            bool IsPrimaryKey = false, IsNullable = false, IsUnique = false, IsIdentity = false, IsForiegnKey = false;*/


            return itemTable.Columns.Select(item => new SchemaTableColumnViewModel()
            {
                text = item.Name,
                IsPrimaryKey = (item.IsPrimaryKey == true ? "Yes" : "No"),
                IsNullable = (item.isNullable == true ? "Yes" : "No"),
                IsUnique = (item.isUnique == true ? "Yes" : "No"),
                IsIdentity = (item.IsIdentity == true ? "Yes" : "No"),
                IsForiegnKey = (item.isForiegnKey == true ? "Yes" : "No"),
                ColType = isNull(item.DataType),
                Length = isNull(item.Length),
                ForiegnKeyTableName = isNull(item.ForiegnKeyTableName),
                ForiegnKeyColumnName = isNull(item.ForiegnKeyColumnName),
                valuePath = isNull(item.Label),
                ReferenceTable = isNull(item.valuePath),
                CustomType = isNull(item.CustomType),
                Description = isNull(item.Description),
                imageUrl = (item.IsPrimaryKey == true ?
                                    @"/Content/css/custom-theme/images/primarykey.gif" :
                                                (item.ForiegnKeyTableName != null ? @"/Content/css/custom-theme/images/Pkey2.jfif" : @"/Content/css/custom-theme/images/Colum1.jfif"))

            }).ToList();



            /*

            if (IsPrimaryKey)   //PRIMARY KEY
            {
                JSONString.Append("\"imageUrl\": \"/Content/css/custom-theme/images/primarykey.gif\"},");
            }
            else if (ForiegnKeyTableName != null) //FORIEGN KEY
            {
                JSONString.Append("\"imageUrl\": \"/Content/css/custom-theme/images/Pkey2.jfif\"},");
                CreateForiegnKeyTableJson(ForiegnKeyTableName);
            }
            else    //NORMAL COLUMN
            {
                JSONString.Append("\"imageUrl\": \"/Content/css/custom-theme/images/Colum1.jfif\"},");
            }
            */
            //  JSONString.Replace("False", "No");
            //   JSONString.Replace("True", "Yes");

        }
        //public void WriteColumns(ReportingTableInfo itemTable)
        //{
        //    string ColumnName = "", DataType = "", Length = "", Description = "";
        //    string ForiegnKeyTableName = "", ForiegnKeyColumnName = "", valuePath = "";
        //    string ReferenceTable = "", CustomType = "", Label = "";
        //    bool IsPrimaryKey = false, IsNullable = false, IsUnique = false, IsIdentity = false, IsForiegnKey = false;

        //    foreach (var item in itemTable.Columns)
        //    {
        //        ColumnName = item.Name;
        //        IsPrimaryKey = item.IsPrimaryKey;
        //        IsNullable = item.isNullable;
        //        IsUnique = item.isUnique;
        //        IsIdentity = item.IsIdentity;
        //        IsForiegnKey = item.isForiegnKey;
        //        DataType = item.DataType;
        //        Length = item.Length;
        //        ForiegnKeyTableName = item.ForiegnKeyTableName;
        //        ForiegnKeyColumnName = item.ForiegnKeyColumnName;
        //        valuePath = item.valuePath;
        //        ReferenceTable = item.valuePath;
        //        CustomType = item.CustomType;
        //        Label = item.Label;
        //        Description = item.Description;

        //        if (Description != null)
        //            Description = Description.Replace("\"", "");
        //        if (Label != null)
        //            Label = Description.Replace("\"", "");


        //        JSONString.Append("{");
        //        JSONString.Append(string.Format("\"text\" :\"{0}\"", ColumnName));
        //        JSONString.Append(string.Format(",\"ColType\":\"{0}\"", DataType));
        //        JSONString.Append(string.Format(",\"Length\": \" {0}\"", Length));

        //        JSONString.Append(string.Format(",\"Description\" :\"{0}\"", Description));
        //        JSONString.Append(string.Format(",\"IsNullable\":\"{0}\"", IsNullable.ToString()));
        //        JSONString.Append(string.Format(",\"IsForiegnKey\": \" {0}\"", IsForiegnKey.ToString()));

        //        JSONString.Append(string.Format(",\"IsPrimaryKey\" :\"{0}\"", IsPrimaryKey));
        //        JSONString.Append(string.Format(",\"IsIdentity\":\"{0}\"", IsIdentity));
        //        JSONString.Append(string.Format(",\"IsUnique\": \" {0}\"", IsUnique));

        //        JSONString.Append(string.Format(",\"ForiegnKeyTableName\" :\"{0}\"", ForiegnKeyTableName));
        //        JSONString.Append(string.Format(",\"ForiegnKeyColumnName\":\"{0}\"", ForiegnKeyColumnName));
        //        JSONString.Append(string.Format(",\"valuePath\": \" {0}\"", valuePath));

        //        JSONString.Append(string.Format(",\"ReferenceTable\" :\"{0}\"", ReferenceTable));
        //        JSONString.Append(string.Format(",\"CustomType\":\"{0}\"", CustomType));
        //        JSONString.Append(string.Format(",\"valuePath\": \" {0}\",", Label));


        //        if (IsPrimaryKey)   //PRIMARY KEY
        //        {
        //            JSONString.Append("\"imageUrl\": \"/Content/css/custom-theme/images/primarykey.gif\"},");
        //        }
        //        else if (ForiegnKeyTableName != null) //FORIEGN KEY
        //        {
        //            JSONString.Append("\"imageUrl\": \"/Content/css/custom-theme/images/Pkey2.jfif\"},");
        //            CreateForiegnKeyTableJson(ForiegnKeyTableName);
        //        }
        //        else    //NORMAL COLUMN
        //        {
        //            JSONString.Append("\"imageUrl\": \"/Content/css/custom-theme/images/Colum1.jfif\"},");
        //        }

        //        JSONString.Replace("False", "No");
        //        JSONString.Replace("True", "Yes");
        //    }
        //}

        public IList<RCSchemaViewModel> GetSchemaList()
        {
            dynamic Schemalist = null;
            Schemalist = (from rt in GetAllTables().Select(p => p.SchemaName).Distinct()
                          select new RCSchemaViewModel { SchemaName = rt }).OrderBy(m => m.SchemaName).ToList();
            return Schemalist;
        }



        public DataTable ToDataTables<T>(IList<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prp = props[i];
                table.Columns.Add(prp.Name, prp.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
        public List<ReportingTableDetails> GetRawTableData()
        {
            SQLSchemaRepository sqlSchemaRepository = new SQLSchemaRepository(null, this._unitOfWork);
            return sqlSchemaRepository.GetRawTableData();

        }
        public IList<ReportingTableInfo> GetAllTables()
        {

            if (reportingTableColumnInfoData == null)
                reportingTableColumnInfoData = CachingManager.memoryCachingStore.TryGetValue<IList<ReportingTableInfo>>(CacheAllTableKey);

            if (reportingTableColumnInfoData == null)
            {

                SQLSchemaRepository sqlSchemaRepository = new SQLSchemaRepository(null, this._unitOfWork);
                reportingTableColumnInfoData = sqlSchemaRepository.GetTables();

                CachingManager.memoryCachingStore.AddOrUpdate<IList<ReportingTableInfo>>(CacheAllTableKey, reportingTableColumnInfoData);
            }

            return reportingTableColumnInfoData;
        }

        //public IList<RCTablesViewModel> GetTablesBySchemaList(string SchemaName)
        //{
        //    dynamic Tablelist = null;

        //    Tablelist = (from rt in GetAllTables().Where(rt => rt.SchemaName.Equals(SchemaName))
        //                 select new RCTablesViewModel
        //                 {
        //                     ID = rt.ID,
        //                     Name = rt.Name,
        //                     SchemaName = rt.SchemaName
        //                 }).ToList();
        //    return Tablelist;
        //}
        public List<SchemaTableViewModel> GetTablesBySchemaList(string SchemaName)
        {
            dynamic Tablelist = null;

            Tablelist = (from rt in GetAllTables().Where(rt => rt.SchemaName.Equals(SchemaName))
                         select new SchemaTableViewModel
                         {
                             ColType = "Table",
                             text = rt.Name,
                             imageUrl = @"/Content/css/custom-theme/images/table.jfif"
                         }).OrderBy(r => r.text).ToList();
            return Tablelist;
        }


    }
}
