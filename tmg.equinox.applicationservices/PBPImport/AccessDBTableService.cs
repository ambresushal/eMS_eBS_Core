using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.applicationservices.PBPImport
{
    public class AccessDBTableService
    {
        #region Public Members
        public string OledbConnectingString;
        #endregion

        #region Public Methods
        public AccessDBTableService(string mdfDbPath)
        {
            OleDbHelper Helper = new OleDbHelper();
            OledbConnectingString = Helper.GetOleDbConnectingString(mdfDbPath);
        }

        public List<PBPPlanViewModel> ReadPBPTableData()
        {
            string TableName = "PBP";
            List<PBPPlanViewModel> PBPPlanList = new List<PBPPlanViewModel>();
            string Query = String.Concat("SELECT DISTINCT (QID),PBP_A_PLAN_NAME,PBP_A_CONTRACT_NUMBER,PBP_A_CONTRACT_PERIOD,PBP_A_EGHP_YN", " FROM ", "PBP");
            DataTable TableData = ReadTableData(Query, TableName);

            foreach (DataRow dr in TableData.Rows)
            {
                PBPPlanList.Add(new PBPPlanViewModel
                {
                    QID = dr["QID"].ToString(),
                    PBPPlanName = dr["PBP_A_PLAN_NAME"].ToString(),
                    //PBPPlanNumber = dr["PBP_A_CONTRACT_NUMBER"].ToString(),
                    PBPPlanNumber = dr["QID"].ToString(),
                    Year = (!string.IsNullOrEmpty(dr["PBP_A_CONTRACT_PERIOD"].ToString()).Equals(true)) ? Convert.ToInt32(dr["PBP_A_CONTRACT_PERIOD"].ToString()) : 0,
                    IsEGWPPlan = !string.IsNullOrEmpty(dr["PBP_A_EGHP_YN"].ToString()).Equals(true) ? (dr["PBP_A_EGHP_YN"].ToString().Equals("1") ? true : false) : false,
                    IsEGWP = dr["PBP_A_EGHP_YN"].ToString(),
                });
            }

            return PBPPlanList;
        }

        public List<PBPPlanViewModel> ReadPBPPlanAreaTableData()
        {
            string TableName = "PLAN_AREAS";
            List<PBPPlanViewModel> PBPPlanList = new List<PBPPlanViewModel>();
            string Query = String.Concat("SELECT DISTINCT (PBP_A_SQUISH_ID)", " FROM ", TableName);
            DataTable TableData = ReadTableData(Query, TableName);

            foreach (DataRow dr in TableData.Rows)
            {
                PBPPlanList.Add(new PBPPlanViewModel
                {
                    QID = dr["PBP_A_SQUISH_ID"].ToString(),
                });
            }
            return PBPPlanList;
        }

        public List<PBPPlanViewModel> ReadPBPRegionsTableData()
        {
            string TableName = "pbpregions";
            List<PBPPlanViewModel> PBPPlanList = new List<PBPPlanViewModel>();
            string Query = String.Concat("SELECT DISTINCT (PBP_A_SQUISH_ID)", " FROM ", TableName);
            DataTable TableData = ReadTableData(Query, TableName);

            foreach (DataRow dr in TableData.Rows)
            {
                PBPPlanList.Add(new PBPPlanViewModel
                {
                    QID = dr["PBP_A_SQUISH_ID"].ToString(),
                });
            }
            return PBPPlanList;

        }

        public bool IsTableExist(string tableName)
        {
            bool IsExist = false;
            OleDbConnection conn;
            try
            {
                conn = new OleDbConnection(OledbConnectingString);
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                conn.Close();
                if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == tableName.ToLower()))
                {
                    IsExist = true;
                }
                else
                {
                    IsExist = false;
                }
            }
            catch (Exception ex)
            {
                IsExist = false;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return IsExist;
        }

        private DataTable ReadTableData(string sqlQuery, string tableName)
        {
            OleDbConnection conn = null;
            OleDbCommand cmd = null;
            OleDbDataAdapter Reader = null;
            DataSet DataSet = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                using (conn = new OleDbConnection(OledbConnectingString))
                {
                    conn.Open();
                    Reader = new OleDbDataAdapter(sqlQuery, conn);
                    Reader.Fill(DataSet, tableName);
                    Reader.Dispose();
                    conn.Close();
                    dt = DataSet.Tables[tableName];
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }

        public int GetPlanYear()
        {
            string TableName = "PBP";
            int Year = 0;
            string Query = String.Concat("SELECT QID,PBP_A_CONTRACT_PERIOD", " FROM ", "PBP");
            DataTable TableData = ReadTableData(Query, TableName);

            foreach (DataRow dr in TableData.Rows)
            {
                if (!String.IsNullOrEmpty(dr["PBP_A_CONTRACT_PERIOD"].ToString()))
                {
                    Year = Convert.ToInt32(dr["PBP_A_CONTRACT_PERIOD"].ToString());
                    break;
                }
            }

            return Year;
        }

        public DataTable ReadTable(string tableName)
        {
            OleDbConnection conn = null;            
            OleDbDataAdapter Reader = null;
            DataSet DataSet = new DataSet();
            DataTable dt = new DataTable();

            string query = String.Concat("SELECT *", " FROM ", tableName);

            try
            {
                using (conn = new OleDbConnection(OledbConnectingString))
                {
                    conn.Open();
                    Reader = new OleDbDataAdapter(query, conn);
                    Reader.Fill(DataSet, tableName);
                    Reader.Dispose();
                    conn.Close();
                    dt = DataSet.Tables[tableName];
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }
       
        public DataTable GetTableList()
        {
            OleDbConnection conn = null;
            DataTable userTables = null;
            // Microsoft Access provider factory          
            using (conn = new OleDbConnection(OledbConnectingString))
            {
                conn.Open();

                string[] restrictions = new string[4];
                restrictions[3] = "Table";

                // Get list of user tables
                userTables = conn.GetSchema("Tables", restrictions);
            }

            List<string> tableNames = new List<string>();
            for (int i = 0; i < userTables.Rows.Count; i++)
            {
                tableNames.Add(userTables.Rows[i][2].ToString());
            }

            return userTables;
        }

        #endregion
    }
}
