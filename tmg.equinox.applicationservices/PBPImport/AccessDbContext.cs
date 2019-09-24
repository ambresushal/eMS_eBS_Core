using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.pbpimport.Interfaces;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.PBPImport
{
    public class AccessDbContext : IAccessDbContext
    {
        #region private Members
        //private OleDbDataAdapter _dataAdapter;
        //private OleDbConnection _connection;

        private OleDbHelper _oleDBHelperClass;
        IUnitOfWorkAsync _unitOfWorkAsync = null;

        #endregion

        #region Public 
        public string _connectingString;
        #endregion
        public AccessDbContext(IUnitOfWorkAsync unitOfWorkAsync)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
        }
        #region Constructor
        //public AccessDbContext(string )
        //{
        //    _dataAdapter = new OleDbDataAdapter();
        //    _oleDBHelperClass = new OleDbHelper();
        //    _connectingString = _oleDBHelperClass.GetOleDbConnectingString(mdfDbPath);
        //    _connection = new OleDbConnection(_connectingString);
        //}
        #endregion

        #region Private Methods
     /*   private OleDbConnection OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed || _connection.State == ConnectionState.Broken)
            {
                _connection.Open();
            }

            return _connection;
        }*/
        #endregion

        #region Public Methods
        public DataTable ExecuteSelectQuery(string query, OleDbParameter[] parameters)
        {
            DataSet ds = new DataSet();
            DataTable dt = null;
            using (OleDbConnection connection = new OleDbConnection(_connectingString))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    //cmd.Connection = OpenConnection();
                    connection.Open();
                    cmd.Connection = connection;
                    cmd.CommandText = query;
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    var dataAdapter = new OleDbDataAdapter();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(ds);
                    dt = ds.Tables[0];
                }
            }

            return dt;
        }

        public string ExecuteReader(string query, OleDbParameter[] parameters)
        {
            OleDbDataReader sRdr;
            string result = string.Empty;

            using (OleDbConnection connection = new OleDbConnection(_connectingString))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    connection.Open();
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.uspGetEntityClass";
                    cmd.Parameters.AddRange(parameters);
                    sRdr = cmd.ExecuteReader();
                    result = Convert.ToString(sRdr.GetString(0).ToString());
                }
            }

            return result;
        }

        public DataTable GetUsedTables()
        {
            DataTable dt = null;
            string[] restrictions = new string[4];
            restrictions[3] = "Table";

            // OleDbConnection connection = OpenConnection();
            using (OleDbConnection connection = new OleDbConnection(_connectingString))
            {
                connection.Open();
                dt = connection.GetSchema("Tables", restrictions);
            }
            return dt;
        }

        public void InitializeVariables(string mdfDbPath)
        {

            _oleDBHelperClass = new OleDbHelper();
            _connectingString = _oleDBHelperClass.GetOleDbConnectingString(mdfDbPath);
           // _connection = new OleDbConnection(_connectingString);
        }

        public string GetConnectingString()
        {
            return _connectingString;
        }
        #endregion
    }
}
