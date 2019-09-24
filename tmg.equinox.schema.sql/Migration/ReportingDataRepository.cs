using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.caching.Interfaces;
using tmg.equinox.config;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.models;
using tmg.equinox.schema.Base.Interface;
using tmg.equinox.schema.Base.Model;

namespace tmg.equinox.schema.sql.Migration
{
    public class ReportingDataRepository : IReportingDataRepository
    {
        private IRptUnitOfWorkAsync _unitOfWork;
        //JData _jData;
        private static readonly ILog _logger = LogProvider.For<ReportingDataRepository>();
        private static readonly Object _synclock = new Object();
        private static readonly Object _synclockGetReportintTableData = new Object();

        public ReportingDataRepository(ICacheProvider cacheProvider, IRptUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public List<ReportingTableInfo> GetTableInformation(JData jData)
        {
            lock (_synclockGetReportintTableData)
            {
                // _logger.Debug("Get reporting database table schema from WellCare database.");
                //_jData = jData;
                SQLSchemaRepository sqlSchemaRepository = new SQLSchemaRepository(null, _unitOfWork);
                return sqlSchemaRepository.GetTables(new JsonDesign { JsonDesignId = jData.FormDesignId, JsonDesignVersionId = jData.FormDesignVersionId }).ToList();
            }
        }

        public bool CheckFormInstanceIdExists(ReportingTableInfo table, string whereClauseForSequence, int formInstanceId)
        {
            // _logger.Debug("Check whether Form Instance Id already exists or not.");
            string sqlStatement = string.Format("SELECT TOP 1 * FROM [{0}].[{1}] WHERE FormInstanceId = {2} {3}", table.SchemaName, table.Name, formInstanceId, whereClauseForSequence);

            DataTable dt = ExecuteWithResultsOnReportingDB(sqlStatement);
            if (dt != null && dt.Rows.Count > 0)
                return true;
            else
                return false;
        }
        public bool CheckFormInstanceIdExists(int formInstanceId)
        {
            //_logger.Debug("Check whether Form Instance Id already exists or not.");
            string sqlStatement = string.Format("SELECT TOP 1 * FROM [RPT].[FormInstanceDetail] WHERE FormInstanceId = {0} ", formInstanceId);

            DataTable dt = ExecuteWithResultsOnReportingDB(sqlStatement);
            if (dt != null && dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public DataTable GetFormInstanceData(ReportingTableInfo table, int formInstanceId)
        {
            //_logger.Debug("Get Form Instance Data.");
            string sqlStatement = string.Format("SELECT * FROM [{0}].[{1}] WHERE FormInstanceId = {2}", table.SchemaName, table.Name, formInstanceId);
            return ExecuteWithResultsOnReportingDB(sqlStatement);
        }

        private DataTable ExecuteWithResultsOnReportingDB(string sqlStatement)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection connection = new SqlConnection(Config.GetReportingCenterConnectionString()))
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlStatement, connection))
                    {
                        adapter.SelectCommand.CommandTimeout = 3600;
                        adapter.Fill(ds);
                    }
                }
                return ds.Tables.Count > 0 ? ds.Tables[0] : null;
            }
            catch (Exception ex)
            {
                //_logger.ErrorException("Exceprion Occures while executing result query ["+ sqlStatement + "].", ex);
                return null;
                //throw ex; // Temporary commentted to get maximum data for reporting.
            }
        }
        public void ExecuteQueryOnReportingDB(string sqlStatement)
        {
            lock (_synclock)
            {
                ExecuteQuery(sqlStatement);
            }
        }

        void ExecuteQuery(string sqlStatement)
        {
            try
            {
                // _logger.Debug("Executed SQL statement on ReportingCenter database.");
                using (SqlConnection connection = new SqlConnection(Config.GetReportingCenterConnectionString()))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "sp_executesql";
                        cmd.Parameters.Add("@statement", SqlDbType.NText).Value = sqlStatement;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 3600;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Exceprion Occures while executing query [" + sqlStatement + "] on ReportingCenter database.", ex);
                throw ex; // Temporary commentted to get maximum data for reporting.
            }

        }


    }
}
