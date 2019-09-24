using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.pbpimportservices
{
    public class SqlHelperClass
    {

        public static string CreateTABLEPablo(DataTable table)
        {
            string sqlsc;
            string tableName = table.TableName;

            sqlsc = "CREATE TABLE " + tableName + "(";
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sqlsc += "\n" + table.Columns[i].ColumnName;
                if (table.Columns[i].DataType.ToString().Contains("System.Int32"))
                    sqlsc += " int ";
                else if (table.Columns[i].DataType.ToString().Contains("System.DateTime"))
                    sqlsc += " datetime ";
                else if (table.Columns[i].DataType.ToString().Contains("System.String"))
                    sqlsc += " nvarchar(" + table.Columns[i].MaxLength.ToString() + ") ";
                else if (table.Columns[i].DataType.ToString().Contains("System.Single"))
                    sqlsc += " single ";
                else if (table.Columns[i].DataType.ToString().Contains("System.Double"))
                    sqlsc += " double ";
                else
                    sqlsc += " nvarchar(" + table.Columns[i].MaxLength.ToString() + ") ";

                if (table.Columns[i].AutoIncrement)
                    sqlsc += " IDENTITY(" + table.Columns[i].AutoIncrementSeed.ToString() + "," + table.Columns[i].AutoIncrementStep.ToString() + ") ";
                if (!table.Columns[i].AllowDBNull)
                    sqlsc += " NOT NULL ";
                sqlsc += ",";
            }

            string pks = "\nCONSTRAINT PK_" + tableName + " PRIMARY KEY (";
            for (int i = 0; i < table.PrimaryKey.Length; i++)
            {
                pks += table.PrimaryKey[i].ColumnName + ",";
            }
            pks = pks.Substring(0, pks.Length - 1) + ")";

            sqlsc += pks;

            return sqlsc + ")";
        }

        /// <summary>
        /// Inspects a DataTable and return a SQL string that can be used to CREATE a TABLE in SQL Server.
        /// </summary>
        /// <param name="table">System.Data.DataTable object to be inspected for building the SQL CREATE TABLE statement.</param>
        /// <returns>String of SQL</returns>
        public static string GetCreateTableSql(DataTable table, string schemaName)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder alterSql = new StringBuilder();
            if (string.IsNullOrEmpty(schemaName))
            {
                sql.AppendFormat("IF EXISTS ( SELECT * FROM sys.tables WHERE name LIKE '{0}%') DROP TABLE {0} ", table.TableName);
                sql.AppendFormat("CREATE TABLE {0} (", table.TableName);
            }
            else
            {
                sql.AppendFormat("IF EXISTS ( SELECT * FROM sys.tables WHERE name LIKE '{0}%' AND SCHEMA_NAME(schema_id) = '{1}') DROP TABLE [{1}].[{0}] ", table.TableName, schemaName);
                sql.AppendFormat("CREATE TABLE [{0}].[{1}] (", schemaName, table.TableName);
            }

            //Create Batch ID single column
            //sql.AppendFormat("\n\t[PBPImportBatchID] int,");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                bool isNumeric = false;
                bool usesColumnDefault = true;

                sql.AppendFormat("\n\t[{0}]", table.Columns[i].ColumnName);

                switch (table.Columns[i].DataType.ToString().ToUpper())
                {
                    case "SYSTEM.INT16":
                        sql.Append(" smallint");
                        isNumeric = true;
                        break;
                    case "SYSTEM.INT32":
                        sql.Append(" int");
                        isNumeric = true;
                        break;
                    case "SYSTEM.INT64":
                        sql.Append(" bigint");
                        isNumeric = true;
                        break;
                    case "SYSTEM.DATETIME":
                        sql.Append(" datetime");
                        usesColumnDefault = false;
                        break;
                    case "SYSTEM.STRING":
                        if (table.Columns[i].MaxLength > 0)
                        {
                            sql.AppendFormat(" varchar({0})", table.Columns[i].MaxLength);
                        }
                        else
                        {
                            sql.AppendFormat(" varchar(MAX)");
                            //sql.AppendFormat(" varchar({0})", 400);
                        }
                        break;
                    case "SYSTEM.SINGLE":
                        sql.Append(" single");
                        isNumeric = true;
                        break;
                    case "SYSTEM.DOUBLE":
                        sql.Append(" double");
                        isNumeric = true;
                        break;
                    case "SYSTEM.DECIMAL":
                        sql.AppendFormat(" decimal(18, 6)");
                        isNumeric = true;
                        break;
                    default:
                        if (table.Columns[i].MaxLength > 0)
                        {
                            sql.AppendFormat(" varchar({0})", table.Columns[i].MaxLength);
                        }
                        else
                        {
                            sql.AppendFormat(" varchar(MAX)");
                            //sql.AppendFormat(" varchar({0})", 400);
                        }
                        break;
                }

                if (table.Columns[i].AutoIncrement)
                {
                    sql.AppendFormat(" IDENTITY({0},{1})",
                        table.Columns[i].AutoIncrementSeed,
                        table.Columns[i].AutoIncrementStep);
                }
                //else
                //{
                //    // DataColumns will add a blank DefaultValue for any AutoIncrement column. 
                //    // We only want to create an ALTER statement for those columns that are not set to AutoIncrement. 
                //    if (table.Columns[i].DefaultValue != null)
                //    {
                //        if (usesColumnDefault)
                //        {
                //            if (isNumeric)
                //            {
                //                alterSql.AppendFormat("\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ({2}) FOR [{1}];",
                //                    table.TableName,
                //                    table.Columns[i].ColumnName,
                //                    table.Columns[i].DefaultValue);
                //            }
                //            else
                //            {
                //                alterSql.AppendFormat("\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ('{2}') FOR [{1}];",
                //                    table.TableName,
                //                    table.Columns[i].ColumnName,
                //                    table.Columns[i].DefaultValue);
                //            }
                //        }
                //        else
                //        {
                //            // Default values on Date columns, e.g., "DateTime.Now" will not translate to SQL.
                //            // This inspects the caption for a simple XML string to see if there is a SQL compliant default value, e.g., "GETDATE()".
                //            try
                //            {
                //                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();

                //                xml.LoadXml(table.Columns[i].Caption);

                //                alterSql.AppendFormat("\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ({2}) FOR [{1}];",
                //                    table.TableName,
                //                    table.Columns[i].ColumnName,
                //                    xml.GetElementsByTagName("defaultValue")[0].InnerText);
                //            }
                //            catch
                //            {
                //                // Handle
                //            }
                //        }
                //    }
                //}

                if (!table.Columns[i].AllowDBNull)
                {
                    sql.Append(" NOT NULL");
                }

                sql.Append(",");
            }

            if (table.PrimaryKey.Length > 0)
            {
                StringBuilder primaryKeySql = new StringBuilder();

                primaryKeySql.AppendFormat("\n\tCONSTRAINT PK_{0} PRIMARY KEY (", table.TableName);

                for (int i = 0; i < table.PrimaryKey.Length; i++)
                {
                    primaryKeySql.AppendFormat("{0},", table.PrimaryKey[i].ColumnName);
                }

                primaryKeySql.Remove(primaryKeySql.Length - 1, 1);
                primaryKeySql.Append(")");

                sql.Append(primaryKeySql);
            }
            else
            {
                sql.Remove(sql.Length - 1, 1);
            }

            sql.AppendFormat("\n);\n{0}", alterSql.ToString());

            return sql.ToString();
        }

    }

    public static class Library
    {
        public static void WriteErrorLog(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " + ex.Message.ToString().Trim());
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }

        public static void WriteErrorLog(string Message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine( Message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }
    }
}
