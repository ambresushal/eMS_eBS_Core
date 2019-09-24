using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.Report;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IReportService
    {
        /// <summary>
        /// Gets the PlaceHolders and their DataPath in JSON object which is used to parse PlaceHolder values from JSON object.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formInstanceID"></param>
        /// <param name="reportID"></param>
        /// <param name="reportVersionID"></param>
        /// <returns></returns>
        List<ReportServiceViewModel> GetFormMappingData(int tenantId, int formInstanceID, string reportName);

        /// <summary>
        /// Parses the Values from the JSON data using the Datapath retrieved from the DB for the form elements.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="tenantId"></param>
        /// <param name="formInstanceID"></param>
        /// <param name="ReportName"></param>
        /// <param name="reportVersionID"></param>
        /// <returns></returns>
        List<ReportServiceViewModel> ParseMappingDataFromJSON(IFolderVersionServices service, int tenantId, int formInstanceID, out int ParseMappingDataFromJSON, int formDesignID, int folderVersionID, out string formData, out string adminFormData, out string NetworkTiers, string ReportName = "FaxBack");

        string GetDataPath(string propertyName, int formInstanceID);
        List<SBCReportServiceViewModel> GetSBCReportServiceMasterDataList(int tenantID);
        SBCReportHeaderViewModel GetSBCReportHeader(int tenantID, int forminstanceid, int accountid);    
    }
}
