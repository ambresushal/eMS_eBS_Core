using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using tmg.equinox.applicationservices.viewmodels.QueryManager;

namespace tmg.equinox.web.QueryManager
{
    public class QueryExecutor
    {
        private SqlCommand cmd = null;
        private SqlDataReader rdr = null;
        private bool _CancelExecution;
        public List<string> _Messages;
        public List<string> Messages
        {
            get { return _Messages; }
            set { _Messages = value; }
        }
        private string _LastMessage;
        public string LastMessage
        {
            get { return _LastMessage; }
            set { _LastMessage = value; }
        }
        private SqlConnection _Connection;
        public SqlConnection Connection
        {
            get { return _Connection; }
            set { _Connection = value; }
        }
        private Exception _NrEx;
        public Exception NrEx
        {
            get { return _NrEx; }
            set { _NrEx = value; }
        }
        public SqlException _SqlEx;
        public SqlException SqlEx
        {
            get { return _SqlEx; }
            set { _SqlEx = value; }
        }
        private DataSet _Results;
        public DataSet Results
        {
            get { return _Results; }
            set { _Results = value; }
        }
        private int _TimeOut;
        public int TimeOut
        {
            get { return _TimeOut; }
            set { _TimeOut = value; }
        }
        
        public string ConnectionString
        {
            get
            {
                if (Connection != null)
                    return Connection.ConnectionString;
                return "";
            }
            set
            {
                Connection.ConnectionString = value;
            }
        }        
        public bool Error
        {
            get { return !LastMessage.Equals("OK", StringComparison.CurrentCultureIgnoreCase); }
        }
        public Exception LastException
        {
            get { return NrEx; }
        }
        public SqlException LastSqlException
        {
            get { return SqlEx; }
        }
        public string Server
        {
            get
            {
                if (Connection != null)
                    return Connection.DataSource;
                return "";
            }
        }
        public string DataBase
        {
            get
            {
                if (Connection != null)
                    return Connection.Database;
                return "";
            }
        }

        public void Initialize(string ConnectionString)
        {
            Connection = new SqlConnection(ConnectionString);
            Connection.InfoMessage += Connection_InfoMessage;
            Messages = new List<string>();
            NrEx = null;
            SqlEx = null;
            LastMessage = "OK";
        }
        void Connection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            Messages.Add(String.Format("{0} - {1}", DateTime.Now.ToString("hh:mm:ss.fff"), e.Message));
        }
        
        public void CancelExecute()
        {
            _CancelExecution = true;
        }       

        public void ExecQuery(QueryManagerViewModel model)
        {
            DateTime LastCheck;
            Results = new DataSet();
            cmd = null;
            rdr = null;
            int indexxx;
            SqlTransaction transaction = null; 
            try
            {
                cmd = new SqlCommand(model.UserQuery, Connection);
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                transaction = Connection.BeginTransaction();
                cmd.Transaction = transaction;
                rdr = cmd.ExecuteReader();//(CommandBehavior.CloseConnection);                
                indexxx = 0;
                LastCheck = DateTime.Now;
                do
                {
                    indexxx++;
                    // Create new data table
                    DataTable schemaTable = rdr.GetSchemaTable();
                    DataTable dataTable = new DataTable();
                    if (schemaTable != null)
                    {// A query returning records was executed
                        for (int i = 0; i < schemaTable.Rows.Count; i++)
                        {
                            DataRow dataRow = schemaTable.Rows[i];
                            // Create a column name that is unique in the data table
                            string columnName = (string)dataRow["ColumnName"]; //+ "<C" + i + "/>";
                            if (dataTable.Columns.Contains(columnName))
                            {
                                int index = 1;
                                foreach (DataColumn Col in dataTable.Columns)
                                    if (Col.ColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                                        index++;
                                columnName += index.ToString();
                            }

                            // Add the column definition to the data table
                            System.Type datatype = (Type)dataRow["DataType"];
                            DataColumn column = null;
                            if (datatype.Name == "DateTime")                            
                                column = new DataColumn(columnName, typeof(String));  //converted datetime into string to export formatted data into excel sheet                       
                            else                            
                                column = new DataColumn(columnName, (Type)dataRow["DataType"]);
                            
                            dataTable.Columns.Add(column);
                        }
                        Results.Tables.Add(dataTable);
                        // Fill the data table we just created
                        while (rdr.Read())
                        {
                            DataRow dataRow = dataTable.NewRow();
                            for (int i = 0; i < rdr.FieldCount; i++)
                                dataRow[i] = rdr.GetValue(i);
                            dataTable.Rows.Add(dataRow);
                            if (DateTime.Now.Subtract(LastCheck) > new TimeSpan(0, 0, 1))
                            {
                                if (_CancelExecution)
                                {
                                    rdr.Close();
                                    LastMessage = "OK";
                                    break;
                                }
                                else
                                {
                                    LastCheck = DateTime.Now;
                                }
                            }
                        }

                        DataTable NonQ1 = new DataTable("NonQuery" + indexxx.ToString());
                        NonQ1.Columns.Add(new DataColumn("RowsAffected"));
                        DataRow DRx1 = NonQ1.NewRow();
                        DRx1[0] = Math.Max(rdr.RecordsAffected, 0);
                        NonQ1.Rows.Add(DRx1);
                        Results.Tables.Add(NonQ1);
                    }
                    else
                    {
                        // No records were returned
                        DataTable NonQ2 = new DataTable("NonQuery" + indexxx.ToString());
                        NonQ2.Columns.Add(new DataColumn("RowsAffected"));
                        DataRow DRx2 = NonQ2.NewRow();
                        DRx2[0] = Math.Max(rdr.RecordsAffected, 0);
                        NonQ2.Rows.Add(DRx2);
                        Results.Tables.Add(NonQ2);
                    }
                } while (rdr.NextResult());
                rdr.Close();
                if (model.IsCommit)
                    transaction.Commit();
                else
                    transaction.Rollback();
                cmd.Connection.Close();                    
                LastMessage = "OK";
            }
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                cmd.Connection.Close();
                LastMessage = sqlex.Message;
                SqlEx = sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                cmd.Connection.Close();
                LastMessage = ex.Message;
                NrEx = ex;
            }
            finally
            {
                if (rdr != null)
                {
                    if (!rdr.IsClosed)
                        rdr.Close();
                    rdr.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();                    
                }                    
            }
        }

        public bool TestConnection()
        {
            if (Connection != null && Connection.ConnectionString != null && Connection.ConnectionString.Length > 0)
            {
                try
                {
                    Connection.Open();
                }
                catch (SqlException sqlex)
                {
                    SqlEx = sqlex;
                    LastMessage = sqlex.Message;
                }
                catch (Exception ex)
                {
                    NrEx = ex;
                    LastMessage = ex.Message;
                }
                finally
                {
                    if (Connection.State == System.Data.ConnectionState.Open)
                        Connection.Close();
                }
                return !Error;
            }
            return true;
        }
    }
}
