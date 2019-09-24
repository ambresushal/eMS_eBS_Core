using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IWorkFlowTaskMappingService
    {
        IList<KeyValue> GetWorkFlowList(int tenantId);
        IEnumerable<KeyValue> GetNonSelectedTaskList(int tenantId);
        ServiceResult UpdateApplicableWFTaskMap(int tenantId, int WfstateId, List<ApplicableTaskMapModel> applicableTaskMapModel, string userName);

        IEnumerable<KeyValue> GetApplicableWfTaskList(int tenantId, int WfstateId);
        ServiceResult SaveDPFTaskAndWorkflowMappings(int wfStateId, string taskDescription, DateTime addedDate, string addedBy, int tenantId);
    }
}