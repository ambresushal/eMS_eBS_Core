using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace tmg.equinox.pbpimportservices
{
    public class AccessDbContext
    {
        private OleDbDataAdapter _dataAdapter;
        private OleDbConnection _connection;
        private string _connectingString;
        private OleDbHelperClass _oleDBHelperClass;
        public AccessDbContext(string mdfDbPath)
        {
            _dataAdapter = new OleDbDataAdapter();
            _oleDBHelperClass = new OleDbHelperClass();
            _connectingString = _oleDBHelperClass.GetOleDbConnectingString(mdfDbPath);
            _connection = new OleDbConnection(_connectingString);
        }

        private OleDbConnection OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed || _connection.State == ConnectionState.Broken)
            {
                _connection.Open();
            }

            return _connection;
        }

        public DataTable ExecuteSelectQuery(string query, OleDbParameter[] parameters)
        {
            DataSet ds = new DataSet();
            DataTable dt = null;

            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = OpenConnection();
                cmd.CommandText = query;
                if(parameters != null)
                  cmd.Parameters.AddRange(parameters);
                _dataAdapter.SelectCommand = cmd;
                _dataAdapter.Fill(ds);
                dt = ds.Tables[0];
            }

            return dt;
        }

        public string ExecuteReader(string query, OleDbParameter[] parameters)
        {
            OleDbDataReader sRdr;
            string result = string.Empty;

            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = OpenConnection();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.uspGetEntityClass";
                cmd.Parameters.AddRange(parameters);
                sRdr = cmd.ExecuteReader();
                result = Convert.ToString(sRdr.GetString(0).ToString());
            }

            return result;
        }

        public  DataTable GetUsedTables()
        {
            DataTable dt = null;
            string[] restrictions = new string[4];
            restrictions[3] = "Table";

            OleDbConnection connection = OpenConnection();
            dt = connection.GetSchema("Tables", restrictions);

            return dt;
        }
    }
}
