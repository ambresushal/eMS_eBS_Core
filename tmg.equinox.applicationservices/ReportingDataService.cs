using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.repository.interfaces;
using tmg.equinox.schema.Base;
using tmg.equinox.schema.Base.Interface;
using tmg.equinox.schema.Base.Model;
using tmg.equinox.schema.sql;
using tmg.equinox.schema.sql.Migration;

namespace tmg.equinox.applicationservices
{
    public class ReportingDataService : IReportingDataService
    {

        private IReportingDataRepository _reportingDataRepository;
        private static readonly ILog _logger = LogProvider.For<ReportingDataService>();
        private IMDMSyncDataService _mDMSyncDataService;

        public ReportingDataService(IRptUnitOfWorkAsync unitOfWork, IMDMSyncDataService mDMSyncDataService)
        {
            this._reportingDataRepository = new ReportingDataRepository(null, unitOfWork);
            _mDMSyncDataService = mDMSyncDataService;
        }

        public bool Run(JData jData)
        {
            try
            {
                SQLStatement sqlStatement = new SQLStatement(_reportingDataRepository, _mDMSyncDataService);
                var schemeManager = new SchemeManager(sqlStatement);
                schemeManager.UpdateDatabase(jData);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Issue found in " + jData.FormInstanceId.ToString(), ex);
            }
            return true;
        }

        public bool RunAsync(JData jData)
        {
            Task.Factory.StartNew(() => executeAsyn(jData));
            return true;
        }
        private void executeAsyn(JData jData)
        {
            try
            {
                SQLStatement sqlStatement = new SQLStatement(_reportingDataRepository, _mDMSyncDataService);
                var schemeManager = new SchemeManager(sqlStatement);
                schemeManager.UpdateDatabase(jData);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Issue found in " + jData.FormInstanceId.ToString(), ex);
            }
        }
        public bool MigrateExistingData(List<JData> lstjData)
        {
            foreach (JData jData in lstjData)
            {
                Run(jData);
            }
            return true;
        }
    }
}
