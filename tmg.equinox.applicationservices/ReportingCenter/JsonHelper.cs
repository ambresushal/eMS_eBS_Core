using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.ReportingCenter
{
    public class JsonHelper
    {

        public string JsonColumnNameFormatting(string JColumn)
        {
            JColumn = JColumn.Replace(" ", "");
            JColumn = JColumn.Replace("?", "");
            JColumn = JColumn.Replace("-", "");
            JColumn = JColumn.Replace("_", "");
            JColumn = JColumn.Replace("(", "");
            JColumn = JColumn.Replace(")", "");
            JColumn = JColumn.Replace("%", "");
            return JColumn;
        }

        public string CreateJson()
        {
            string connString = ConfigurationManager.ConnectionStrings["ReportingCenterContext"].ConnectionString;
            SqlDataReader drSchema, drTable, drColumn;
            string SchemaName = "", TableID = "", TableName = "", ColumnName = "";
            var JSONString = new StringBuilder();
            string DataType = "", Length = "", Description = "";
            bool IsPrimaryKey = false, IsNullable = false, IsUnique = false, IsIdentity = false, IsForiegnKey = false;

            using (SqlConnection conn = new SqlConnection(connString))
            {

                //string query = "select top 500 SchemaName,RTI.Name as TableName,RTI.Label,DesignType,RTCI.Name";
                //query +=" from RPT.ReportingTableInfo RTI inner join RPT.ReportingTableColumnInfo RTCI on RTI.ID = RTCI.ReportingTableInfo_ID";
                string querySchema = "select  Distinct top 2 SchemaName from RPT.ReportingTableInfo";
                string queryTable = "select * from RPT.ReportingTableInfo where SchemaName='{0}'";
                string queryColumn = "select * from RPT.ReportingTableColumnInfo Where ReportingTableInfo_ID={0}";

                SqlCommand cmd = new SqlCommand(querySchema, conn);
                cmd.CommandTimeout = 0;
                conn.Open();
                drSchema = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (drSchema.Read())
                {
                    SchemaName = drSchema["SchemaName"].ToString();
                    JSONString.Append("{text :" + "\"" + SchemaName + "\", imageUrl: \"/Content/css/custom-theme/images/dbSchema1.jfif\" , items: [");
                    using (SqlConnection ConnTable = new SqlConnection(connString))
                    {
                        queryTable = String.Format(queryTable, SchemaName);
                        SqlCommand cmdTable = new SqlCommand(queryTable, ConnTable);
                        cmdTable.CommandTimeout = 0;
                        ConnTable.Open();
                        drTable = cmdTable.ExecuteReader(CommandBehavior.CloseConnection);
                        while (drTable.Read())
                        {
                            TableID = drTable["ID"].ToString();
                            TableName = drTable["Name"].ToString();
                            JSONString.Append("{text :" + "\"" + TableName + "\"," + "ColType: \"Table\", imageUrl: \"/Content/css/custom-theme/images/table.jfif\", items: [");
                            using (SqlConnection ConnColumn = new SqlConnection(connString))
                            {
                                queryColumn = String.Format(queryColumn, TableID);
                                SqlCommand cmdColumn = new SqlCommand(queryColumn, ConnColumn);
                                cmdColumn.CommandTimeout = 0;
                                ConnColumn.Open();
                                drColumn = cmdColumn.ExecuteReader(CommandBehavior.CloseConnection);
                                while (drColumn.Read())
                                {
                                    ColumnName = drColumn["Name"].ToString();
                                    IsPrimaryKey = Convert.ToBoolean(drColumn["IsPrimaryKey"]);
                                    IsNullable = Convert.ToBoolean(drColumn["IsNullable"]);
                                    IsUnique = Convert.ToBoolean(drColumn["IsUnique"]);
                                    IsIdentity = Convert.ToBoolean(drColumn["IsIdentity"]);
                                    IsForiegnKey = Convert.ToBoolean(drColumn["IsForiegnKey"]);
                                    DataType = drColumn["DataType"].ToString();
                                    Length = drColumn["Length"].ToString();
                                    Description = drColumn["Description"].ToString();

                                    if (IsPrimaryKey)
                                        JSONString.Append("{text :" + "\"" + ColumnName + "\"," + "ColType: \"" + DataType + "\"," + "Length: \"" + Length + "\"," + "Description: \"" + Description + "\"," + "IsNullable: \"" + IsNullable.ToString() + "\"," + "IsForiegnKey: \"" + IsForiegnKey.ToString() + "\"," + "IsPrimaryKey: \"" + IsPrimaryKey + "\"," + "IsIdentity: \"" + IsIdentity + "\"," + "IsUnique: \"" + IsUnique + "\", imageUrl: \"/Content/css/custom-theme/images/Pkey2.jfif\"},");
                                    else
                                        JSONString.Append("{text :" + "\"" + ColumnName + "\"," + "ColType: \"" + DataType + "\"," + "Length: \"" + Length + "\"," + "Description: \"" + Description + "\"," + "IsNullable: \"" + IsNullable.ToString() + "\"," + "IsForiegnKey: \"" + IsForiegnKey.ToString() + "\"," + "IsPrimaryKey: \"" + IsPrimaryKey + "\"," + "IsIdentity: \"" + IsIdentity + "\"," + "IsUnique: \"" + IsUnique + "\", imageUrl: \"/Content/css/custom-theme/images/Column1.jfif\"},");

                                    JSONString.Replace("False", "No");
                                    JSONString.Replace("True", "Yes");
                                }
                                JSONString.Remove(JSONString.Length - 1, 1);
                            }
                            JSONString.Append("]},");
                            TableID = "";
                            queryColumn = "select * from RPT.ReportingTableColumnInfo Where ReportingTableInfo_ID={0}";
                        }
                        JSONString.Remove(JSONString.Length - 1, 1);

                    }
                    JSONString.Append("]},");
                    SchemaName = "";
                    queryTable = "select * from RPT.ReportingTableInfo where SchemaName='{0}'";
                }
                if (JSONString.Length > 0) JSONString.Remove(JSONString.Length - 1, 1);
                return JSONString.ToString();
            }
        }

    }
}
