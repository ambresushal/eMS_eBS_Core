using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.pbp.dataaccess
{
    public class AccessDbContext
    {
        #region private Members
        private OleDbDataAdapter _dataAdapter;
        private OleDbConnection _connection;
        private OleDbHelper _oleDBHelperClass;
        #endregion

        #region Public 
        public string _connectingString;
        #endregion
        #region Constructor
        public AccessDbContext(string mdfDbPath)
        {
            _dataAdapter = new OleDbDataAdapter();
            _oleDBHelperClass = new OleDbHelper();
            _connectingString = _oleDBHelperClass.GetOleDbConnectingString(mdfDbPath);
            _connection = new OleDbConnection(_connectingString);
        }
        #endregion

        #region Private Methods
        private OleDbConnection OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed || _connection.State == ConnectionState.Broken)
            {
                _connection.Open();
            }

            return _connection;
        }
        #endregion

        #region Public Methods
        public DataTable ExecuteSelectQuery(string query, OleDbParameter[] parameters)
        {
            DataSet ds = new DataSet();
            DataTable dt = null;

            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = OpenConnection();
                cmd.CommandText = query;
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                _dataAdapter.SelectCommand = cmd;

                _dataAdapter.Fill(ds);
                dt = ds.Tables[0];
            }
            return dt;
        }

        public List<string> GetQIDList(string query)
        {
            DataTable dtQID = ExecuteSelectQuery(query, null);
            List<string> qidList = new List<string>();
            var qids = from row in dtQID.AsEnumerable() select row["QID"].ToString();
            if (qids != null && qids.Count() > 0)
            {
                qidList = qids.ToList();
            }
            return qidList;
        }

        public void GetQIDData(string query, ref JObject tableObject, bool isArray = false)
        {
            DataTable dtQID = ExecuteSelectQuery(query, null);
            if (dtQID != null && dtQID.Rows.Count > 0)
            {
                if (dtQID.Rows.Count == 1 && isArray == false)
                {
                    CopyRowToJSONObject(dtQID.Rows[0], ref tableObject);
                }
                else
                {
                    CopyTableToJSONObject(dtQID, ref tableObject);
                }
            }
            else
            {
                if (isArray)
                    tableObject = null;
            }
        }

        #endregion

        private void CopyRowToJSONObject(DataRow dr, ref JObject tableObject)
        {
            foreach (var prop in tableObject.Properties())
            {
                if (dr.Table.Columns.Contains(prop.Name))
                {
                    prop.Value = dr[prop.Name].ToString();
                }
                else
                {
                    //TODO: log missing column in Access table
                }
            }
        }

        private void CopyTableToJSONObject(DataTable dt, ref JObject tableObject)
        {
            JArray arr = new JArray();
            foreach (DataRow dr in dt.Rows)
            {
                JObject clone = (JObject)tableObject.DeepClone();
                foreach (var prop in clone.Properties())
                {
                    if (dr.Table.Columns.Contains(prop.Name))
                    {
                        prop.Value = dr[prop.Name].ToString();
                    }
                    else
                    {
                        //TODO: log missing column in Access table
                    }
                }
                arr.Add(clone);
            }
            tableObject = new JObject(new JProperty("", arr));
        }
    }

    public class OleDbHelper
    {
        public string GetOleDbConnectingString(string mdfDbPath)
        {
            string strOledbConnectingString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + mdfDbPath + "; Persist Security Info=True;";
            return strOledbConnectingString;
        }
    }

}
