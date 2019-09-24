using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.PlanContactReport;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IPlanContactReportService
    {

      IEnumerable<PlanContactReportViewModel> GetPlanContactReportList(int tenantId, string groupName, string title, string name, DateTime effectiveDate, GridPagingRequest gridPagingRequest, out int totalCount);

      List<GroupContactReportViewModel> GetGroupContactList(int formInstanceId);

      List<BroakerContactReportViewModel> GetbroakerContactList(int formInstanceId);

      List<HSBContactReportViewModel> GetHSBContactList(int formInstanceId);
    }
}
