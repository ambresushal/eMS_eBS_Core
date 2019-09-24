using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.repository.models;

namespace tmg.equinox.reporting.Base.Interface
{
    public interface IReportConfig
    {
        ReportSetting GetReportSettingByName(string reportName);
        ReportSetting GetReportSettingById(int reportId);

        //send it without mapping & sql
        List<ReportSetting> GetReportSetting();
    }
}
