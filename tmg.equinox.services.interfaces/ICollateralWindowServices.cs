using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.CollateralWindowService;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface ICollateralWindowServices
    {
        /// <summary>
        /// Gets the list of CollateralProcessGovernance.
        /// </summary>
        /// <returns></returns>
        IEnumerable<CollateralProcessGovernanceViewModel> GetCollateralProcessGovernance();

        /// <summary>
        /// Gets the list of CollateralProcessQueue.
        /// <param name="ProcessGovernance1up">The ProcessGovernance1up.</param>
        /// </summary>
        /// <returns></returns>
        IEnumerable<CollateralProcessQueueViewModel> GetCollateralProcessQueue(int processGovernance1up);

        IEnumerable<CollateralProcessQueueViewModel> GetStatuswiseCollateralProcessQueue(int processGovernance1up, int status1up);
        void UpdateCollateralProcessGovData(int processGovernance1up, int processStatus1Up);
        void UpdateCollateralStatus(int collateralProcessQueue1Up, int processStatus1Up, string message);
        void UpdateCollateralFilePath(string strWordPath, ColleteralFilePath strPDFPath, int collateralProcessQueue1Up, int processStatus1Up);
        List<string> GetDataAndDesignJSON(int? formDesignId, int? formDesignVersionId, int? formInstanceId);
        void AddLogEntry(int tenantID, int processGovernance1up, int severity, string message);
        void LoadActivity(int tenantID, int processGovernance1up, string message);
        void UnLoadActivity(int tenantID, int processGovernance1up, string message);
        bool HasSBDesignTemplate(int templateReportID, int templateReportVersionID, out int formDesignVersionID);
        bool HasEOCCombinedDesignTemplate(int templateReportID, int templateReportVersionID, out int formDesignVersionID);
        List<SBCollateralProcessQueueViewModel> HasMultilpeAnchorDocument(int queue1Up);
        FormInstanceViewModel GetAnchorFromView(int viewFormInstanceId);
        void DeleteSevenDaysOldCollateral(int numberOfDays);
    }
    public class ColleteralFilePath
    {
        public string PDF { get; set; }
        public string PrintX { get; set; }
    }
}
