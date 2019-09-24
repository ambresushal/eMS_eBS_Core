using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.applicationservices.viewmodels.MasterList;


namespace tmg.equinox.applicationservices.interfaces
{
    public interface ITaskListService
    {
        List<TaskListViewModel> GetTaskList(int tenantID);
        ServiceResult AddTask(int tenantID, string TaskDescription, string addedBy,bool IsStandardCheckbox);
        ServiceResult UpdateTask(int TaskID, string TaskDescription, string updatedBy, bool IsStandardCheckbox);
        ServiceResult DeleteTask(int TaskID, string updatedBy);
        IQueryable<TaskListViewModel> GetDPFTasksMasterList(int tenantId, int? userId, int wfStateId);
    }
}
