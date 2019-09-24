using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Interface;
using tmg.equinox.repository.models;

namespace tmg.equinox.reporting.Base
{

   
    public class ReportManager<Input>:IReportManager<Input> where Input : BaseJobInfo
    {
      //  IReportExecuter<Input> _reportExecuter;
        IReportConfig _reportConfig;
        IDbRepostory _dbRepostory;

        //public IReportExecuter<Input> ReportExecuter { get { return _reportExecuter; } set { _reportExecuter = value; } }

        public ReportManager(IReportConfig reportConfig, IDbRepostory dbRepostory)
        {
          //  _reportExecuter = reportExecuter;
            _reportConfig = reportConfig;
            _dbRepostory = dbRepostory;
        }

        public virtual IResult Execute(Input queueId, IReportExecuter<Input> reportExecuter)//this queueId need to replace with model
        {

            var reportSetting = _reportConfig.GetReportSettingById(Convert.ToInt32(queueId.FeatureId));

            var dataList = _dbRepostory.Get(reportExecuter.PrepareSQLStatement(reportSetting, queueId));

            if (dataList == null)
                return null;


            var result  = reportExecuter.CreateContainerBasedOnTemplate(reportSetting, dataList); //todo
            int rowNo = 0;
            int colNo = 0;
            int tableNo = 0;
            bool isNewRow = false;
            foreach (var row in dataList)
            {               
                foreach (var table in row.Data)
                {
                    isNewRow = true;
                    rowNo++;
                    foreach (var col in table)
                    {
                        colNo++;
                        if (reportSetting.isMapping)            
                        {
                            ExecuteWithMapping((IReportWIthMapExecuter<Input>)reportExecuter, reportSetting, col);
                        }
                        else
                        {
                            if (reportSetting.ReportId != 17)
                                reportExecuter.WriteInContainer(col, result, isNewRow, rowNo, colNo, tableNo);
                        }
                           
                        isNewRow = false;
                    }
                    colNo = 0;
                }
                rowNo = 0;
                tableNo++;
            }
            var logSQL = "select t.DesignType,t.SchemaName,t.Name as 'GeneratedName',  t.Label, c.Name as 'GeneratedFieldName', c.Label as 'Field', v.ErrorMessage as 'Error Message'  " +
                                "from rpt.ReportQueue q (nolock) inner join rpt.ReportQueueDetail (nolock) qd on q.ReportQueueId = qd.ReportQueueId" +
                                " inner join rpt.FormInstanceDetail i on i.FormInstanceID = qd.FormInstanceId " +
                                " inner join rpt.ValidationLog v (nolock) on v.DesignVersionId = i.FormDesignVersionID and v.FormInstanceId = i.FormInstanceID " +
                                " inner join rpt.ReportingTableColumnInfo c on c.ID = v.ReportingTableColumnInfoID " +
                                " inner join rpt.ReportingTableInfo t on t.ID = c.ReportingTableInfo_ID and t.DesignVersionId = v.DesignVersionId " +
                                " where q.ReportQueueId = " + queueId.QueueId;

          //  var techLog = _dbRepostory.Get(logSQL);
            //if (techLog != null)
            //    reportExecuter.AddtechincalLogInSheet(techLog);

            reportExecuter.UpdateResultInQueue(result, queueId);
            return result;
        }


        private void ExecuteWithMapping(IReportWIthMapExecuter<Input> _reportExecuter, ReportSetting reportSetting, KeyValuePair<string,object> col)
        {
            foreach (var mapping in reportSetting.mappings)
            {
                _reportExecuter.ApplyRule(col, mapping);
                _reportExecuter.WriteInContainer(col, mapping);
            }
        }
    }

}
