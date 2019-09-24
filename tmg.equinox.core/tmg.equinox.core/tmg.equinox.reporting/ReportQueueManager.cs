using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.reporting.Base;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Base.Mapping;
using tmg.equinox.reporting.Interface;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.reporting
{

 
    public class ReportQueueManager
    {
        IReportManager<BaseJobInfo> _reportManager;
        //IReportRepository _reportRepository;
        //IMapper<List<ReportMappingField>> _mapper;
        //IReportExecuter<BaseJobInfo> _reportExecuter;
        //IDbRepostory _dbRepostory;

        public ReportQueueManager(IReportManager<BaseJobInfo> reportManager)
        {
            _reportManager = reportManager;
            //_reportRepository = reportRepository;
            //_reportExecuter = reportExecuter;
            //_dbRepostory = dbRepostory;
            //_mapper = mapper;
        }
        //test/ing
        public ReportQueueManager()
        {
            _reportManager = new ReportManager<BaseJobInfo>(
                          new ReportConfig( new ReportRepository(null), new Mapper(), new SQLMapper())
                          , new ReportDbRepository());
        }

        public void Execute(BaseJobInfo queue)
        {
          //_reportManager.ReportExecuter = GetReport(queue);
          var result =  _reportManager.Execute(queue, GetReport(queue));
        }

        public IReportExecuter<BaseJobInfo> GetReport(BaseJobInfo queue)
        {
            //switch case
            return ReportFactory.ReportInstance<BaseJobInfo>(queue.FeatureId.ToEnum<enumReportType>());
        }
      
    }
}
