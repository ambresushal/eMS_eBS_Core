using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.interfaces
{
    public enum EmailTemplateTypes
    {
        Default,
        Workflow,
        ApprovalWorkflow,
        WorkFlowStateChangeNotification,
        AssignFolderMemberNotification,
        PSoTPreparationNotification,
        PBPImportQueueNotification,
        NewTaskAssignment,
        TaskCompletion,
        ChangesInPlanAndTaskAssignment,
        DueDateIsOver,
        NewTaskAssignmentForAllPlans
    }
}
