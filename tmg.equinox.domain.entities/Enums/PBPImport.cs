using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Enums
{
    public enum PBPUserActionList
    {
        AddPlanIneBS = 1,
        TerminatePlanFromeBS = 2,
        MapItWithAnothereBSPlan = 3,
        NoActionRequired = 4,
        UpdatePlan = 5
    }

    public enum ProcessStatusMasterCode
    {
        Queued = 1,
        InProgress = 2,
        Errored = 3,
        Complete = 4,
        Finalized = 5,
        NotScheduled = 6,
        Scheduled = 7,
        InReview = 8,
        Cancel = 9
    }

    public enum PBPImportErrorCode
    {
        Success = 0,
        QIDIsNotSame = 1,
        FileSchemeIsInvalid = 2,
        Exception = 3
    }

    public static class DocumentName
    {
        public const string MEDICARE = "Medicare";

        public const string PBPVIEW = "PBPView";

        public const string USERANAME = "TMG Super User";
    }


    public enum CustomRuleType
    {
        Repeater = 1,
        ChildPopUp = 2,
        MultiSelect = 3,
    }

    public static class ExportPreStatus
    {
        public const string InQueued = "Queued";
        public const string PreQueueInProcess = "In-Process";
        public const string PreQueueSuccess = "Success";
        public const string PreQueueFailed = "Failed";
        public const string PreQueueErrored = "Errored";
    }

}
