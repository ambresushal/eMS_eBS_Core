using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using tmg.equinox.applicationservices.viewmodels.WCReport;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IReportMasterService
    {
        IList<ReportViewModel> GetReportList();
        //ReportViewModel GetReportByReportId(int reportID);
    }
}
