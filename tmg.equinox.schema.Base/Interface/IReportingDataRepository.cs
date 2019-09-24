using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.models;
using tmg.equinox.schema.Base.Model;

namespace tmg.equinox.schema.Base.Interface
{
    public interface IReportingDataRepository
    {
        List<ReportingTableInfo> GetTableInformation(JData jData);
        bool CheckFormInstanceIdExists(ReportingTableInfo table, string whereClauseForSequence, int formInstanceId);
        bool CheckFormInstanceIdExists(int formInstanceId);
        void ExecuteQueryOnReportingDB(string sqlStatement);
        DataTable GetFormInstanceData(ReportingTableInfo table, int formInstanceId);
    }
}
