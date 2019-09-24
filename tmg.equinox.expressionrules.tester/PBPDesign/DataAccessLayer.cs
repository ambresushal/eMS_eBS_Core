using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignGenerator
{
    public class DataAccessLayer
    {
        private SqlDataAdapter _dataAdapter;
        private SqlConnection _connection;

        public DataAccessLayer()
        {
            _dataAdapter = new SqlDataAdapter();
            _connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PBPContext"].ConnectionString);
            // "Data Source=.;Initial Catalog=PBP;Persist Security Info=True;User ID=sa;Password=sa@123;MultipleActiveResultSets=True;Connection Timeout=180");
        }

        private SqlConnection OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed || _connection.State == ConnectionState.Broken)
            {
                _connection.Open();
            }

            return _connection;
        }

        public DataTable ExecuteSelectQuery(string query, SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            DataTable dt = null;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = OpenConnection();
                cmd.CommandText = query;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                _dataAdapter.SelectCommand = cmd;
                _dataAdapter.Fill(ds);
                dt = ds.Tables[0];
            }

            return dt;
        }

        public string ExecuteReader(string query, SqlParameter[] parameters)
        {
            SqlDataReader sRdr;
            string result = string.Empty;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = OpenConnection();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.uspGetEntityClass";
                cmd.Parameters.AddRange(parameters);
                sRdr = cmd.ExecuteReader();
                while (sRdr.Read())
                {
                    result = Convert.ToString(sRdr.GetString(0).ToString());
                    break;
                }
            }

            return result;
        }
    }
}
