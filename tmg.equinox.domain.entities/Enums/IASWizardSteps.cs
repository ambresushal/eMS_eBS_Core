using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Enums
{
    public enum IASWizardSteps
    {
        SetUp=1,
        ElementSelection,
        UpdateSelection,
        GenerateIAS,
    }

    public enum GlobalUpdateIASStatus
    {
        InProgress = 1,
        Complete,
        IASDownloadInProgress,
        IASDownloadComplete,
        ValidationInProgress,
        ErrorLogDownloadComplete,
        IASDownloadFailed,
        ErrorLogDownloadFailed,
        IASExecutionFailed,
        IASExecutionInProgress,
        IASExecutionComplete
    }
}
