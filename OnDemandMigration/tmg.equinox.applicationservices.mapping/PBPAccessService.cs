using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.pbp.dataaccess;

namespace tmg.equinox.applicationservices.pbp
{
    public class PBPAccessService : IPBPAccessService
    {
        public JObject GetQIDBenefitData(string QID, string mdbFilePath)
        {
            AccessDbContext context = new AccessDbContext(mdbFilePath);
            throw new NotImplementedException();

        }

        public List<string> GetQIDList(string mdbFilePath)
        {
            AccessDbContext context = new AccessDbContext(mdbFilePath);
            string selectQuery = String.Format(AccessFileConstants.SELECTQUERY, AccessFileConstants.QIDCOLUMN, AccessFileConstants.QIDTABLE, "1 = 1");
            return context.GetQIDList(selectQuery);
        }

        public JObject GetQIDData(string mdbFilePath, List<PBPTable> tables, string QID)
        {
            JObject qidInstance = new JObject();
            foreach (var table in tables)
            {

                JObject tableObject = PBPAccessServiceHelper.GetTableObject(table);
                string query = PBPAccessServiceHelper.GetQuery(table, "QID = '" + QID + "'", true);
                AccessDbContext context = new AccessDbContext(mdbFilePath);

                if (table.TableName == "PBPC_POS" || table.TableName == "PBPMRX_T")
                    context.GetQIDData(query, ref tableObject, true);
                else
                    context.GetQIDData(query, ref tableObject, false);

                if (tableObject != null)
                {
                    if (tableObject[""] != null)
                        qidInstance.Add(table.TableName, tableObject[""]);
                    else
                        qidInstance.Add(table.TableName, tableObject);
                }
                else
                    qidInstance.Add(table.TableName, new JArray());
            }
            return qidInstance;
        }

        public List<AccessDatabaseTableInfo> GetAccessDatabaseTables(string mdbFilePath)
        {
            AccessDbContext context = new AccessDbContext(mdbFilePath);
            //System.Data.DataTable tables;
            System.Data.DataTable columns;
            using (System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(context._connectingString))
            {
                connection.Open();
                //tables = connection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                columns = connection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Columns, new object[] { null, null, null, null });
            }
            List<AccessDatabaseTableInfo> Tables = new List<AccessDatabaseTableInfo>();
            string tableName = "";
            AccessDatabaseTableInfo table = new AccessDatabaseTableInfo();
            table.accessDatabaseTableColumnInfo = new List<AccessDatabaseTableColumnInfo>();
            for (int i = 0; i < columns.Rows.Count; i++)
            {
                if (columns.Rows[i][11].ToString() == "8" || columns.Rows[i][11].ToString() == "129" || columns.Rows[i][11].ToString() == "130"
                    || columns.Rows[i][11].ToString() == "200" || columns.Rows[i][11].ToString() == "201" || columns.Rows[i][11].ToString() == "202"
                    || columns.Rows[i][11].ToString() == "203")
                {
                    WriteLog(columns.Rows[i][2].ToString() + "," + columns.Rows[i][3].ToString() + "," + columns.Rows[i][13].ToString());
                    if (tableName != columns.Rows[i][2].ToString())
                    {
                        if (table.accessDatabaseTableColumnInfo.Count > 0)
                        {
                            Tables.Add(table);
                            table = new AccessDatabaseTableInfo();
                            table.accessDatabaseTableColumnInfo = new List<AccessDatabaseTableColumnInfo>();
                        }
                        tableName = columns.Rows[i][2].ToString();
                        table.TableName = tableName;
                        table.accessDatabaseTableColumnInfo.Add(new AccessDatabaseTableColumnInfo { ColumnName = columns.Rows[i][3].ToString(), ColumnLength = Convert.ToInt32(columns.Rows[i][13]) });
                    }
                    else
                    {
                        table.accessDatabaseTableColumnInfo.Add(new AccessDatabaseTableColumnInfo { ColumnName = columns.Rows[i][3].ToString(), ColumnLength = Convert.ToInt32(columns.Rows[i][13]) });

                    }

                }
            }
            // AccessDatabaseTableColumnInfo avbc = Tables.Where(x => x.TableName == "PBP").FirstOrDefault().accessDatabaseTableColumnInfo.Where(z => z.ColumnName == "BPT_MA_DT").FirstOrDefault();
            return Tables;
        }

        private void WriteLog(string message)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"D:\PBPFieldLength.txt", true))
            {
                file.WriteLine(message);
            }
        }
    }
}
